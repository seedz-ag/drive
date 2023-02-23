using System.Collections.Generic;

namespace SeedzDrive;

public sealed class LogService
{
    private static LogService? _instance;
    private static List<string> _lines;

    private LogService()
    {
        _lines = new List<string>();
    }

    public static LogService GetInstance()
    {
        return _instance ??= new LogService();
    }

    public void Write(string text)
    {
        if (_lines.Count > 100) _lines.RemoveAt(0);

        _lines.Add(text);
    }

    public string Read()
    {
        return string.Join("\n", _lines);
    }

    public void Show(LogsWindow logsWindow)
    {
        logsWindow.LogTxb.Text = Read();
    }
}