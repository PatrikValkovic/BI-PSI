using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace second
{
    class TimeMeasure
    {
        private Stopwatch timer;

        public TimeMeasure(bool begin = false)
        {
            timer = new Stopwatch();
            if (begin)
                this.Start();
        }

        public void Start()
        {
            timer.Restart();
        }

        public void Stop()
        {
            timer.Stop();
        }

        public void ShowSpeed(UInt64 bytes)
        {
            if (timer.IsRunning)
                this.Stop();

            double kb = bytes / 1024.0;

            double seconds = timer.ElapsedMilliseconds / 1000.0;

            double avarage = kb / seconds;

            Logger.WriteLine($"Avarage speed: {avarage:0.00}KB/s, total time: {seconds:0.00}, size: {kb:0.00}KB", ConsoleColor.Cyan);
        }
    }
}
