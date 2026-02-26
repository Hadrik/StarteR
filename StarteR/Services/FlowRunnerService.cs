using System;
using System.Threading.Tasks;
using StarteR.Models;

namespace StarteR.Services;

public class FlowRunnerService
{
    /// <summary>
    /// Runs steps of a flow sequentially. Task finishes once all steps have completed - even those which have <c>WaitForCompletion = false</c>.
    /// </summary>
    /// <returns>
    /// True if all steps completed successfully
    /// </returns>
    public async Task<bool> RunAsync(FlowModel model)
    {
        var allSuccessful = true;
        foreach (var step in model.Steps) // TODO: Fix
        {
            if (!step.IsEnabled) continue;
            try
            {
                if (step.WaitForCompletion)
                {
                    if (await step.Run() == false)
                    {
                        allSuccessful = false;
                    }
                }
                else
                {
                    _ = step.Run().ContinueWith(result =>
                    {
                        if (result.Result == false)
                        {
                            allSuccessful = false;
                        }
                    });
                }
            }
            catch (Exception)
            {
                // ignored
            }
        }
        return allSuccessful;
    }
}