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

namespace second
{
    class Downloader
    {
        private TextWriter outFile;
        private Socket socket;

        private UInt32 connectionNumber;
        private UInt64 required;

        public Downloader(Socket s, TextWriter writer)
        {
            this.outFile = writer;
            this.socket = s;
        }

        public void InitConnection()
        {
            this.connectionNumber = CommunicationFacade.InitConnection(this.socket, Command.DOWNLOAD);
        }

        private DownloadPacket receive(CommunicationPacket p)
        {
            UInt16 minRequired = Convert.ToUInt16((required)&UInt16.MaxValue);
            UInt16 maxRequired = Convert.ToUInt16((required + (int)Sizes.WINDOW_SIZE)%UInt16.MaxValue);
            UInt64 modCurrent = this.required - (this.required & UInt16.MaxValue);
            DownloadPacket toReturn;

            Logger.WriteLine($"MinAccept: {minRequired}, MaxAccept: {maxRequired}");
            if (minRequired < maxRequired)
            {
                toReturn = new DownloadPacket(p.Data, p.ConnectionNumber, p.Flags, (UInt64)modCurrent + (UInt64)p.SerialNumber);
            }
                 //                                            CUR  
                 //                                             V
            else //packet over edge of UInt16 <----MAX.........MIN---->
            {
                UInt64 realSerial;               
                if (p.SerialNumber >= minRequired) //  <----MAX.........MIN---P-->
                    realSerial = (UInt64)modCurrent + (UInt64)p.SerialNumber;
                else //  <--P---MAX.........MIN----->
                    realSerial = modCurrent + (UInt64)UInt16.MaxValue + (UInt64)p.SerialNumber;
                toReturn = new DownloadPacket(p.Data, p.ConnectionNumber, p.Flags, realSerial);
            }
            Logger.WriteLine($"Downloader recive packet with serial={toReturn.SerialNumber}");
            return toReturn;
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
                    if (pack.Flags == (byte)Flag.FIN)
                    {
                        Logger.WriteLine("All data arrive", ConsoleColor.Cyan);
                        CommunicationFacade.Send(this.socket,new CommunicationPacket(this.connectionNumber, Convert.ToUInt16(this.required % UInt16.MaxValue), Convert.ToUInt16(this.required % UInt16.MaxValue),(byte)Flag.FIN,empty));
                        return;
                    }
                    if (pack.Flags == (byte)Flag.RST)
                        throw new CommunicationException();


                    queue.Enqueue(pack, pack.SerialNumber);

                    //Attach into priority queue
                    while (queue.Count > 0 && queue.First.SerialNumber <= this.required)
                    {
                        DownloadPacket toProccess = queue.Dequeue();
                        if (toProccess.SerialNumber < this.required)
                            continue;
                        this.outFile.Write(toProccess.Data);
                        this.required += (uint)toProccess.Data.Length;
                        if (toProccess.Data.Length != 255)
                        {
                            //TODO remove
                            Logger.WriteLine("Last packet arrive");
                            return;
                        }
                    }
                    Logger.WriteLine($"Waiting for packet {this.required}");
                    CommunicationFacade.Send(this.socket, new CommunicationPacket(this.connectionNumber, 0, Convert.ToUInt16(this.required % UInt16.MaxValue), 0, empty));
                }
            }
            catch(CommunicationException)
            {
                Logger.WriteLine("Occurs error during communication",ConsoleColor.Yellow);
                throw new TerminateException();
            }
        }
    }
}
