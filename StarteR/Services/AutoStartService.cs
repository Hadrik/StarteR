using System;
using Microsoft.Win32;

namespace StarteR.Services;

public class AutoStartService
{
    public const string AppName = "StarteR";
    
    public static bool AddToStartup(string?[] args)
    {
        return AddToStartup(AppName, System.Reflection.Assembly.GetExecutingAssembly().Location, args);
    }
    
    public static bool AddToStartup(string appName, string appPath, string?[] args)
    {
        try
        {
            var rk = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run", true);
            rk?.SetValue(appName, $"\"{appPath}\" {string.Join(" ", args)}", RegistryValueKind.String);
            return true;
        }
        catch
        {
            return false;
        }
    }

    public static bool RemoveFromStartup(string appName = AppName)
    {
        try
        {
            var rk = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run", true);
            rk?.DeleteValue(appName, true);
            return true;
        }
        catch
        {
            return false;
        }
    }
    
    public static bool IsInStartup(string appName = AppName, bool removeOldValue = true)
    {
        try
        {
            var rk = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run", false);
            var value = rk?.GetValue(appName)?.ToString();
            if (value == null) return false;
            
            if (removeOldValue && StripArgs(value) != System.Reflection.Assembly.GetExecutingAssembly().Location)
            {
                RemoveFromStartup(appName);
                return false;
            }
            
            return true;
        }
        catch
        {
            return false;
        }
    }
    
    private static string StripArgs(string value)
    {
        if (value.StartsWith('\"'))
        {
            var endQuoteIndex = value.IndexOf('"', 1);
            if (endQuoteIndex > 0)
            {
                return value[1..endQuoteIndex];
            }
        }
        else
        {
            var firstSpaceIndex = value.IndexOf(' ');
            if (firstSpaceIndex > 0)
            {
                return value[..firstSpaceIndex];
            }
        }

        return value;
    }
}