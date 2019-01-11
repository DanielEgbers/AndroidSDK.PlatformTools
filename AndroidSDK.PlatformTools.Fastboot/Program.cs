using System;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace AndroidSDK.PlatformTools.Fastboot
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            var processInfo = new ProcessStartInfo
            (
                fileName: Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "fastboot.exe"),
                arguments: string.Join(" ", args.Select(a => $"\"{a}\""))
            );
            Process.Start(processInfo).WaitForExit();

            if (Debugger.IsAttached)
            {
                Console.ReadKey();
            }
        }
    }
}
