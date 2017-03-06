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
            //TODO transform
            return new DownloadPacket(p.Data,p.ConnectionNumber,p.Flags,p.SerialNumber);
        }

        public void AcceptFile()
        {
            byte[] empty = new byte[] { };
            LinkedList<DownloadPacket> PacketsToProccess = new LinkedList<DownloadPacket>();
            UInt64 required = 0;
            while(true)
            {
                DownloadPacket pack = this.receive();
                LinkedListNode<DownloadPacket> before = PacketsToProccess.First;
                //find before which to insert
                for (; before != null && before.Value.SerialNumber < pack.SerialNumber;before = before.Next) ;
                //insert
                if (before == null)
                    PacketsToProccess.AddFirst(pack);
                else
                    PacketsToProccess.AddBefore(before,pack);

                while(PacketsToProccess.First != null && PacketsToProccess.First.Value.SerialNumber <= required)
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
                CommunicationFacade.Send(this.socket,new CommunicationPacket(this.connectionNumber,0, Convert.ToUInt16(required),0,empty));
            }
        }
    }
}
