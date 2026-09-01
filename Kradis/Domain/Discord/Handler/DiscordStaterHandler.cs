using Discord;
using Discord.WebSocket;
using Kradis.Domain.Discord.Core;
using Kradis.Service;

namespace Kradis.Domain.Discord.Handler;

public class DiscordStaterHandler (
    DiscordSocketClient discordSocketClient,
    EnvironmentService environmentService,
    ILogger<DiscordStaterHandler> logger
) : IDiscordHandler
{
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        logger.LogInformation("Starting Discord application");
        
        await discordSocketClient.LoginAsync(TokenType.Bot,
            environmentService.EnvironmentVariables.DiscordApplicationToken);

        await discordSocketClient.StartAsync();
        
        logger.LogInformation("Discord application started.");
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        await discordSocketClient.StopAsync();
        logger.LogInformation("Discord application stopped.");
    }
}