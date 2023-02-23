using System;
using System.Windows;
using System.Windows.Forms;

namespace SeedzDrive;

/// <summary>
///     Interaction logic for LogsWindow.xaml
/// </summary>
public partial class LogsWindow : Window
{
    public LogsWindow()
    {
        InitializeComponent();
    }

    protected override void OnActivated(EventArgs e)
    {
        LogService.GetInstance().Show(this);

        base.OnActivated(e);
    }

    protected override void OnDeactivated(EventArgs e)
    {
        if (Visibility != Visibility.Hidden && WindowState == WindowState.Minimized)
        {
            Visibility = Visibility.Hidden;

            IconState.GetInstance().ShowIcon(Preferences.Default.Folder + Resource.NotifyIconBaloomTextSync, Resource.NotifyIconBaloomTitle, ToolTipIcon.Info);
        }

        base.OnDeactivated(e);
    }
}