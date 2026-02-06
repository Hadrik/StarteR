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

            try
            {
                if (step.WaitForCompletion)
                {
                    await step.Run();
                }
                else
                {
                    _ = step.Run();
                }
            }
            catch (Exception)
            {
                // ignored
            }
        }
    }
}