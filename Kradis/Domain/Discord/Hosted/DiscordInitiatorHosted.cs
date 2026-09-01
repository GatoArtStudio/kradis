using Kradis.Domain.Discord.Core;

namespace Kradis.Domain.Discord.Hosted;

public class DiscordInitiatorHosted (
    IEnumerable<IDiscordHandler> handlers,
    ILogger<DiscordInitiatorHosted> logger
) : IHostedService
{
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        foreach (var discordApplicationEventHandler in handlers)
        {
            string handlerName = discordApplicationEventHandler.GetType().Name;
            try
            {
                logger.LogInformation("{HandlerName} starting.", handlerName);
                await discordApplicationEventHandler.StartAsync(cancellationToken);
                logger.LogInformation("{HandlerName} started.", handlerName);
            }
            catch (Exception e)
            {
                logger.LogError(e, "{HandlerName} started error.", handlerName);
            }
        }
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        foreach (var discordApplicationEventHandler in handlers)
        {
            string handlerName = discordApplicationEventHandler.GetType().Name;
            try
            {
                logger.LogInformation("{HandlerName} stopping.", handlerName);
                await discordApplicationEventHandler.StopAsync(cancellationToken);
                logger.LogInformation("{HandlerName} stopped.", handlerName);
            }
            catch (Exception e)
            {
                logger.LogError(e, "{HandlerName} stopped error.", handlerName);
            }
        }
    }
}