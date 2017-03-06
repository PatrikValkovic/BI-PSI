using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using second.Packets;

namespace second
{
    class Downloader
    {
        private StreamWriter outFile;
        private Socket socket;

        private UInt32 connectionNumber;
        private UInt64 required;

        public Downloader(Socket s, StreamWriter writer)
        {
            this.outFile = writer;
            this.socket = s;
        }

        public void InitConnection()
        {
            this.connectionNumber = CommunicationFacade.InitConnection(this.socket, Command.DOWNLOAD);
        }

        private DownloadPacket receive()
        {
            CommunicationPacket p = CommunicationFacade.Receive(this.socket);
            UInt16 minRequired = Convert.ToUInt16(required);
            UInt16 maxRequired = Convert.ToUInt16(required + (int)Sizes.WINDOW_SIZE);
            UInt64 modCurrent = this.required - (this.required & UInt16.MaxValue);
            DownloadPacket toReturn;
            if (minRequired < maxRequired)
            {
                toReturn = new DownloadPacket(p.Data, p.ConnectionNumber, p.Flags, (UInt32)modCurrent + (UInt32)p.SerialNumber);
            }
            else
            {
                toReturn = new DownloadPacket(p.Data, p.ConnectionNumber, p.Flags, modCurrent + (UInt32)UInt16.MaxValue + (UInt32)p.SerialNumber);
            }
            Logger.WriteLine($"Downloader recive packet with serial={toReturn.SerialNumber}");
            return toReturn;
        }

        public void AcceptFile()
        {
            byte[] empty = new byte[] { };
            //TODO add priority queue
            required = 0;

            while (true)
            {
                DownloadPacket pack = this.receive();
                
                //Attach into priority queue

                while (PacketsToProccess.First != null && PacketsToProccess.First.Value.SerialNumber <= required)
                {
                    DownloadPacket toProccess = PacketsToProccess.First.Value;
                    PacketsToProccess.RemoveFirst();
                    if (toProccess.SerialNumber < required)
                        continue;
                    this.outFile.Write(toProccess.Data);
                    required += (uint)toProccess.Data.Length;
                    if (toProccess.Data.Length != 255)
                        return;
                }
                CommunicationFacade.Send(this.socket, new CommunicationPacket(this.connectionNumber, 0, Convert.ToUInt16(required), 0, empty));
            }
        }
    }
}
