using System;
using System.IO;
using System.Text.Json;
using StarteR.Models;
using StarteR.StepManagement;

namespace StarteR;

public static class ConfigManager
{
    private static readonly string ConfigPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + @"\StarteR\config.json";
    
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        Converters = { new StepModelConverter() }
    };

    public static string Serialize(AppModel appModel)
    {
        return JsonSerializer.Serialize(appModel, JsonOptions);
    }

    public static AppModel? Deserialize(string json)
    {
        return JsonSerializer.Deserialize<AppModel>(json, JsonOptions);
    }
    
    public static string? Read()
    {
        try
        {
            return File.ReadAllText(ConfigPath);
        }
        catch (Exception)
        {
            return null;
        }
    }
    
    public static AppModel? Load()
    {
        var json = Read();
        if (json == null) return null;
        
        var model = Deserialize(json);
        model?.LoadedFromFile = true;
        return model;
    }
    
    public static void Save(AppModel appModel)
    {
        if (!Path.Exists(ConfigPath))
        {
            Directory.CreateDirectory(Path.GetDirectoryName(ConfigPath) ?? throw new InvalidOperationException());
        }
        
        using var stream = File.Create(ConfigPath);
        using var writer = new StreamWriter(stream);
        writer.Write(Serialize(appModel));
    }
}