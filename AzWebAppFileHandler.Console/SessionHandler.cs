using AzWebAppFileHandler.Console.Interfaces;
using Microsoft.Extensions.Logging;
using WinSCP;

namespace AzWebAppFileHandler.Console;

public class SessionHandler : ISessionHandler, IDisposable
{
    private bool disposedValue;
    private Session? currentSession;
    private readonly ILogger<SessionHandler> logger;

    public SessionHandler(ILogger<SessionHandler> logger)
    {
        this.logger = logger;
    }

    public async Task StartSessionAsync()
    {
        await Task.Factory.StartNew(() =>
        {
            SessionOptions sessionOptions = new SessionOptions
            {
                Protocol = Protocol.Ftp,
                HostName = Environment.GetEnvironmentVariable("AWAFH_HOSTNAME", EnvironmentVariableTarget.Machine),
                UserName = Environment.GetEnvironmentVariable("AWAFH_USERNAME", EnvironmentVariableTarget.Machine),
                Password = Environment.GetEnvironmentVariable("AWAFH_PASSWORD", EnvironmentVariableTarget.Machine),
                FtpSecure = FtpSecure.Implicit,
            };

            currentSession = new Session();
            currentSession.Open(sessionOptions);
        });
    }

    public Task CloseSessionAsync()
    {
        if (currentSession != null)
        {
            currentSession.Close();
        }

        return Task.CompletedTask;
    }

    public string[] ListFilesInDirectory(string path)
    {
        if (currentSession == null)
        {
            return Enumerable.Empty<string>().ToArray();
        }

        var files = currentSession.ListDirectory(path);
        var listOfFiles = new List<string>();
        foreach (var file in files.Files)
        {
            listOfFiles.Add(file.Name);
            System.Console.WriteLine(file.FullName);
        }

        return listOfFiles.ToArray();
    }

    public bool DownloadFileFromSourceToDestination(string fileName, string srcDir, string destDir)
    {
        if (currentSession == null)
        {
            return false;
        }

        string fullPath = Path.Combine(srcDir, fileName);
        if (currentSession.TryGetFileInfo(fullPath, out var remoteFileInfo))
        {
            logger.LogInformation($"Download file:{remoteFileInfo.Name} to destination:{destDir}");
            
            using var inputFile = currentSession.GetFile(fullPath);
            if (!Directory.Exists(destDir))
            {
                logger.LogInformation($"Creating directory:{destDir} in path:{Directory.GetCurrentDirectory()}");
                Directory.CreateDirectory(destDir);
            }
            using var outputFile = File.Create(Path.Combine(destDir, fileName));

            inputFile.CopyTo(outputFile);

            logger.LogInformation($"File:{remoteFileInfo.Name} downloaded!");

            return true;
        }

        logger.LogWarning($"File:{fileName} was not found in srcDir:{srcDir}!");

        return false;
    }

    public bool UploadFileFromSourceToDestination(string fileName, string srcDir, string destDir)
    {
        if (currentSession == null)
        {
            return false;
        }

        var currentDir = Directory.GetCurrentDirectory();
        var fullPath = Path.Combine(Path.Combine(currentDir, srcDir), fileName);
        if (File.Exists(fullPath))
        {
            logger.LogInformation($"Upload file:{fileName} to destination:{destDir}");

            var inputFile = currentSession.PutFileToDirectory(fullPath, destDir);

            return true;
        }

        return false;
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!disposedValue)
        {
            if (disposing)
            {
                if (currentSession != null && currentSession.Opened)
                {
                    currentSession.Close();
                }

                currentSession?.Dispose();
            }

            disposedValue = true;
        }
    }

    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}
