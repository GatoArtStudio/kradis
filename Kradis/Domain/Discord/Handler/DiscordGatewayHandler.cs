using Discord.WebSocket;
using Kradis.Domain.Discord.Core;

namespace Kradis.Domain.Discord.Handler;

public class DiscordGatewayHandler (
    DiscordSocketClient discordSocketClient,
    ILogger<DiscordGatewayHandler> logger
) : IDiscordHandler
{
    public Task StartAsync(CancellationToken cancellationToken)
    {
        discordSocketClient.Connected += OnGatewayConnectedAsync;
        discordSocketClient.Disconnected += OnGatewayDisconnectedAsync;
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        discordSocketClient.Connected -= OnGatewayConnectedAsync;
        discordSocketClient.Disconnected -= OnGatewayDisconnectedAsync;
        return Task.CompletedTask;
    }

    private Task OnGatewayConnectedAsync()
    {
        logger.LogInformation("Gateway connected.");
        return Task.CompletedTask;
    }

    private Task OnGatewayDisconnectedAsync(Exception exception)
    {
        logger.LogError(exception, "Gateway disconnected.");
        return Task.CompletedTask;
    }
}