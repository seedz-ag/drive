using System.Windows;

namespace SeedzDrive;

/// <summary>
///     Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    protected override void OnStartup(StartupEventArgs e)
    {
        WindowsFileSystemWatcher.GetInstance().Watcher();

        base.OnStartup(e);
    }
}