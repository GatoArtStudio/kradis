using Kradis.Core.Pattern;
using Kradis.Domain.Discord.Core;
using Kradis.Domain.Discord.Model;
using Microsoft.Extensions.Caching.Memory;

namespace Kradis.Domain.Discord.Module.Security.Service;

public class DiscordSecurityService (
    IMemoryCache cache,
    IDiscordGuildService discordGuildService,
    ILogger<DiscordSecurityService> logger
) : IDiscordSecurityService
{
    private const string KeyGuildChannelAntiSpam = "discord-guild-channel-antispam";
    
    public async Task<Result<ulong, string>> GetChannelAntiSpam(ulong guildId, CancellationToken cancellationToken = default)
    {
        var channelAntiSpamResult = await GetChannelAntiSpamFromCache(guildId, cancellationToken);
        if (channelAntiSpamResult.IsFailure)
        {
            return Result<ulong, string>.Fail(channelAntiSpamResult.Error);
        }
        
        return Result<ulong, string>.Ok(channelAntiSpamResult.Value);
    }

    public void RemoveChannelAntiSpamFromCache(ulong guildId)
    {
        string keyCache = GenerateKeyCache(guildId);
        cache.Remove(keyCache);
    }

    private async Task<Result<ulong, string>> GetChannelAntiSpamFromCache(ulong guildId, CancellationToken cancellationToken = default)
    {
        string keyCache = GenerateKeyCache(guildId);
        if (cache.TryGetValue(keyCache, out ulong channelId))
        {
            return Result<ulong, string>.Ok(channelId);
        }

        var guildResult = await discordGuildService.GetAsync(guildId, cancellationToken);
        if (guildResult.IsFailure)
        {
            return Result<ulong, string>.Fail(guildResult.Error);
        }

        Guild guild = guildResult.Value;
        var antiSpamChannelId = guild.AntiSpamChannelId;
        
        if (antiSpamChannelId is not ulong channelIdValue)
        {
            return Result<ulong, string>.Fail("The channel antispam was not found, is null.");
        }

        cache.Set(keyCache, channelIdValue, TimeSpan.FromHours(2));
        return Result<ulong, string>.Ok(channelIdValue);
    }

    private string GenerateKeyCache(ulong guildId)
    {
        return $"{KeyGuildChannelAntiSpam}:{guildId}";
    }
}