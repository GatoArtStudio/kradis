using Discord.WebSocket;
using Kradis.Domain.Discord.Core;

namespace Kradis.Domain.Discord.Module.Security.Handler;

public class DiscordSecurityMessageHandler (
    DiscordSocketClient discordSocketClient,
    IDiscordSecurityService securityService,
    ILogger<DiscordSecurityMessageHandler> logger
) : IDiscordHandler
{
    public Task StartAsync(CancellationToken cancellationToken)
    {
        logger.LogInformation("Starting Discord security message handler.");
        discordSocketClient.MessageReceived += OnMessageReceivedAsync;
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        logger.LogInformation("Stopping Discord security message handler.");
        discordSocketClient.MessageReceived -= OnMessageReceivedAsync;
        return Task.CompletedTask;
    }

    private async Task OnMessageReceivedAsync(SocketMessage rawMessage)
    {
        if (rawMessage.Author.IsBot) return;

        if (rawMessage.Channel is SocketGuildChannel channel)
        {
            ulong guildId = channel.Guild.Id;
            string guildName = channel.Guild.Name;
            
            ulong channelId = channel.Id;
            string channelName = channel.Name;

            ulong userid = rawMessage.Author.Id;
            string userName = rawMessage.Author.Username;

            var channelAntiSpamResult = await securityService.GetChannelAntiSpam(guildId);
            if (channelAntiSpamResult.IsFailure)
            {
                logger.LogWarning(channelAntiSpamResult.Error);
                return;
            }

            ulong channelAntiSpam = channelAntiSpamResult.Value;
            if (channelAntiSpam == channelId)
            {
                logger.LogInformation($"Spam message blocked for user {userName}:{userid} on channel " + 
                    $"{channelName}:{channelId} of the {guildName}:{guildId}.");
                await channel.Guild.BanUserAsync(userId: userid, pruneSeconds: 60 * 20); // remove message before
            }
        }
    }
}