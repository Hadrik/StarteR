using System.Linq;
using System.Threading.Tasks;
using Microsoft.Toolkit.Uwp.Notifications;
using StarteR.Models;

namespace StarteR.Services;

public class HeadlessRunnerService(AppModel appModel, FlowRunnerService flowRunner, SaveService saveService)
{
    public async Task RunAsync()
    {
        if (!appModel.LoadedFromFile)
        {
            ShowToast("No configuration found. Please run the app at least once to create a config file.");
            return;
        }
        var tasks = (
            from flow
            in appModel.Flows
            where flow.IsEnabled
            select flowRunner.RunAsync(flow)
        ).ToArray();
        
        var results = await Task.WhenAll(tasks);
        var failCount = results.Count(b => !b);
        if (failCount > 0)
        {
            saveService.Save(); // Save error messages
            ShowToast($"Finished with {failCount} failed steps. Check the app for details.");
        }
    }

    private static void ShowToast(string message)
    {
        new ToastContentBuilder()
            .AddText("StarteR")
            .AddText(message)
            .Show();
    }
}