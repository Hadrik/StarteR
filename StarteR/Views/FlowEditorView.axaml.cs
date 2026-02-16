using System;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using FluentAvalonia.UI.Controls;

namespace StarteR.Views;

public partial class FlowEditorView : UserControl
{
    private readonly Symbol?[] _availableIcons = new Symbol?[] {null}.Concat<Symbol?>(Enum.GetValues<Symbol>().Where(e =>
    {
        // Exclude icons with `ObsoleteAttribute`
        var memberInfo = typeof(Symbol).GetMember(e.ToString()).FirstOrDefault();
        return memberInfo == null || !Attribute.IsDefined(memberInfo, typeof(ObsoleteAttribute));
    }).Cast<Symbol?>()).ToArray();
    
    public FlowEditorView()
    {
        InitializeComponent();
        IconItemsRepeater.ItemsSource = _availableIcons;
        IconScrollViewer.PointerWheelChanged += (sender, e) =>
        {
            if (e.Delta.Y > 0)
            {
                IconScrollViewer.PageLeft();
            }
            else
            {
                IconScrollViewer.PageRight();
            }
        };
    }

    private void StartNameEdit(object? sender, TappedEventArgs e)
    {
        NameEditPanelView.IsVisible = false;
        NameEditPanelEdit.IsVisible = true;
        NameEditTextBox.Focus();
    }
    
    private void EndNameEdit(object? sender, RoutedEventArgs e)
    {
        NameEditPanelView.IsVisible = true;
        NameEditPanelEdit.IsVisible = false;
    }
}