using AzWebAppFileHandler.Console.Interfaces;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

public class ConsoleHostedService : IHostedService
{
    private readonly ISessionHandler sessionHandler;
    private readonly IHostApplicationLifetime hostApplicationLifetime;
    private readonly ILogger<ConsoleHostedService> logger;

    public ConsoleHostedService(
        ISessionHandler sessionHandler,
        IHostApplicationLifetime hostApplicationLifetime,
        ILogger<ConsoleHostedService> logger)
    {
        this.sessionHandler = sessionHandler;
        this.hostApplicationLifetime = hostApplicationLifetime;
        this.logger = logger;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        await StartWorkAsync(cancellationToken);
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }

    private async Task StartWorkAsync(CancellationToken cancellationToken)
    {
        await Task.Factory.StartNew(async () =>
        {
            try
            {
                await DoWork(Environment.GetCommandLineArgs());
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Unhandled exception");
            }
            finally
            {
                hostApplicationLifetime.StopApplication();
            }
        }, cancellationToken);
    }

    private async Task DoWork(string[]? args)
    {
        await sessionHandler.StartSessionAsync();

        var operation = args?[1];

        var remoteDir = "/site/wwwroot/";
        var localDir = "BACKUP";

        switch (operation?.ToUpper())
        {
            case "DOWNLOAD":
                sessionHandler.DownloadFileFromSourceToDestination("Database.sqlite", remoteDir, localDir);
                sessionHandler.DownloadFileFromSourceToDestination("Identity.sqlite", remoteDir, localDir);
                break;
            
            case "UPLOAD":
                sessionHandler.UploadFileFromSourceToDestination("Database.sqlite", localDir, remoteDir);
                sessionHandler.UploadFileFromSourceToDestination("Identity.sqlite", localDir, remoteDir);
                break;
            
            default:
                break;
        }
    }
}
