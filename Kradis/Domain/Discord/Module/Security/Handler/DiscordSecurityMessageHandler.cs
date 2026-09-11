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
            ulong channelId = channel.Id;

            var channelAntiSpamResult = await securityService.GetChannelAntiSpam(guildId);
            if (channelAntiSpamResult.IsFailure)
            {
                logger.LogWarning(channelAntiSpamResult.Error);
                return;
            }

            ulong channelAntiSpam = channelAntiSpamResult.Value;
            if (channelAntiSpam == channelId)
            {
                logger.LogInformation("Channel antispam found.");
                channel.Guild.GetTextChannel(channelAntiSpam)?.SendMessageAsync("Fuiste bloqueado por enviar spam");
            }
        }
    }
}