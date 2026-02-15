using StarteR.Models;
using StarteR.Models.Steps;
using StarteR.ViewModels;

namespace StarteR.DesignViewModels;

public class FlowEditor : FlowEditorViewModel
{
    public FlowEditor() : base(
        new FlowModel
        {
            Name = "Sample Flow 1",
            Steps = [
                new ProcessStepModel
                {
                    ErrorMessage = "Test error message",
                },
                new WebRequestStepModel()
            ]
        },
        null!, model => { }) { }
}