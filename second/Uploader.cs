using second.Packets;
using second.Exceptions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace second
{
    class Uploader
    {
        private BinaryReader inFile;
        private Socket socket;

        private UInt32 connectionNumber;

        private TimeMeasure measure = new TimeMeasure();

        public Uploader(Socket s, BinaryReader reader)
        {
            this.inFile = reader;
            this.socket = s;
        }

        public void InitConnection()
        {
            this.connectionNumber = CommunicationFacade.InitConnection(this.socket, Command.UPLOAD);
        }

        private class SharedObject
        {
            public SharedObject(Socket s, UInt32 connectionNumber, BinaryReader reader, TimeMeasure measure)
            {
                this.Socket = s;
                this.ConnectionNumber = connectionNumber;
                this.Reader = reader;
                this.Measure = measure;
            }

            public object CountersLocker = new object();
            public UInt64 Confirmed = 0;
            public ushort ThisConfirmedTimes = 0;

            public uint ConnectionNumber;
            public Socket Socket;
            public Queue<UploadRecvPacket> ArriveQueue = new Queue<UploadRecvPacket>();
            public LinkedList<UploadSendPacket> SendedPackets = new LinkedList<UploadSendPacket>();
            public BinaryReader Reader;

            public volatile bool Ended = false;

            public TimeMeasure Measure;
        }

        static private void TimeoutCheckerThread(object Param)
        {
            Logger.WriteLine($"TimeoutChecker thread started with id {Task.CurrentId}");
            SharedObject data = (SharedObject)Param;
            UploadSendPacket p;
            while (!data.Ended)
            {
                p = null;

                //check the oldest packet
                lock (data.SendedPackets)
                {
                    if (data.SendedPackets.Count > 0)
                    {
                        TimeSpan diff = DateTime.Now - data.SendedPackets.First.Value.LastSend;
                        if (Math.Abs(diff.TotalMilliseconds) >= (ushort)PacketsProps.WAIT_TIME)
                        {
                            p = data.SendedPackets.First.Value;
                            data.SendedPackets.RemoveFirst();
                        }
                    }
                }

                //if expires oldes packet
                if (p != null)
                {
                    //check if is required to send it
                    UInt64 currentConfirmed;
                    lock (data.CountersLocker)
                    {
                        currentConfirmed = data.Confirmed;
                    }
                    bool sendIt = p.SerialNumber >= currentConfirmed;

                    //send it if needed
                    if (sendIt)
                    {
                        //socket have highter serial number that is confirmed number
                        p.Sended++;
                        p.LastSend = DateTime.Now;
                        if (p.Sended == (ushort)PacketsProps.MAX_ATTEMPS)
                            throw new MaximumAttempException();
                        Logger.WriteLine($"Packet {p.SerialNumber} timeouted, sends again");
                        CommunicationFacade.Send(data.Socket, p.CreatePacketToSend());
                        lock (data.SendedPackets)
                            data.SendedPackets.AddLast(p);
                    }
                }
                else //timeout not expired for the oldest packet
                    Thread.Sleep(0);
            }
        }

        static private UploadRecvPacket Validate(UploadRecvPacket p, UInt32 connectionNumber, UInt64 required)
        {
            if (p.ConnectionNumber != connectionNumber)
                throw new InvalidPacketException($"Connection number not match, required: {connectionNumber:X} accepted: {p.ConnectionNumber:X}");

            if (p.Flags > 0 && p.Flags != (byte)Flag.FIN && p.Flags != (byte)Flag.SYN && p.Flags != (byte)Flag.RST)
                throw new InvalidPacketException($"Invalid packet, obtained {Convert.ToString(p.Flags, 2)}");

            if (p.Flags == (byte)Flag.RST)
                throw new CommunicationException();

            UInt64 i = 0;
            UInt64 max = (UInt64)Sizes.WINDOW_PACKETS * 4;
            for (i = 0; i < max; i++)
                if (p.ConfirmationNumber + i * (UInt64)Sizes.MAX_DATA == required || p.ConfirmationNumber + i * (UInt64)Sizes.MAX_DATA == required + (UInt64)Sizes.WINDOW_SIZE)
                    break;
            if (i == max)
                throw new InvalidPacketNumberException($"Invalid packet serial, min required: {required}, obtained: {p.ConfirmationNumber}");

            if (p.Flags == (byte)Flag.FIN && p.Data.Length > 0)
                throw new InvalidPacketException("Packet with FIN contains data");

            return p;
        }

        static private void ProccessDataThread(object Param)
        {
            Logger.WriteLine($"ProccessData thread started with id {Task.CurrentId}");
            SharedObject data = (SharedObject)Param;
            UInt64 sended = 0;
            bool sendedLastSocket = false;
            bool lastPacketConfirmed = false;

            //fill first window
            for (int i = 0; i < 8; i++)
            {
                UploadSendPacket packet = new UploadSendPacket(data.ConnectionNumber, 0, data.Reader.ReadBytes(255), sended);
                packet.LastSend = new DateTime(1990, 1, 1);
                lock (data.SendedPackets)
                {
                    data.SendedPackets.AddFirst(packet);
                }
                sended += (uint)packet.Data.Length;
            }

            Logger.WriteLine("ProcessData fill first window");

            UploadRecvPacket p;
            while (!data.Ended && !lastPacketConfirmed)
            {
                //get item from queue
                p = null;
                lock (data.ArriveQueue)
                    if (data.ArriveQueue.Count > 0)
                        p = data.ArriveQueue.Dequeue();

                //if items exists
                if (p != null)
                {
                    //get current confirmation number
                    UInt64 currentConfirmation;
                    lock (data.CountersLocker)
                        currentConfirmation = data.Confirmed;

                    if (currentConfirmation == sended)
                        break;

                    try { p = Validate(p, data.ConnectionNumber, currentConfirmation); }
                    catch (InvalidPacketNumberException) when (sendedLastSocket && p.ConfirmationNumber == sended)
                    {
                        Logger.WriteLine($"Last packet with confirmation {p.ConfirmationNumber} arrive", ConsoleColor.Cyan);
                        lastPacketConfirmed = true;
                    }

                    //if has packet lower configmration that is current confirmation, it is irelevant
                    if (p.ConfirmationNumber <= currentConfirmation)
                        continue;

                    //packet is useful
                    Logger.WriteLine($"Recive confirmation {p.ConfirmationNumber}, previous confirmation {currentConfirmation}");
                    UInt64 bytesSucesfullySended = p.ConfirmationNumber - currentConfirmation;
                    ushort sendPackets = (ushort)((bytesSucesfullySended / (uint)Sizes.MAX_DATA));
                    lock (data.CountersLocker)
                        data.Confirmed = p.ConfirmationNumber;
                    for (uint i = 0; i < sendPackets && !sendedLastSocket; i++)
                    {
                        UploadSendPacket send = new UploadSendPacket(data.ConnectionNumber, 0, data.Reader.ReadBytes(255), sended);
                        sended += (uint)send.Data.Length;
                        send.LastSend = new DateTime(1900, 1, 1);
                        Logger.WriteLine($"Adding packet with serial {send.SerialNumber}");
                        if (send.Data.Length != (uint)Sizes.MAX_DATA)
                        {
                            Logger.WriteLine($"This socket will be final - serial {send.SerialNumber}", ConsoleColor.Cyan);
                            sendedLastSocket = true;
                        }
                        lock (data.SendedPackets)
                            data.SendedPackets.AddFirst(send);

                    }
                }
                //wait for arrive packet
                else
                    Thread.Sleep(0);
            }

            data.Measure.Stop();

            Logger.WriteLine("Sending FIN packet", ConsoleColor.Cyan);
            UploadSendPacket final = new UploadSendPacket(data.ConnectionNumber, (byte)Flag.FIN, new byte[] { }, sended);
            lock (data.SendedPackets)
                data.SendedPackets.AddFirst(final);

            //wait for FIN packet
            while (!data.Ended)
            {
                //get item from queue
                p = null;
                lock (data.ArriveQueue)
                    if (data.ArriveQueue.Count > 0)
                        p = data.ArriveQueue.Dequeue();

                //if items exists
                if (p != null)
                {
                    // get current confirmation number
                    UInt64 currentConfirmation;
                    lock (data.CountersLocker)
                        currentConfirmation = data.Confirmed;

                    p = Validate(p, data.ConnectionNumber, currentConfirmation);
                    if (p.Flags == (uint)Flag.FIN)
                    {
                        Logger.WriteLine("FIN packet arrive", ConsoleColor.Cyan);
                        data.Ended = true;
                    }
                }
                else
                    Thread.Sleep(0);
            }
        }

        static private void ReceiveThread(object Param)
        {
            Logger.WriteLine($"Receive thread started with id {Task.CurrentId}");
            SharedObject data = (SharedObject)Param;
            while (!data.Ended)
            {
                data.Socket.ReceiveTimeout = (int)PacketsProps.WAIT_TIME;
                try
                {
                    CommunicationPacket p = CommunicationFacade.Receive(data.Socket);
                    UInt64 currentPoint;
                    lock (data.CountersLocker)
                        currentPoint = data.Confirmed;

                    UInt64 confirmationNumber = CommunicationFacade.ComputeRealNumber(p.ConfirmationNumber, currentPoint, UInt16.MaxValue, (uint)Sizes.WINDOW_SIZE);
                    UploadRecvPacket recv = new UploadRecvPacket(p.ConnectionNumber, p.Flags, p.Data, confirmationNumber);

                    lock (data.ArriveQueue)
                        data.ArriveQueue.Enqueue(recv);
                }
                catch (SocketException e) when (e.SocketErrorCode == SocketError.TimedOut)
                { }
            }
        }

        public void SendFile()
        {
            SharedObject shared = new SharedObject(this.socket, this.connectionNumber, this.inFile, this.measure);

            Task timeoutChecker = new Task(TimeoutCheckerThread, shared);
            Task receive = new Task(ReceiveThread, shared);
            Task dataProccess = new Task(ProccessDataThread, shared);
            Task[] tasks = new Task[] { timeoutChecker, receive, dataProccess };

            try
            {
                measure.Start();

                foreach (Task t in tasks)
                    t.Start();

                Task<Task> waiter = Task.WhenAny(timeoutChecker, receive, dataProccess);
                waiter.Wait();

                Task whoEndIt = waiter.Result;
                Logger.WriteLine($"Thread {whoEndIt.Id} ended his work");
                if (whoEndIt.Exception != null)
                    throw whoEndIt.Exception.InnerException;

            }
            catch (MaximumAttempException)
            {
                Logger.WriteLine("Maximum attemp of send, sending RST packet", ConsoleColor.Yellow);
                CommunicationFacade.Send(this.socket, new CommunicationPacket(this.connectionNumber, 0, 0, (byte)Flag.RST, new byte[] { }));
                throw;
            }
            catch (InvalidPacketException e)
            {
                Logger.WriteLine($"Invalid packet ({e.Message}), sending RST packet", ConsoleColor.Yellow);
                CommunicationFacade.Send(this.socket, new CommunicationPacket(this.connectionNumber, 0, 0, (byte)Flag.RST, new byte[] { }));
                throw;
            }
            catch (CommunicationException)
            {
                Logger.WriteLine($"Error occurs during communication", ConsoleColor.Yellow);
                throw;
            }
            finally
            {
                shared.Ended = true;
                foreach (Task t in tasks)
                {
                    if (t.Status == TaskStatus.Running)
                        t.Wait();
                    t.Dispose();
                }
                this.measure.ShowSpeed(shared.Confirmed);
            }
        }
    }
}
