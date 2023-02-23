using System;
using System.IO;
using System.Net.Http;
using IdentityModel.Client;
using RestSharp;

namespace SeedzDrive;

public sealed class WindowsFileSystemWatcher
{
    private static WindowsFileSystemWatcher? _instance;
    private static FileSystemWatcher? _fileSystemWatcher;

    private WindowsFileSystemWatcher()
    {
    }

    public static WindowsFileSystemWatcher GetInstance()
    {
        return _instance ??= new WindowsFileSystemWatcher();
    }

    public void Watcher()
    {
        if (string.IsNullOrWhiteSpace(Preferences.Default.Folder)) return;

        _fileSystemWatcher = new FileSystemWatcher
        {
            Path = Preferences.Default.Folder
        };

        _fileSystemWatcher.Filters.Add("*.txt");
        _fileSystemWatcher.Filters.Add("*.csv");
        _fileSystemWatcher.Filters.Add("*.xml");

        _fileSystemWatcher.NotifyFilter = NotifyFilters.Attributes | NotifyFilters.CreationTime |
                                          NotifyFilters.LastWrite | NotifyFilters.FileName | NotifyFilters.Size;
        _fileSystemWatcher.Changed += OnChanged;
        _fileSystemWatcher.Created += OnChanged;

        _fileSystemWatcher.EnableRaisingEvents = true;
    }

    public static void OnChanged(object source, FileSystemEventArgs e)
    {
        LogService.GetInstance().Write($"{DateTime.Now} - {Resource.Uploading} {e.FullPath}");

        if (GetFileSize(e.FullPath) <= 10000000) //Limit of 10MB
        {
            try
            {
                var client = new HttpClient();
                var token = client.RequestClientCredentialsTokenAsync(new ClientCredentialsTokenRequest
                {
                    Address = Preferences.Default.AuthUri,

                    ClientId = Preferences.Default.ClientId,
                    ClientSecret = Preferences.Default.Secret,
                    Scope = "client-credentials-server/tenantid"
                }).Result;

                var restClient = new RestClient(Preferences.Default.DriveAPIUri);
                var request = new RestRequest("", Method.Post)
                {
                    AlwaysMultipartFormData = true
                };

                request.AddHeader("Content-Type", "multipart/form-data");
                request.AddHeader("Authorization", $"Bearer {token.AccessToken}");
                request.AddFile("file", e.FullPath);

                var response = restClient.Execute(request);
                response.ThrowIfError(); //todo: verify

                LogService.GetInstance().Write($"{DateTime.Now} - {e.FullPath} {Resource.UploadComplete}");
            }
            catch (Exception exception)
            {
                LogService.GetInstance().Write($"{DateTime.Now} - {exception.Message}");
                IconState.GetInstance().Current = IconState.GetInstance().IconRed;
            }
        }
        else
        {
            LogService.GetInstance().Write($"{DateTime.Now} - {e.FullPath} {Resource.NotUploadedBecauseLarger}");
        }
    }

    private static long GetFileSize(string FilePath)
    {
        return File.Exists(FilePath) ? new FileInfo(FilePath).Length : 0;
    }
}