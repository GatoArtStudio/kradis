using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using Kradis.Domain.Discord.Core;

namespace Kradis.Domain.Discord.Handler;

public class DiscordReadyHandler (
    DiscordSocketClient discordSocketClient,
    InteractionService interactionService,
    ILogger<DiscordReadyHandler> logger
) : IDiscordHandler
{
    public Task StartAsync(CancellationToken cancellationToken)
    {
        logger.LogInformation();
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }

    private async Task OnReadyAsync()
    {
        logger.LogInformation("Discord client is ready.");

        await interactionService.RegisterCommandsGloballyAsync();
        logger.LogInformation("Slash commands registered globally.");

        await discordSocketClient.SetStatusAsync(UserStatus.Online);
        await discordSocketClient.SetGameAsync("Gato Universe at the top.");
        
        logger.LogInformation("Discord bot connected as {User} {Id} | Guilds: {Guilds}",
            discordSocketClient.CurrentUser.Username,
            discordSocketClient.CurrentUser.Id,
            discordSocketClient.Guilds.Count);

        foreach (var socketGuild in discordSocketClient.Guilds)
        {
            logger.LogInformation("Connected in Guild: {GuildName}", socketGuild.Name);
        }
    }
}