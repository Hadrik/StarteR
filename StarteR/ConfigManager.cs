using System.IO;
using System.Text.Json;
using StarteR.Models;

namespace StarteR;

public class ConfigManager
{
    public static AppModel? Load(string path = "./config.json")
    {
        try
        {
            using var stream = File.OpenRead(path);
            return JsonSerializer.Deserialize<AppModel>(stream)!;
        }
        catch (FileNotFoundException)
        {
            return null;
        }
    }
    
    public static void Save(AppModel appModel, string path = "./config.json")
    {
        using var stream = File.Create(path);
        using var writer = new StreamWriter(stream);
        writer.Write(JsonSerializer.Serialize(appModel));
    }
}