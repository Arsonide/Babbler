using System;
using System.Diagnostics;
using System.Reflection;
using System.Text;
using BepInEx.Logging;

namespace Babbler.Implementation.Common;

public static class Utilities
{
    public const bool DEBUG_BUILD = false;

    public static readonly StringBuilder GlobalStringBuilder = new StringBuilder();
    public static readonly Random GlobalRandom = new Random();

    public static void Log(string message, LogLevel level = LogLevel.Info)
    {
        // Debug does not appear, presumably because it's for some functionality we don't have. We'll use it to filter based on DEBUG_BUILD instead.
        if (level == LogLevel.Debug)
        {
#pragma warning disable CS0162

            if (DEBUG_BUILD)
            {
                level = LogLevel.Info;
            }
            else
            {
                return;
            }
            
#pragma warning restore CS0162
        }
        
        BabblerPlugin.Log.Log(level, message);
    }
    
    public static string GetCallingMethodName()
    {
        StackTrace stackTrace = new StackTrace();
        StackFrame[] stackFrames = stackTrace.GetFrames();

        // 0 will be this utility method, the calling method is 1, and it wants the method that called it, which is 2.
        if (stackFrames == null || stackFrames.Length <= 2)
        {
            return string.Empty;
        }

        MethodBase callingMethod = stackFrames[2].GetMethod();

        if (callingMethod == null)
        {
            return string.Empty;
        }
        
        return callingMethod.Name;
    }

    public static float GetRandomFloat(float minimum, float maximum)
    {
        return minimum + (GlobalRandom.NextSingle() * (maximum - minimum));
    }
}