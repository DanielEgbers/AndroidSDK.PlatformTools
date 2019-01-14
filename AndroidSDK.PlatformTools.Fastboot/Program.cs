using System.Threading.Tasks;

namespace AndroidSDK.PlatformTools.Fastboot
{
    public static class Program
    {
#pragma warning disable IDE1006 // Naming Styles
        public static async Task Main(string[] args)
#pragma warning restore IDE1006 // Naming Styles
        {
            await PlatformTools.Program.MainAsync("fastboot.exe", args);
        }
    }
}
