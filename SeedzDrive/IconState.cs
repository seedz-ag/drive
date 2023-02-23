using System.Drawing;
using System.Windows.Interop;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using System.Windows;
using System.Windows.Forms;
using System.Diagnostics;
using System;
using Application = System.Windows.Application;

namespace SeedzDrive;

internal sealed class IconState
{
    private static IconState? _instance;
    private static Icon _current;
    private static NotifyIcon _notifyIcon;
    private static LogsWindow _logsWindow = new LogsWindow();
    private static MainWindow _mainWindow = new MainWindow();

    public Icon Icon = new("appicon.ico");
    public Icon IconGrey = new("appicon-grey.ico");
    public Icon IconRed = new("appicon-red.ico");

    private IconState()
    {
        _notifyIcon = new NotifyIcon
        {
            Icon = Current,
            Text = Resource.ApplicationName
        };

        _notifyIcon.DoubleClick += NotifyIcon_Click;
        _notifyIcon.ContextMenuStrip = new ContextMenuStrip();
        _notifyIcon.ContextMenuStrip.Items.Add(Resource.NotifyIconMenuConfiguration, null, OpenConfiguration);
        _notifyIcon.ContextMenuStrip.Items.Add(Resource.NotifyIconMenuOpenFolder, null, OpenFolder);
        _notifyIcon.ContextMenuStrip.Items.Add(Resource.NotifyIconMenuLogs, null, OpenLogs);
        _notifyIcon.ContextMenuStrip.Items.Add(Resource.NotifyIconMenuClose, null, OnClose);

        Current = IconGrey;
    }

    public Icon Current
    {
        get => _current;
        set
        {
            _current = value;
            _notifyIcon.Icon = value;
        }
    }

    public static IconState GetInstance()
    {
        return _instance ??= new IconState();
    }

    private static void OpenConfiguration()
    {
        _notifyIcon.Visible = false;

        _mainWindow.Icon = IconState.GetInstance().Current.ToImageSource();
        _mainWindow.Visibility = Visibility.Visible;
        _mainWindow.WindowState = WindowState.Normal;
    }

    private static void OpenConfiguration(object? sender, EventArgs e)
    {
        OpenConfiguration();
    }

    private static void OpenLogs(object? sender, EventArgs e)
    {
        _notifyIcon.Visible = false;

        _logsWindow.Icon = IconState.GetInstance().Current.ToImageSource();
        _logsWindow.Visibility = Visibility.Visible;
        _logsWindow.WindowState = WindowState.Normal;
    }

    private static void OpenFolder(object? sender, EventArgs e)
    {
        Process.Start(Environment.GetEnvironmentVariable("WINDIR") + @"\explorer.exe", Preferences.Default.Folder);
    }

    private static void OnClose(object? sender, EventArgs e)
    {
        Application.Current.Shutdown();
    }
    private static void NotifyIcon_Click(object? sender, EventArgs e)
    {
        OpenConfiguration();
    }

    public void ShowIcon(string ballonTipTex = null, string ballonTipTitle = null, ToolTipIcon toolTipIcon = ToolTipIcon.None)
    {
        _notifyIcon.Visible = true;

        _notifyIcon.BalloonTipText = ballonTipTex;
        _notifyIcon.BalloonTipTitle = ballonTipTitle;
        _notifyIcon.BalloonTipIcon = toolTipIcon;

        _notifyIcon.ShowBalloonTip(1000);
    }

    public void HideIcon()
    {
        _notifyIcon.Visible = false;
    }
}

internal static class Extensions
{
    public static ImageSource ToImageSource(this Icon icon)
    {
        ImageSource imageSource = Imaging.CreateBitmapSourceFromHIcon(
            icon.Handle,
            Int32Rect.Empty,
            BitmapSizeOptions.FromEmptyOptions());

        return imageSource;
    }
}