using System;
using System.Diagnostics;

namespace NauraaBot.Core.Utils;

// Will be useful if we want to switch to more advanced logging
public static class LogUtils
{
#if DEBUG
    public static void Debug(string message)
    {
        Console.WriteLine(message);
    }
#endif
    public static void Log(string message)
    {
        Console.WriteLine(message);
    }

    public static void Warn(string message)
    {
        Console.WriteLine(message);
    }

    public static void Error(string message)
    {
        Console.Error.WriteLine(message);
    }
}