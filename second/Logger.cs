using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace second
{
    class Logger
    {
        private static object locker = new object();

        public static void WriteLine(string text, ConsoleColor foreground = ConsoleColor.White, ConsoleColor background = ConsoleColor.Black)
        {
            lock (locker)
            {
                Console.BackgroundColor = background;
                Console.ForegroundColor = foreground;
                Console.WriteLine(text);
            }
        }
    }
}
