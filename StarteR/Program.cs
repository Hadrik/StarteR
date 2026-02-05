using Avalonia;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StarteR;

sealed class Program
{
    // Initialization code. Don't use any Avalonia, third-party APIs or any
    // SynchronizationContext-reliant code before AppMain is called: things aren't initialized
    // yet and stuff might break.
    [STAThread]
    public static void Main(string[] args)
    {
        if (args.Contains("--run"))
        {
            List<Task> tasks = [];
            var config = ConfigManager.Load();
            if (config == null) return;
            var flowRunner = new Services.FlowRunnerService();
            tasks.AddRange(
                from flow
                in config.Flows
                where flow.IsEnabled
                select flowRunner.RunAsync(flow)
            );
            Task.WaitAll(tasks.ToArray());
            return;
        }
        
        BuildAvaloniaApp().StartWithClassicDesktopLifetime(args);
    }

    // Avalonia configuration, don't remove; also used by visual designer.
    public static AppBuilder BuildAvaloniaApp()
        => AppBuilder.Configure<App>()
            .UsePlatformDetect()
            .WithInterFont()
            .LogToTrace();
}