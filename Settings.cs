namespace Tptpd
{
    class Settings
    {

        const string PARAM_ADD_IP = "--add-ip";
        const string PARAM_ADD_IPS = "--add-ips";
        const string SWITCH_HELP = "--help";
        public string[]? IPAddresses;
        public bool Valid { get; private set; }
        public Settings(string[] args)
        {
            try
            {
                if ((args[0] == PARAM_ADD_IP || args[0] == PARAM_ADD_IPS) && args.Length == 2)
                    IPAddresses = args[1].Split(",");
                else if (args[0] == SWITCH_HELP)
                    ShowHelp();
                else
                    throw new InvalidDataException("Invalid arguments");
                Valid = true;
            }
            catch (IndexOutOfRangeException e)
            {
                Console.WriteLine("Not enough arguments. --help for additional information");
                Valid = false;
            }
            catch (InvalidDataException e)
            {
                Console.WriteLine("Invalid arguments. --help for additional information");
                Valid = false;
            }
        }
        void ShowHelp()
        {
            Console.WriteLine("PARAMETERS:");
            Console.WriteLine("\t--add-ip [IP Address]\tAdd a client unicast IP address");
            Console.WriteLine("\t--add-ips [IP Address1,IP Address2, IP Address3]\tAdd multiple client unicast IP addresses seperated by ,");
            Console.WriteLine();
            Console.WriteLine("EXAMPLES:");
            Console.WriteLine("tptpd.exe --add-ip 127.0.0.1");
            Console.WriteLine("tptpd.exe -add-ips 192.168.1.10,192.168.1.11,192.168.1.12");
        }
    }
}