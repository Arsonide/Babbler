using System.Diagnostics;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using UnityEngine;
using BepInEx.Configuration;
using BepInEx.Logging;
using Random = System.Random;

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

    public static int GetDeterministicInteger(int hash, int prime, int min, int max)
    {
        int range = max - min;
        int scaled = Mathf.Abs(hash * prime);
        return scaled % range + min;
    }
    
    public static float GetDeterministicFloat(int hash, int prime, float min, float max)
    {
        float normalizedHash = (Mathf.Abs(hash * prime) % 100000) / 100000f;
        return normalizedHash * (max - min) + min;
    }

    public static void EnforceMinMax(ref ConfigEntry<float> minimum, ref ConfigEntry<float> maximum)
    {
        float min = Mathf.Min(minimum.Value, maximum.Value);
        float max = Mathf.Max(minimum.Value, maximum.Value);
        minimum.Value = min;
        maximum.Value = max;
    }
    
    public static void EnforceMinMax(ref ConfigEntry<int> minimum, ref ConfigEntry<int> maximum)
    {
        int min = Mathf.Min(minimum.Value, maximum.Value);
        int max = Mathf.Max(minimum.Value, maximum.Value);
        minimum.Value = min;
        maximum.Value = max;
    }
    
    public static int GetDeterministicStringHash(string s)
    {
        // Fun fact, string.GetHashCode is deterministic within a game launch, but not ACROSS game launches. "Bob".GetHashCode will be different in one launch from another.
        // So we need to make this hashing method to be able to hash strings deterministically across program launches, or people's voices will change.
        // Source: https://andrewlock.net/why-is-string-gethashcode-different-each-time-i-run-my-program-in-net-core/
        unchecked
        {
            int hash1 = (5381 << 16) + 5381;
            int hash2 = hash1;

            for (int i = 0; i < s.Length; i += 2)
            {
                hash1 = ((hash1 << 5) + hash1) ^ s[i];
                
                if (i == s.Length - 1)
                {
                    break;
                }

                hash2 = ((hash2 << 5) + hash2) ^ s[i + 1];
            }

            return hash1 + (hash2 * 1566083941);
        }
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsHumanOutside(Human human)
    {
        if (human.inAirVent)
        {
            return false;
        }
        
        return human.isOnStreet || human.currentNode.isOutside || human.currentRoom.IsOutside();
    }
}