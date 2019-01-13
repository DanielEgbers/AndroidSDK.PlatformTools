using System;
using System.IO;
using System.Threading.Tasks;

namespace AndroidSDK.PlatformTools.ADB
{
    public static class Program
    {
#pragma warning disable IDE1006 // Naming Styles
        public static async Task Main(string[] args)
#pragma warning restore IDE1006 // Naming Styles
        {
#if DEBUG
            if (!System.Diagnostics.Debugger.IsAttached)
            {
                System.Diagnostics.Debugger.Launch();
            }
#endif
            var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "adb.exe");
            var process = new WrappedProcess(path, args);
            await process.StartAsync
            (
                progress: new Progress<string>
                (
                    data => 
                        Console.WriteLine(data)
                )
            );
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
