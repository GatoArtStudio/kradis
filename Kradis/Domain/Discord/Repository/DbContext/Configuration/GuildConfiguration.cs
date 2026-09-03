using Kradis.Domain.Discord.Repository.Model;
using Medo;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Kradis.Domain.Discord.Repository.DbContext.Configuration;

public class GuildConfiguration : IEntityTypeConfiguration<GuildModel>
{
    public void Configure(EntityTypeBuilder<GuildModel> builder)
    {
        var converter = new ValueConverter<Uuid7, Guid>(
            id => id.ToGuid(),
            guid => Uuid7.FromGuid(guid));
        
        builder.ToTable("guilds");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .HasConversion(converter)
            .HasColumnType("binary(36)")
            .IsRequired();

        builder.Property(x => x.GuildId)
            .IsRequired();
        
        builder.HasIndex(x => x.GuildId).IsUnique();

        builder.Property(x => x.AntiSpamChannelId);
    }
}