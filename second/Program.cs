using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net;
using System.Threading;
using System.IO;

namespace second
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                string ip = "192.168.56.101";
                int port = 4000;
                string file = "";

                args = InitActions.ValidateArgs(args, ip);

                //ip validation
                ip = args[0];
                IPAddress address = InitActions.ParseAddress(ip);
                
                using (Socket s = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp))
                {
                    Console.WriteLine($"Connecting to {address}:{port}");
                    InitActions.ConnectSocket(s,address,port);

                    if (args.Length == 1)
                    {
                        Console.WriteLine("Download action will be use");
                        using (var str = new StreamWriter(File.OpenWrite("photo.png")))
                        {
                            Downloader d = new Downloader(s, str);
                            d.initConnection();
                        }
                    }
                    else if (args.Length == 2)
                    {
                        Console.WriteLine($"Firmware upload action will be use with file {args[1]}");
                        using (var str = new StreamReader(File.OpenRead(args[1])))
                        {
                            Uploader d = new Uploader(s, str);
                        }
                    }
                }
            }
            catch (TerminateException e)
            {
                Console.WriteLine("Program will be terminated");
            }
            finally
            {
                Console.WriteLine("End");
                Console.ReadKey();
            }
        }
    }
}
