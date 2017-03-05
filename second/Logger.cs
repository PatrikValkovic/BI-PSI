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

        public static void WriteLine(string text)
        {
            lock (locker)
            {
                Console.WriteLine(text);
            }
        }
    }
}
