using Kradis.Core.Pattern;

namespace Kradis.Domain.Discord.Core;

public interface IDiscordSecurityService
{
    Task<Result<ulong, string>> GetChannelAntiSpam(ulong guildId, CancellationToken cancellationToken = default);
    void RemoveChannelAntiSpamFromCache(ulong guildId);
}