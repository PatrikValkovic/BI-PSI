using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

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

        public void AcceptFile()
        {
            
        }
    }
}
