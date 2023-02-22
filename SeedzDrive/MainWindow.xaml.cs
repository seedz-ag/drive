using System;
using System.IO;
using System.Windows;
using System.Windows.Forms;

namespace SeedzDrive;

/// <summary>
///     Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    private static IconState _iconState;

    public MainWindow()
    {
        _iconState = IconState.GetInstance();
        InitializeComponent();
    }

    protected override void OnInitialized(EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(Preferences.Default.ClientId) ||
            string.IsNullOrWhiteSpace(Preferences.Default.Secret) ||
            string.IsNullOrWhiteSpace(Preferences.Default.Folder))
        {
            _iconState.Current = _iconState.IconGrey;
            OpenConfiguration(this);
        }
        else
        {
            ClientIdTxb.Text = Preferences.Default.ClientId;
            SecretTxb.Text = Preferences.Default.Secret;
            FolderTxb.Text = Preferences.Default.Folder;

            Visibility = Visibility.Hidden;
            WindowState = WindowState.Minimized;

            _iconState.Current = _iconState.Icon;
            IconState.GetInstance().ShowIcon(Preferences.Default.Folder + Resource.NotifyIconBaloomTextSync,
                Resource.NotifyIconBaloomTitle, ToolTipIcon.Info);
        }

        base.OnInitialized(e);
    }

    private void SelectFolderBtn_OnClick(object sender, RoutedEventArgs e)
    {
        using var dialog = new FolderBrowserDialog();

        dialog.Description = Resource.ConfigurationWindowSelectFolderDialogTitle;
        dialog.UseDescriptionForTitle = true;
        dialog.SelectedPath = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory) +
                              Path.DirectorySeparatorChar;
        dialog.ShowNewFolderButton = true;

        FolderTxb.Text = dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK
            ? dialog.SelectedPath
            : Preferences.Default.Folder;
    }

    private void SaveBtn_OnClick(object sender, RoutedEventArgs e)
    {
        Preferences.Default.ClientId = ClientIdTxb.Text;
        Preferences.Default.Secret = SecretTxb.Text;
        Preferences.Default.Folder = FolderTxb.Text;

        Preferences.Default.Save();

        WindowsFileSystemWatcher.GetInstance().Watcher();

        var iconState = IconState.GetInstance();

        if (string.IsNullOrWhiteSpace(Preferences.Default.ClientId) ||
            string.IsNullOrWhiteSpace(Preferences.Default.Secret) ||
            string.IsNullOrWhiteSpace(Preferences.Default.Folder))
        {
            iconState.Current = iconState.IconGrey;
        }
        else
        {
            iconState.Current = iconState.Icon;
            IconState.GetInstance().ShowIcon(Preferences.Default.Folder + Resource.NotifyIconBaloomTextSync, Resource.ConfigurationWindowSavedMessage, ToolTipIcon.Info);
            Hide();
        }
    }

    protected override void OnDeactivated(EventArgs e)
    {
        if (Visibility != Visibility.Hidden && WindowState == WindowState.Minimized)
        {
            Visibility = Visibility.Hidden;

            IconState.GetInstance().ShowIcon(Preferences.Default.Folder + Resource.NotifyIconBaloomTextSync,
                Resource.NotifyIconBaloomTitle, ToolTipIcon.Info);
        }

        base.OnDeactivated(e);
    }

    private static void OpenConfiguration(MainWindow mainWindow)
    {
        mainWindow.Icon = IconState.GetInstance().Current.ToImageSource();
        mainWindow.Visibility = Visibility.Visible;
        mainWindow.WindowState = WindowState.Normal;
    }
}