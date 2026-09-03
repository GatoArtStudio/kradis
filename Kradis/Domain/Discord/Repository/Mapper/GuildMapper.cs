using Kradis.Domain.Discord.Model;
using Kradis.Domain.Discord.Repository.Model;
using Medo;

namespace Kradis.Domain.Discord.Repository.Mapper;

public class GuildMapper
{
    public static Guild? Map(GuildModel model)
    {
        var guildResult = Guild.Create(
            model.GuildId,
            model.AntiSpamChannelId);
        
        if (guildResult.IsSuccess)
            return guildResult.Value;

        return null;
    }

    public static GuildModel Map(Uuid7 uuid, Guild guild)
    {
        return new GuildModel()
        {
            Id = uuid,
            GuildId = guild.Id,
            AntiSpamChannelId = guild.AntiSpamChannelId
        };
    }
}