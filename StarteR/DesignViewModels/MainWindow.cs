using StarteR.Models;
using StarteR.Models.Steps;
using StarteR.ViewModels;

namespace StarteR.DesignViewModels;

public class MainWindow : MainWindowViewModel
{
    public MainWindow() : base(new AppModel
        {
            Flows = [
                new FlowModel
                {
                    Name = "Sample Flow 1",
                    Steps = [
                        new ProcessStepModel
                        {
                            
                        }
                    ]
                },
                new FlowModel
                {
                    Name = "Sample Flow 2",
                }
            ]
        },
        null!) {}
}