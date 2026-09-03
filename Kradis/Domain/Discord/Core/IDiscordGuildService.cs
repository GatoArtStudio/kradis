using Kradis.Core.Pattern;
using Kradis.Domain.Discord.Model;

namespace Kradis.Domain.Discord.Core;

public interface IDiscordGuildService
{
    Task<Result<Guild, string>> GetAsync(ulong discordGuildId, CancellationToken cancellationToken);
    Task<Result<Guild, string>> CreateAsync(ulong discordGuildId, ulong? discordAntiSpamChannelId,
        CancellationToken cancellationToken);
    Task<Result<Guild, string>> UpdateAsync(Guild guild, CancellationToken cancellationToken);
    Task<Result<Guild, string>> DeleteAsync(ulong discordGuildId, CancellationToken cancellationToken);
}