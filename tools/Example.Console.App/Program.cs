using System;
using System.Threading.Tasks;
using Microsoft.Win32;

namespace Example.Console.App
{
    class Program
    {
        static async Task Main(string[] args)
        {
            System.Console.WriteLine("Operation system");
            System.Console.WriteLine($"Platform: {Environment.OSVersion.Platform}");
            System.Console.WriteLine($"Version: {Environment.OSVersion.VersionString}");
            await Task.Delay(1000);
            System.Console.WriteLine($"Description: {System.Runtime.InteropServices.RuntimeInformation.OSDescription}");
            System.Console.WriteLine();

            await Task.Delay(5000);

            Get45PlusFromRegistry();
            System.Console.WriteLine();
        }

        private static void Get45PlusFromRegistry()
        {
            const string subkey = @"SOFTWARE\Microsoft\NET Framework Setup\NDP\v4\Full\";

            using (var ndpKey = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry32).OpenSubKey(subkey))
            {
                if (ndpKey != null && ndpKey.GetValue("Release") != null)
                {
                    System.Console.WriteLine($".NET Framework Version: {CheckFor45PlusVersion((int)ndpKey.GetValue("Release"))}");
                }
                else
                {
                    System.Console.WriteLine(".NET Framework Version 4.5 or later is not detected.");
                }
            }

            // Checking the version using >= enables forward compatibility.
            string CheckFor45PlusVersion(int releaseKey)
            {
                if (releaseKey >= 528040)
                    return "4.8 or later";
                if (releaseKey >= 461808)
                    return "4.7.2";
                if (releaseKey >= 461308)
                    return "4.7.1";
                if (releaseKey >= 460798)
                    return "4.7";
                if (releaseKey >= 394802)
                    return "4.6.2";
                if (releaseKey >= 394254)
                    return "4.6.1";
                if (releaseKey >= 393295)
                    return "4.6";
                if (releaseKey >= 379893)
                    return "4.5.2";
                if (releaseKey >= 378675)
                    return "4.5.1";
                if (releaseKey >= 378389)
                    return "4.5";
                // This code should never execute. A non-null release key should mean
                // that 4.5 or later is installed.
                return "No 4.5 or later version detected";
            }
        }
    }
}