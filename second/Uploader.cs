using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace second
{
    class Uploader
    {
        private StreamReader inFile;
        private Socket socket;
        public Uploader(Socket s, StreamReader reader)
        {
            this.inFile = reader;
            this.socket = s;
        }
    }
}
