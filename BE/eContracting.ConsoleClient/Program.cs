using System;
using System.Linq;
using System.Threading.Tasks;
using eContracting.Services;

namespace eContracting.ConsoleClient
{
    public class Program
    {
        protected static string ServiceUrl = "http://lv423075.aci3.rwegroup.cz:8001/sap/bc/srt/rfc/sap/zcch_cache_api/100/zcch_cache_api/zcch_cache_api";
        protected static string ServiceUser = "UkZDX1NJVEVDT1JF";
        protected static string ServicePassword = "QWRIYjI0Nyo=";

        public static async Task Main(string[] args)
        {
            await RunAsync(args);

            #if DEBUG
            Console.ReadKey();
            #endif
        }

        public static async Task RunAsync(string[] args)
        {
            Console.WriteLine("Welcome to eContracting connector");
            string guid;
            string command;

            if (args.Length == 0)
            {
                Console.WriteLine("Define parameters:");
                Console.Write("GUID: ");
                var input1 = Console.ReadLine();
                Console.Write("Command / type: ");
                var input2 = Console.ReadLine();
                args = new[] { input1, input2 };
            }

            if (args.Length < 2)
            {
                Console.WriteLine("Invalid arguments");
                return;
            }

            guid = args[0];
            command = args[1];

            if (!CacheApiService.AvailableRequestTypes.Contains(command))
            {
                Console.WriteLine("Invalid command: " + command);
                return;
            }

            try
            {
                var options = new CacheApiServiceOptions(ServiceUser, ServicePassword, ServiceUrl);
                var service = new CacheApiService(options);
                var response = await service.GetResponse(guid, command);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }
    }
}
