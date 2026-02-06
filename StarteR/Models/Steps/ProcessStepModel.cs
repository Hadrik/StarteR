using System.Diagnostics;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace StarteR.Models.Steps;

public class ProcessStepModel : StepModelBase
{
    [JsonIgnore] public override StepType Type => StepType.Process;
    [JsonIgnore] public override string DisplayName => "Process";
    
    public string FilePath { get; set; } = string.Empty;
    public string Arguments { get; set; } = string.Empty;
    public string WorkingDirectory { get; set; } = string.Empty;
    
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