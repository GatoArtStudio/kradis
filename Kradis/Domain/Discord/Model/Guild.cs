using Kradis.Core.Pattern;

namespace Kradis.Domain.Discord.Model;

public class Guild
{
    public ulong Id { get; init; }
    public ulong? AntiSpamChannelId { get; private set; }

    private Guild(ulong id, ulong? antiSpamChannelId)
    {
        Id = id;
        AntiSpamChannelId = antiSpamChannelId;
    }
    
    public static Result<Guild, string> Create(ulong id, ulong? antiSpamChannelId)
    {
        if (id <= 0)
            return Result<Guild, string>.Fail("The server ID cannot be 0 or negative.");

        if (antiSpamChannelId is not null && antiSpamChannelId <= 0)
            return Result<Guild, string>.Fail("The anti-spam channel ID cannot be 0 or negative.");

        var guild = new Guild(
            id,
            antiSpamChannelId
        );

        return Result<Guild, string>.Ok(guild);
    }

    public Result<Guild, string> SetAntiSpamChannelId(ulong? antiSpamChannelId)
    {
        if (antiSpamChannelId is not null && antiSpamChannelId <= 0)
            return Result<Guild, string>.Fail("The anti-spam channel ID cannot be 0 or negative.");

        AntiSpamChannelId = antiSpamChannelId;
        
        return Result<Guild, string>.Ok(this);
    }
}