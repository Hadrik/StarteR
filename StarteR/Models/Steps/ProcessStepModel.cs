using System.Diagnostics;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;

namespace StarteR.Models.Steps;

public partial class ProcessStepModel : StepModelBase
{
    [JsonIgnore] public override StepType Type => StepType.Process;
    [JsonIgnore] public override string DisplayName => "Process";
    
    [ObservableProperty]
    private string _filePath = string.Empty;
    
    [ObservableProperty]
    private string _arguments = string.Empty;
    
    [ObservableProperty]
    private string _workingDirectory = string.Empty;
    
    protected override async Task ExecuteAsync()
    {
        var startInfo = new ProcessStartInfo
        {
            FileName = FilePath,
            Arguments = Arguments,
            WorkingDirectory = WorkingDirectory,
            UseShellExecute = false
        };

        using var process = Process.Start(startInfo);
        if (process != null && WaitForCompletion)
        {
            await process.WaitForExitAsync();
        }
    }
}