using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace AndroidSDK.PlatformTools
{
    public static class Program
    {
        public static async Task MainAsync(string name, string[] arguments)
        {
#if DEBUG
            if (!System.Diagnostics.Debugger.IsAttached)
            {
                System.Diagnostics.Debugger.Launch();
            }
#endif
            Console.CancelKeyPress += (s, a) =>
            {
                Environment.Exit(1);
            };

            var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, name);
            var process = new Process().SetStartInfo(path, arguments);
            await process.StartAsync(breakawayChildProcess: true);
#if DEBUG
            if (System.Diagnostics.Debugger.IsAttached)
            {
                Console.WriteLine("----------");
                Console.ReadKey();
            }
#endif
        }
    }
}
