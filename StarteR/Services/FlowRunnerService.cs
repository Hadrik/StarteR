using System;
using System.Threading.Tasks;
using StarteR.Models;

namespace StarteR.Services;

public class FlowRunnerService
{
    public async Task RunAsync(FlowModel model)
    {
        foreach (var step in model.Steps) // TODO: Fix
        {
            if (!step.IsEnabled) continue;

            step.IsRunning = true;
            try
            {
                if (step.WaitForCompletion)
                {
                    step.ExecuteAsync().Start();
                }
                else
                {
                    await step.ExecuteAsync();
                }
            }
            finally
            {
                step.IsRunning = false;
            }
        }
    }
}