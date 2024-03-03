namespace AzWebAppFileHandler.Console.Interfaces;

public interface ISessionHandler
{
    Task StartSessionAsync();
    Task CloseSessionAsync();
    string[] ListFilesInDirectory(string path);
    bool DownloadFileFromSourceToDestination(string fileName, string srcDir, string destDir);
    bool UploadFileFromSourceToDestination(string fileName, string srcDir, string destDir);
}
