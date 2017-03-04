using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace second
{
    class Downloader
    {
        private StreamWriter outFile;
        private Socket socket;

        private UInt32 connectionNumber;
        private UInt16 serialNumber;
        private UInt16 confirmationNumber;
        private byte[] data;

        public Downloader(Socket s, StreamWriter writer)
        {
            this.outFile = writer;
            this.socket = s;
        }

        public void initConnection()
        {
            CommunicationFacade.InitConnection(this.socket,out this.connectionNumber,Command.DOWNLOAD);
        }
    }
}
