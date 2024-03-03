using AzWebAppFileHandler.Console;
using AzWebAppFileHandler.Console.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

HostApplicationBuilder builder = Host.CreateApplicationBuilder(args);

builder.Environment.ContentRootPath = Directory.GetCurrentDirectory();
builder.Configuration.AddEnvironmentVariables("AWAFH_");
builder.Logging.AddFilter("Microsoft", LogLevel.Error);
builder.Logging.AddConsole();

builder.Services.AddScoped<ISessionHandler, SessionHandler>();
builder.Services.AddHostedService<ConsoleHostedService>();

using IHost host = builder.Build();

await host.RunAsync();
