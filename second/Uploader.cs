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
        private UInt64 windowBegin;

        public Uploader(Socket s, BinaryReader reader)
        {
            this.inFile = reader;
            this.socket = s;
        }

        public void InitConnection()
        {
            this.connectionNumber = CommunicationFacade.InitConnection(this.socket, Command.UPLOAD);
        }

        public UInt64 Sended { get { return this.windowBegin; } }

        private class SharedObject
        {
            public SharedObject(Socket s, UInt32 connectionNumber, BinaryReader reader)
            {
                this.Socket = s;
                this.ConnectionNumber = connectionNumber;
                this.Reader = reader;
            }

            public object CountersLocker = new object();
            public UInt64 Confirmed = 0;
            public ushort ThisConfirmedTimes = 0;

            public uint ConnectionNumber;
            public Socket Socket;
            public Queue<CommunicationPacket> ArriveQueue = new Queue<CommunicationPacket>();
            public LinkedList<UploadSendPacket> SendedPackets = new LinkedList<UploadSendPacket>();
            public BinaryReader Reader;

            public volatile bool Ended = false;
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
                        TimeSpan diff = new DateTime() - data.SendedPackets.First.Value.LastSend;
                        if(Math.Abs(diff.TotalMilliseconds) >= (ushort)PacketsProps.WAIT_TIME)
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
                    lock(data.CountersLocker)
                    {
                        currentConfirmed = data.Confirmed;
                    }
                    bool sendIt = p.SerialNumber >= currentConfirmed;

                    //send it if needed
                    if (sendIt)
                    {
                        //socket have highter serial number that is confirmed number
                        p.Sended++;
                        p.LastSend = new DateTime();
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

        static private void ProccessDataThread(object Param)
        {
            Logger.WriteLine($"ProccessData thread started with id {Task.CurrentId}");
            SharedObject data = (SharedObject)Param;
            UInt64 sended = 0;

            //fill first window
            for (int i=0;i<8;i++)
            {
                UploadSendPacket p = new UploadSendPacket(data.ConnectionNumber,0,data.Reader.ReadBytes(255),sended);
                p.LastSend = new DateTime(1990,1,1);
                lock(data.SendedPackets)
                {
                    data.SendedPackets.AddFirst(p);
                }
                sended += (uint)p.Data.Length;
            }

            Logger.WriteLine("ProcessData fill first window");
            while (true) ;

            //loop in ArriveQueue
                //send new or send first packet
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
                    //TODO validation?
                    lock (data.ArriveQueue)
                    {
                        data.ArriveQueue.Enqueue(p);
                    }
                }
                catch (SocketException e) when (e.SocketErrorCode == SocketError.TimedOut)
                { }
            }
        }

        public async void SendFile()
        {
            SharedObject shared = new SharedObject(this.socket,this.connectionNumber,this.inFile);

            Task timeoutChecker = new Task(TimeoutCheckerThread, shared);
            Task receive = new Task(ReceiveThread, shared);
            Task dataProccess = new Task(ProccessDataThread, shared);
            Task[] tasks = new Task[] { timeoutChecker, receive, dataProccess };

            try
            {
                foreach (Task t in tasks)
                    t.Start();

                Task<Task> waiter = Task.WhenAny(timeoutChecker, receive, dataProccess);
                waiter.Wait();

                Task whoEndIt = waiter.Result;
                Logger.WriteLine($"Thread {whoEndIt.Id} ended his work");
                //TODO do something?

            }
            finally
            {
                shared.Ended = true;
                foreach (Task t in tasks)
                {
                    t.Wait();
                    t.Dispose();
                }
            }
        }
    }
}
