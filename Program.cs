using System;
namespace Tptpd
{
    class Program
    {
        public static void Main(string[] args)
        {
            Console.WriteLine("Tptpd vers 1.0");
            Settings settings = new Settings(args);
            if (settings.Valid)
            {
                PtpServer ptpServer = new PtpServer();
#if DEBUG
                ptpServer.AddClient("127.0.0.1");
#else
                foreach (var ip in settings.IPAddresses)
                    ptpServer.AddClient(ip);
#endif
                ptpServer.Start();
            }
        }
    }
}