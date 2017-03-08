using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net;
using System.Threading;
using System.IO;
using second.Exceptions;

namespace second
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                string ip = "127.0.0.1";
                int port = 4000;

                args = InitActions.ValidateArgs(args, ip);

                //ip validation
                ip = args[0];
                IPAddress address = InitActions.ParseAddress(ip);

                using (Socket s = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp))
                {
                    Logger.WriteLine($"Connecting to {address}:{port}");
                    InitActions.ConnectSocket(s, address, port);

                    if (args.Length == 1)
                    {
                        Logger.WriteLine("Download action will be use");
                        using (var fs = new FileStream("photo.png",FileMode.Create))
                        {
                            using (var str = new BinaryWriter(fs))
                            {
                                Downloader d = new Downloader(s, str);
                                d.InitConnection();
                                d.AcceptFile();
                            }
                        }
                    }
                    else if (args.Length == 2)
                    {
                        Logger.WriteLine($"Firmware upload action will be use with file {args[1]}");
                        using (var str = new StreamReader(File.OpenRead(args[1])))
                        {
                            Uploader d = new Uploader(s, str);
                        }
                    }
                }
            }
            catch (TerminateException)
            {
                Logger.WriteLine("Program will be terminated", ConsoleColor.Yellow);
                return;
            }
            finally
            {
                Logger.WriteLine("End", ConsoleColor.Cyan);
                Console.ReadKey();
            }
        }
    }
}
