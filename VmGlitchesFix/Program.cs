using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Security.Principal;
using Microsoft.Win32;

class Program
{
    static void Main()
    {
        // Check if the application has administrator rights
        if (!IsAdministrator())
        {
            Console.WriteLine("This application must be run with administrator privileges.");
            return;
        }

        // Add the application to startup if not already added
        AddToStartup();

        // Set the priority of the audiodg.exe process
        SetAudiodgProcessPriority(ProcessPriorityClass.High);

        // Set ProcessorAffinity for the audiodg.exe process
        SetAudiodgProcessorAffinity(new IntPtr(4));

        Console.WriteLine("Operations completed successfully.");

        // Close the application
        Environment.Exit(0);
    }

    // Method to check for administrator rights
    static bool IsAdministrator()
    {
        WindowsIdentity identity = WindowsIdentity.GetCurrent();
        WindowsPrincipal principal = new WindowsPrincipal(identity);
        return principal.IsInRole(WindowsBuiltInRole.Administrator);
    }

    // Method to set the priority of the audiodg process
    static void SetAudiodgProcessPriority(ProcessPriorityClass priority)
    {
        Process audiodgProcess = GetAudiodgProcess();

        if (audiodgProcess != null)
        {
            try
            {
                audiodgProcess.PriorityClass = priority;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to set priority for the audiodg process: {ex.Message}");
            }
        }
        else
        {
            Console.WriteLine("Audiodg process not found.");
        }
    }

    // Method to set ProcessorAffinity for the audiodg process
    static void SetAudiodgProcessorAffinity(IntPtr processorAffinity)
    {
        Process audiodgProcess = GetAudiodgProcess();

        if (audiodgProcess != null)
        {
            try
            {
                audiodgProcess.ProcessorAffinity = processorAffinity;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to set ProcessorAffinity for the audiodg process: {ex.Message}");
            }
        }
        else
        {
            Console.WriteLine("Audiodg process not found.");
        }
    }

    // Method to get the audiodg process
    static Process GetAudiodgProcess()
    {
        Process[] processes = Process.GetProcessesByName("audiodg");
        return processes.Length > 0 ? processes[0] : null;
    }

    // Method to add the application to startup
    static void AddToStartup()
    {
        const string appName = "VmGlitchesFix";
        const string appPath = @"E:\Projects\VmGlitchesFix\VmGlitchesFix\bin\Debug\net5.0\VmGlitchesFix.exe"; // Specify the correct path

        // Check if the application is already in startup
        if (!IsInStartup(appName))
        {
            try
            {
                // Add the application to the startup registry key
                RegistryKey key = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);
                key.SetValue(appName, appPath);
                Console.WriteLine("Application added to startup.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to add application to startup: {ex.Message}");
            }
        }
    }

    // Method to check if the application is in startup
    static bool IsInStartup(string appName)
    {
        RegistryKey key = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run");
        string[] valueNames = key.GetValueNames();
        return Array.Exists(valueNames, name => name.Equals(appName, StringComparison.OrdinalIgnoreCase));
    }
}
