using System;
namespace Tptpd
{
    class Program
    {
        public static void Main(string[] args)
        {
            Console.WriteLine("Tptpd vers 1.0");
#if DEBUG
            PtpServer ptpServer = new PtpServer();
            ptpServer.AddClient("127.0.0.1");
            ptpServer.Start();
#else
            Settings settings = new Settings(args);
            Logger.SetLevel(Logger.LEVELS.TRACE);
            if (settings.Valid)
            {
                PtpServer ptpServer = new PtpServer();
                foreach (var ip in settings.IPAddresses)
                    ptpServer.AddClient(ip);
                ptpServer.Start();
            }
#endif
        }
    }
}