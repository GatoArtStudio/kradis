using Medo;

namespace Kradis.Domain.Discord.Repository.Model;

public class GuildModel
{
    public Uuid7 Id { get; set; }
    public ulong GuildId { get; set; }
    public ulong? AntiSpamChannelId { get; set; }
}