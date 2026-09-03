using System.Collections.ObjectModel;
using Kradis.Core.Pattern;
using Kradis.Domain.Discord.Repository.Model;
using Medo;

namespace Kradis.Domain.Discord.Core;

public interface IDiscordGuildRepository
{
    Task<Result<GuildModel, string>> SaveAsync(GuildModel guild, CancellationToken cancellationToken);
    Task<Result<GuildModel, string>> UpdateAsync(GuildModel guild, CancellationToken cancellationToken);
    Task<Result<GuildModel, string>> GetAsync(Uuid7 guildUuid, CancellationToken cancellationToken);
    Task<Result<GuildModel, string>> DeleteAsync(Uuid7 guildId, CancellationToken cancellationToken);
    Task<Result<Uuid7, string>> GetUuidOfGuildAsync(ulong discordGuildId, CancellationToken cancellationToken);
    Task<Result<Collection<GuildModel>, string>> GetAllAsync(CancellationToken cancellationToken);
}