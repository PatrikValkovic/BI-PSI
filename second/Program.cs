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
                int port = 4000;

                args = InitActions.ValidateArgs(args, "127.0.0.1","firmware.bin");

                IPAddress address = InitActions.ParseAddress(args[0]);

                Socket s = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
                try
                {
                    Logger.WriteLine($"Connecting to {address}:{port}");
                    InitActions.ConnectSocket(s, address, port);

                    if (args.Length == 1)
                    {
                        Logger.WriteLine("Download action will be use");
                        using (var fs = new FileStream("photo.png", FileMode.Create))
                        {
                            using (var str = new BinaryWriter(fs))
                            {
                                Downloader d = new Downloader(s, str);
                                d.InitConnection();
                                TimeMeasure timer = new TimeMeasure(true);
                                d.AcceptFile();
                                timer.ShowSpeed(d.Recived);
                            }
                        }
                    }
                    else if (args.Length == 2)
                    {
                        string file = args[1];
                        Logger.WriteLine($"Firmware upload action will be use with file {file}");
                        using (var fs = new FileStream(file, FileMode.Open))
                        {
                            using (var str = new BinaryReader(fs))
                            {
                                Uploader d = new Uploader(s, str);
                                d.InitConnection();
                                d.SendFile();
                            }
                        }
                    }
                }
                finally
                {
                    s.Dispose();
                }

            }
            catch (TerminateException)
            {
                Logger.WriteLine("Program will be terminated", ConsoleColor.Yellow);
            }
            catch (MaximumAttempException)
            {
                Logger.WriteLine("Program performed maximum attempts", ConsoleColor.Yellow);
            }
            catch (Exception e)
            {
                Logger.WriteLine($"Global exception '{e.Message}'", ConsoleColor.White, ConsoleColor.Red);
            }
            finally
            {
                Logger.WriteLine("End", ConsoleColor.Cyan);
                Console.ReadKey();
            }
        }
    }
}
