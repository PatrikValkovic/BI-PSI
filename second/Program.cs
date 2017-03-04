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

                if (args.Length == 0 || args.Length > 2)
                {
                    Console.WriteLine("Usage: second <ip> [<file>]");
                    Console.WriteLine($"Default action will be use: download form ip {ip}");
                    args = new string[] { ip };
                }

                //ip validation
                ip = args[0];
                IPAddress address;
                if (!IPAddress.TryParse(ip, out address))
                {
                    Console.WriteLine($"Invalid IP address {ip}");
                    throw new TerminateException();
                };
                using (Socket s = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp))
                {
                    Console.WriteLine($"Connecting to {address}:{port}");
                    try { s.Connect(address, port); }
                    catch (SocketException e)
                    {
                        Console.WriteLine("Connection was not established");
                        throw new TerminateException();
                    }

                    if (args.Length == 1)
                    {
                        Console.WriteLine("Download action will be use");
                        CommunicationFacade.Send(s,0,0,0,4,new byte[] { 1 });
                    }
                    else if (args.Length == 2)
                    {
                        Console.WriteLine("Firmware upload action will be use");
                        //file validation
                        if (!File.Exists(args[1]))
                        {
                            Console.WriteLine($"Invalid file {args[1]}");
                            throw new TerminateException();
                        }
                        //TODO upload
                    }
                }
            }
            catch (Exception e)
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
