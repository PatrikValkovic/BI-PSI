using System;
using System.Collections.Generic;
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
        private StreamReader inFile;
        private Socket socket;
        public Uploader(Socket s, StreamReader reader)
        {
            this.inFile = reader;
            this.socket = s;
        }

        static private void TimeoutCheckerThread(object Param)
        {
            Console.WriteLine("TimeoutChecker thread started");
        }

        static private void ProccessDataThread(object Param)
        {
            Console.WriteLine("ProccessData thread started");
        }

        static private void ReceiveThread(object Param)
        {
            Console.WriteLine("Receive thread started");
        }

        public void SendFile()
        {
            Thread timeoutChecker = new Thread(TimeoutCheckerThread);
            Thread receiver = new Thread(ReceiveThread);
            Thread processData = new Thread(ProccessDataThread);

            receiver.Start(new object());
            timeoutChecker.Start(new object());
            processData.Start(new object());

            receiver.Join();
            timeoutChecker.Join();
            processData.Join();
        }
    }
}
