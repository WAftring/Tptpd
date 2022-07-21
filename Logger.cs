using System.Diagnostics;
namespace Tptpd
{
    static class Logger
    {
        [Flags]
        public enum LEVELS
        {
            FATAL = 0x1,
            ERROR = 0x3,
            INFO = 0x5,
            TRACE = 0x9
        }
        static LEVELS Level;

        public static void SetLevel(LEVELS set) { Level = set; }
        public static void Trace(string msg)
        {
            if (Level.HasFlag(LEVELS.TRACE))
            {
                string formatted = $"[{Thread.CurrentThread.ManagedThreadId}] INFO {msg}";
                Console.WriteLine(formatted);
                Debug.WriteLine(formatted);
            }
        }
        public static void Info(string msg)
        {
            if (Level.HasFlag(LEVELS.INFO))
            {
                string formatted = $"[{Thread.CurrentThread.ManagedThreadId}] INFO {msg}";
                Console.WriteLine(formatted);
                Debug.WriteLine(formatted);
            }
        }
        public static void Error(string msg)
        {
            if (Level.HasFlag(LEVELS.ERROR))
            {
                string formatted = $"[{Thread.CurrentThread.ManagedThreadId}] ERROR {msg}";
                Console.WriteLine(formatted);
                Debug.WriteLine(formatted);
            }
        }

        public static void Fatal(string msg, Exception e)
        {
            string formatted = $"[{Thread.CurrentThread.ManagedThreadId}] FATAL {msg}";
            Console.WriteLine(formatted);
            Debug.WriteLine(formatted);
            throw e;
        }
    }
}