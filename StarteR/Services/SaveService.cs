using StarteR.Models;

namespace StarteR.Services;

public class SaveService(AppModel model)
{
    public void Save()
    {
        ConfigManager.Save(model);
    }

    public bool HasUnsavedChanges()
    {
        return ConfigManager.Serialize(model) != ConfigManager.Read();
    }
}