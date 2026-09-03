using Kradis.Core.Pattern;
using Kradis.Domain.Discord.Core;
using Kradis.Domain.Discord.Model;
using Kradis.Domain.Discord.Repository.Mapper;
using Kradis.Domain.Discord.Repository.Model;
using Medo;
using Microsoft.Extensions.Caching.Memory;

namespace Kradis.Domain.Discord.Service;

public class DiscordGuildService (
    IMemoryCache cache,
    IDiscordGuildRepository repository
) : IDiscordGuildService
{
    private const string KeyGuildUuidCache = "discord-guild-uuid";
    
    public async Task<Result<Guild, string>> GetAsync(ulong discordGuildId, CancellationToken cancellationToken)
    {

        var guildUuid = await GetGuildUuidFromCacheOrDatabaseAsync(discordGuildId, cancellationToken);
        
        if (guildUuid.IsFailure)
            return Result<Guild, string>.Fail(guildUuid.Error);

        var guildModelResult = await repository.GetAsync(guildUuid.Value, cancellationToken);
        if (guildModelResult.IsFailure)
            return Result<Guild, string>.Fail(guildModelResult.Error);

        var guild = GuildMapper.Map(guildModelResult.Value);
        if (guild is null)
            return Result<Guild, string>.Fail("Error mapping the repository model.");
        
        return Result<Guild, string>.Ok(guild);
    }

    private async Task<Result<Uuid7, string>> GetGuildUuidFromCacheOrDatabaseAsync(ulong discordGuildId,
        CancellationToken cancellationToken)
    {
        var keyCache = $"{KeyGuildUuidCache}:{discordGuildId}";
        
        if (cache.TryGetValue(keyCache, out Uuid7 guildUuidCache))
        {
            return Result<Uuid7, string>.Ok(guildUuidCache);
        }
        else
        {
            var uuidResult = await repository.GetUuidOfGuildAsync(discordGuildId, cancellationToken);
            if (uuidResult.IsFailure)
                return Result<Uuid7, string>.Fail(uuidResult.Error);
                
            var guildUuid = uuidResult.Value;
            cache.Set(keyCache, guildUuid, TimeSpan.FromHours(5));
            return Result<Uuid7, string>.Ok(guildUuid);
        }
    }

    public async Task<Result<Guild, string>> CreateAsync(ulong discordGuildId, ulong? discordAntiSpamChannelId, CancellationToken cancellationToken)
    {
        var guildResult = await GetGuildUuidFromCacheOrDatabaseAsync(discordGuildId, cancellationToken);
        if (guildResult.IsSuccess)
        {
            return Result<Guild, string>.Fail("Guild already created.");
        }
        
        var guildCreateResult = Guild.Create(discordGuildId, discordAntiSpamChannelId);
        if (guildCreateResult.IsFailure)
            return Result<Guild, string>.Fail(guildCreateResult.Error);

        var guild = guildCreateResult.Value;
        GuildModel guildModel = GuildMapper.Map(new Uuid7(), guild);

        var savedResult = await repository.SaveAsync(guildModel, cancellationToken);
        if (savedResult.IsFailure)
            return Result<Guild, string>.Fail(savedResult.Error);

        return Result<Guild, string>.Ok(guild);
    }

    public async Task<Result<Guild, string>> UpdateAsync(Guild guild, CancellationToken cancellationToken)
    {
        var existGuildResult = await GetGuildUuidFromCacheOrDatabaseAsync(guild.Id, cancellationToken);
        if (existGuildResult.IsFailure)
            return Result<Guild, string>.Fail(existGuildResult.Error);
        
        var guildModel = GuildMapper.Map(existGuildResult.Value, guild);
        var savedResult = await repository.UpdateAsync(guildModel, cancellationToken);
        if (savedResult.IsFailure)
            return Result<Guild, string>.Fail(savedResult.Error);

        var guildMap = GuildMapper.Map(savedResult.Value);

        if (guildMap is null)
        {
            await repository.DeleteAsync(guildModel.Id, cancellationToken);
            return Result<Guild, string>.Fail("Error mapping the repository model.");
        }
        
        return Result<Guild, string>.Ok(guildMap);
    }

    public async Task<Result<Guild, string>> DeleteAsync(ulong discordGuildId, CancellationToken cancellationToken)
    {
        var guildUuidResult = await GetGuildUuidFromCacheOrDatabaseAsync(discordGuildId, cancellationToken);
        if (guildUuidResult.IsFailure)
            return Result<Guild, string>.Fail(guildUuidResult.Error);

        var guildDeletedResult = await repository.DeleteAsync(guildUuidResult.Value, cancellationToken);
        if (guildDeletedResult.IsFailure)
            return Result<Guild, string>.Fail(guildDeletedResult.Error);
        
        var guildMap = GuildMapper.Map(guildDeletedResult.Value);

        if (guildMap is null)
        {
            return Result<Guild, string>.Fail("The entity was deleted, but there was an error mapping the resulting model.");
        }
        
        return Result<Guild, string>.Ok(guildMap);
    }
}