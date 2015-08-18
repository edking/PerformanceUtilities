using System.Runtime.InteropServices;

namespace PerformanceUtilities.TestPatterns
{
    public class HiResTimer
    {
        public static long Ticks
        {
            get
            {
                long t;
                QueryPerformanceCounter(out t);
                return t;
            }
        }

        public static long TicksPerSecond
        {
            get
            {
                long freq;
                QueryPerformanceFrequency(out freq);
                return freq;
            }
        }

        [DllImport("Kernel32.dll")]
        private static extern bool QueryPerformanceCounter(out long lpPerformanceCount);

        [DllImport("Kernel32.dll")]
        private static extern bool QueryPerformanceFrequency(out long lpFrequency);
    }
}