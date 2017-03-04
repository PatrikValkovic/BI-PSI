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
            int i = 0;
            this.socket.ReceiveTimeout = 100;

            byte flags;
            while(i<20)
            {
                CommunicationFacade.Send(this.socket, 0, 0, 0, (byte)Flag.SYN, new byte[] { (byte)Command.DOWNLOAD });
                try
                {
                    while(true)
                    {
                        CommunicationFacade.Receive(this.socket, out this.connectionNumber, out this.serialNumber, out this.confirmationNumber, out flags, out data);
                        if (flags == (byte)Flag.SYN && data[0] == (byte)Command.DOWNLOAD)
                        {
                            Console.WriteLine($"Connection established - communication {this.connectionNumber:X}");
                            this.socket.ReceiveTimeout = 0;
                            return;
                        }
                        else
                            Console.WriteLine("Data obtained before connection packet was send, ignoring");
                    }
                }
                catch(SocketException e) when (e.SocketErrorCode == SocketError.TimedOut)
                {
                    Console.WriteLine($"Connection timeouted, attemp number {i+1}");
                    i++;
                }
            }
        }
    }
}
