using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using second.Packets;
using Priority_Queue;
using second.Exceptions;
using System.Diagnostics;

namespace second
{
    class Downloader
    {
        private BinaryWriter outFile;
        private Socket socket;

        private UInt32 connectionNumber;
        private UInt64 required;
        private bool waitingToFin;

        private Stopwatch begin;

        public Downloader(Socket s, BinaryWriter writer)
        {
            this.outFile = writer;
            this.socket = s;
            this.waitingToFin = false;
        }

        public void InitConnection()
        {
            this.connectionNumber = CommunicationFacade.InitConnection(this.socket, Command.DOWNLOAD);
            this.begin = new Stopwatch();
            this.begin.Start();
        }

        private DownloadPacket receive(CommunicationPacket p)
        {
            UInt64 realSerial = CommunicationFacade.ComputeRealNumber(p.SerialNumber,this.required,UInt16.MaxValue,(uint)Sizes.WINDOW_SIZE);
            DownloadPacket toReturn = new DownloadPacket(p.Data, p.ConnectionNumber, p.Flags, realSerial); ;
            Logger.WriteLine($"Downloader recive packet with serial={toReturn.SerialNumber}");
            return toReturn;
        }

        private DownloadPacket validatePacket(DownloadPacket p)
        {
            if (p.ConnectionNumber != this.connectionNumber)
                throw new InvalidPacketException($"Connection number not match, required: {this.connectionNumber:X} accepted: {p.ConnectionNumber:X}");

            if (p.Flags > 0 && p.Flags != (byte)Flag.FIN && p.Flags != (byte)Flag.SYN && p.Flags != (byte)Flag.RST)
                throw new InvalidPacketException($"Invalid packet, obtained {Convert.ToString(p.Flags, 2)}");

            if (!waitingToFin)
            {
                UInt64 i = 0;
                UInt64 max = (UInt64)Sizes.WINDOW_PACKETS * 4;
                for (i = 0; i < max; i++)
                    if (p.SerialNumber + i * (UInt64)Sizes.MAX_DATA == this.required || p.SerialNumber + i * (UInt64)Sizes.MAX_DATA == this.required + (UInt64)Sizes.WINDOW_SIZE)
                        break;
                if (i == max)
                    throw new InvalidPacketException($"Invalid packet serial, min required: {this.required}, obtained: {p.SerialNumber}");
            }

            if (p.Flags == (byte)Flag.FIN && p.Data.Length > 0)
                throw new InvalidPacketException("Packet with FIN contains data");


            if (p.Flags == (byte)Flag.RST)
                throw new CommunicationException();
            return p;
        }

        public void AcceptFile()
        {
            try
            {
                byte[] empty = new byte[] { };
                IPriorityQueue<DownloadPacket, UInt64> queue = new SimplePriorityQueue<DownloadPacket, UInt64>();
                //TODO add priority queue

                while (true)
                {
                    DownloadPacket pack = this.receive(CommunicationFacade.Receive(this.socket));
                    pack = this.validatePacket(pack);

                    if (pack.Flags == (byte)Flag.FIN)
                    {
                        Logger.WriteLine("All data arrive", ConsoleColor.Cyan);
                        CommunicationFacade.Send(this.socket, new CommunicationPacket(this.connectionNumber, 0, Convert.ToUInt16(this.required & UInt16.MaxValue), (byte)Flag.FIN, empty));
                        return;
                    }


                    queue.Enqueue(pack, pack.SerialNumber);

                    //Attach into priority queue
                    while (queue.Count > 0 && queue.First.SerialNumber <= this.required)
                    {
                        DownloadPacket toProccess = queue.Dequeue();
                        if (toProccess.SerialNumber < this.required)
                            continue;
                        Logger.WriteLine($"Accepted packet {toProccess.SerialNumber}");
                        this.outFile.Write(toProccess.Data);
                        this.required += (uint)toProccess.Data.Length;
                        if (toProccess.Data.Length != 255)
                        {
                            Logger.WriteLine("Last packet arrive, waiting to FIN packet", ConsoleColor.Cyan);
                            this.waitingToFin = true;
                        }
                    }
                    Logger.WriteLine($"Waiting for packet {this.required}");
                    CommunicationFacade.Send(this.socket, new CommunicationPacket(this.connectionNumber, 0, Convert.ToUInt16(this.required & UInt16.MaxValue), 0, empty));
                }
            }
            catch (CommunicationException)
            {
                Logger.WriteLine("Occurs error during communication", ConsoleColor.Yellow);
                throw new TerminateException();
            }
            catch (InvalidPacketException e)
            {
                Logger.WriteLine($"Obtained invalid packet: {e.Message}", ConsoleColor.Yellow);
                CommunicationFacade.Send(this.socket, new CommunicationPacket(this.connectionNumber, 0, Convert.ToUInt16(this.required & UInt16.MaxValue), (byte)Flag.RST, new byte[] { }));
                throw new TerminateException();
            }
        }

        public void ShowSpeed()
        {
            this.begin.Stop();
            double kb = (double)this.required / 1024.0;

            double seconds = (double)this.begin.ElapsedMilliseconds / 1000.0;

            double avarage = kb / seconds;

            Logger.WriteLine($"Avarage speed: {avarage:0.00}KB/s, total time: {seconds:0.00}, size: {kb:0.00}KB", ConsoleColor.Cyan);
        }
    }
}
