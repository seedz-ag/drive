using System.Globalization;
using System.Threading;
using System.Windows;

namespace SeedzDrive;

/// <summary>
///     Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    protected override void OnStartup(StartupEventArgs e)
    {
        Thread.CurrentThread.CurrentUICulture = CultureInfo.GetCultureInfoByIetfLanguageTag("pt");

        WindowsFileSystemWatcher.GetInstance().Watcher();

        base.OnStartup(e);
    }
}