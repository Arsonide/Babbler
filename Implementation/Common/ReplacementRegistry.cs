using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text.Json;
using System.Threading.Tasks;

namespace Babbler.Implementation.Common;

public static class ReplacementRegistry
{
    private static Dictionary<string, string> _replacements = new Dictionary<string, string>();
    
    private static string ReplacementPath => Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? throw new InvalidOperationException(), "Replacements.json");
    
    public static void Initialize()
    {
#pragma warning disable CS4014
        Load();
#pragma warning restore CS4014
    }

    public static bool TryGetReplacement(string line, out string replacement)
    {
        return _replacements.TryGetValue(line, out replacement);
    }

    private static async Task Load()
    {
        string path = ReplacementPath;

        if (!File.Exists(path))
        {
            return;
        }
        
        using (FileStream stream = File.OpenRead(path))
        {
            _replacements = await JsonSerializer.DeserializeAsync<Dictionary<string, string>>(stream);
        }
    }
    
    private static async Task Save()
    {
        using (FileStream stream = File.Create(ReplacementPath))
        {
            await JsonSerializer.SerializeAsync(stream, _replacements);
            await stream.DisposeAsync();
        }
    }
}