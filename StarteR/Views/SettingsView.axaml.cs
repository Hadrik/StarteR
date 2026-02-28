using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Markup.Xaml;
using StarteR.Helpers;

namespace StarteR.Views;

public partial class SettingsView : UserControl
{
    public SettingsView()
    {
        InitializeComponent();
        SmoothScrollHelper.For(ContentScrollViewer, Orientation.Vertical);
    }
}