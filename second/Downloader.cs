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
            CommunicationFacade.InitConnection(this.socket,out this.connectionNumber,Command.DOWNLOAD);
        }

        private DownloadPacket receive()
        {
            byte flags;
            UInt32 connectionNumber;
            UInt16 serialNumber,confirmationNumber;
            byte[] data;
            CommunicationFacade.Receive(this.socket,out connectionNumber,out serialNumber,out confirmationNumber,out flags,out data);
            return new DownloadPacket(data,connectionNumber,flags,serialNumber);
        }

        public void AcceptFile()
        {
            LinkedList<DownloadPacket> PacketsToProccess = new LinkedList<DownloadPacket>();
            UInt64 last_received = 10;
            while(true)
            {
                DownloadPacket pack = receive();
                LinkedListNode<DownloadPacket> before = PacketsToProccess.First;
                //find before which to insert
                for (; before != null && before.Value.SerialNumber < pack.SerialNumber;before = before.Next) ;
                //insert
                if (before == null)
                    PacketsToProccess.AddFirst(pack);
                else
                    PacketsToProccess.AddBefore(before,pack);

            }
        }
    }
}
