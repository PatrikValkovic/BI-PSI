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

        public void AcceptFile()
        {
            Thread timeoutChecker = new Thread(TimeoutCheckerThread);
            Thread receiver = new Thread(ReceiveThread);
            Thread processData = new Thread(ProccessDataThread);

            receiver.Start(new object());
            timeoutChecker.Start(new object());
            processData.Start(new object());

            receiver.Join();
            timeoutChecker.Join() ;
            processData.Join();
        }
    }
}
