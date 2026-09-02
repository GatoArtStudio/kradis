using Discord;
using Discord.WebSocket;
using Kradis.Domain.Discord.Core;
using Serilog;
using Serilog.Events;

namespace Kradis.Domain.Discord.Handler;

public class DiscordLogHandler (
    DiscordSocketClient discordSocketClient,
    ILogger<DiscordLogHandler> logger
) : IDiscordHandler
{
    public Task StartAsync(CancellationToken cancellationToken)
    {
        logger.LogInformation("Starting Discord log handler.");
        discordSocketClient.Log += OnReadyAsync;
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        logger.LogInformation("Stopping Discord log handler.");
        discordSocketClient.Log -= OnReadyAsync;
        return Task.CompletedTask;
    }

    private Task OnReadyAsync(LogMessage logMessage)
    {
        var severity = logMessage.Severity switch
        {
            LogSeverity.Critical => LogEventLevel.Fatal,
            LogSeverity.Error => LogEventLevel.Error,
            LogSeverity.Warning => LogEventLevel.Warning,
            LogSeverity.Info => LogEventLevel.Information,
            LogSeverity.Verbose => LogEventLevel.Verbose,
            LogSeverity.Debug => LogEventLevel.Debug,
            _ => LogEventLevel.Information
        };
        Log.Write(severity, logMessage.Exception, "[{Source}] {Message}", logMessage.Source, logMessage.Message);
        return Task.CompletedTask;
    }
}