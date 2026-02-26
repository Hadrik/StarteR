using System.Collections.ObjectModel;
using System.Text.Json.Serialization;
using CommunityToolkit.Mvvm.ComponentModel;

namespace StarteR.Models;

public partial class AppModel : ObservableObject
{
    [ObservableProperty]
    private ObservableCollection<FlowModel> _flows = [];
    
    /// <summary>
    /// Whether the model was loaded from a config file or is a new instance was created.
    /// </summary>
    [JsonIgnore]
    public bool LoadedFromFile { get; set; }
}