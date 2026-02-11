using StarteR.Models;

namespace StarteR.Services;

public class SaveService
{
    private readonly AppModel _currentModel;

    public SaveService(AppModel model)
    {
        _currentModel = model;
    }
    
    public void Save()
    {
        ConfigManager.Save(_currentModel);
    }

    public bool HasUnsavedChanges()
    {
        return ConfigManager.Serialize(_currentModel) != ConfigManager.Read();
    }
}