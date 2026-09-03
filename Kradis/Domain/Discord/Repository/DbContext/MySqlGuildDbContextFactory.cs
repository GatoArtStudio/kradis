using Kradis.Service;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Kradis.Domain.Discord.Repository.DbContext;

public class MySqlGuildDbContextFactory : IDesignTimeDbContextFactory<MySqlGuildDbContext>
{
    public MySqlGuildDbContext CreateDbContext(string[] args)
    {
        DotNetEnv.Env.Load();

        var connectionString = Environment.GetEnvironmentVariable(EnvironmentService.KeyDefaultConnectionStringMySql)
            ?? throw new Exception($"{EnvironmentService.KeyDefaultConnectionStringMySql} not set.");

        var optionsBuilder = new DbContextOptionsBuilder<MySqlGuildDbContext>();

        optionsBuilder.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));
        
        return new MySqlGuildDbContext(optionsBuilder.Options);
    }
}