﻿using System.Threading.Tasks;

namespace AndroidSDK.PlatformTools.ADB
{
    public static class Program
    {
#pragma warning disable IDE1006 // Naming Styles
        public static async Task Main(string[] args)
#pragma warning restore IDE1006 // Naming Styles
        {
            await PlatformTools.Program.MainAsync("adb.exe", args);
        }
    }
}
