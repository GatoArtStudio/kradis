using Kradis.Domain.Discord.Repository.Model;
using Microsoft.EntityFrameworkCore;

namespace Kradis.Domain.Discord.Repository.DbContext;

public class MySqlGuildDbContext : Microsoft.EntityFrameworkCore.DbContext
{
    public DbSet<GuildModel> Guild => Set<GuildModel>();
    
    public MySqlGuildDbContext(DbContextOptions options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(
            typeof(MySqlGuildDbContext).Assembly);
        
        base.OnModelCreating(modelBuilder);
    }
}