using Kradis.Model;

namespace Kradis.Service;

public class EnvironmentService
{
    public EnvironmentVariables EnvironmentVariables { get; init; }

    public EnvironmentService()
    {
        DotNetEnv.Env.Load();

        string discordApplicationToken = Environment.GetEnvironmentVariable("DISCORD_APPLICATION_TOKEN") ?? throw new InvalidOperationException("DISCORD_APPLICATION_TOKEN is not set");
        string? defaultConnectionStringMySql = Environment.GetEnvironmentVariable("DEFAULT_CONNECTION_STRING_MYSQL");

        EnvironmentVariables = new EnvironmentVariables(
            DiscordApplicationToken: discordApplicationToken,
            DefaultConnectionStringMySql: defaultConnectionStringMySql);
    }
}