namespace Kradis.Domain.Discord.Core;

public interface IDiscordHandler
{
    Task StartAsync(CancellationToken cancellationToken);
    
    Task StopAsync(CancellationToken cancellationToken);
}