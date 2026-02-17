using System;
using System.Diagnostics;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using StarteR.StepManagement;

namespace StarteR.Models.Steps;

[RegisterStep("process", "Run Process")]
public partial class ProcessStepModel : StepModelBase
{
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

        try
        {
            using var process = Process.Start(startInfo);
            if (process != null && WaitForCompletion)
            {
                await process.WaitForExitAsync();
            }
        }
        catch (Exception e)
        {
            throw new InvalidOperationException($"Failed to start process: {FilePath} {Arguments}\n{e.Message}", e);
        }
    }
}