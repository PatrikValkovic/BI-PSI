using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Threading.Tasks;
using System.IO;
using System.Net.Sockets;
using second.Exceptions;

namespace second
{
    class InitActions
    {
        public static string[] ValidateArgs(string[] args, string ip)
        {
            if (args.Length == 0 || args.Length > 2)
            {
                Console.WriteLine("Usage: second <ip> [<file>]");
                Console.WriteLine($"Default action will be use: download form ip {ip}");
                return new string[] { ip };
            }
            if(args.Length == 2)
            {
                if (!File.Exists(args[1]))
                {
                    Console.WriteLine($"Invalid file {args[1]}");
                    throw new TerminateException();
                }
            }
            return args;
        }

        public static IPAddress ParseAddress(string ip)
        {
            IPAddress address;
            if (!IPAddress.TryParse(ip, out address))
            {
                Console.WriteLine($"Invalid IP address {ip}");
                throw new TerminateException();
            };
            return address;
        }

        public static void ConnectSocket(Socket s, IPAddress address, int port)
        {
            try { s.Connect(address, port); }
            catch (SocketException)
            {
                Console.WriteLine("Connection was not established");
                throw new TerminateException();
            }
        }
    }
}
