using System.IO;
using System.Text.Json;
using StarteR.Models;

namespace StarteR;

public class ConfigManager
{
    public static string Serialize(AppModel appModel)
    {
        return JsonSerializer.Serialize(appModel);
    }

    public static AppModel? Deserialize(string json)
    {
        return JsonSerializer.Deserialize<AppModel>(json);
    }
    
    public static string? Read(string path = "./config.json")
    {
        try
        {
            return File.ReadAllText(path);
        }
        catch (FileNotFoundException)
        {
            return null;
        }
    }
    
    public static AppModel? Load(string path = "./config.json")
    {
        var json = Read(path);
        return json != null ? Deserialize(json) : null;
    }
    
    public static void Save(AppModel appModel, string path = "./config.json")
    {
        using var stream = File.Create(path);
        using var writer = new StreamWriter(stream);
        writer.Write(Serialize(appModel));
    }
}