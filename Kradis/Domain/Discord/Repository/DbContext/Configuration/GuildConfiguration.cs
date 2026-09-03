using Kradis.Domain.Discord.Repository.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Kradis.Domain.Discord.Repository.DbContext.Configuration;

public class GuildConfiguration : IEntityTypeConfiguration<GuildModel>
{
    public void Configure(EntityTypeBuilder<GuildModel> builder)
    {
        builder.ToTable("guilds");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .HasField("id")
            .IsRequired();

        builder.Property(x => x.GuildId)
            .HasField("guild_id")
            .IsRequired();
        
        builder.HasIndex(x => x.GuildId).IsUnique();

        builder.Property(x => x.AntiSpamChannelId)
            .HasField("antispam_channel_id");
    }
}