using Kradis.Model;

namespace Kradis.Service;

public class EnvironmentService
{
    public const string KeyDiscordApplicationToken = "DISCORD_APPLICATION_TOKEN";
    public const string KeyDefaultConnectionStringMySql = "DEFAULT_CONNECTION_STRING_MYSQL";
    
    public EnvironmentVariables EnvironmentVariables { get; init; }

    public EnvironmentService()
    {
        DotNetEnv.Env.Load();

        string discordApplicationToken = Environment.GetEnvironmentVariable(KeyDiscordApplicationToken) ?? throw new InvalidOperationException($"{KeyDiscordApplicationToken} is not set");
        string? defaultConnectionStringMySql = Environment.GetEnvironmentVariable(KeyDefaultConnectionStringMySql);

        EnvironmentVariables = new EnvironmentVariables(
            DiscordApplicationToken: discordApplicationToken,
            DefaultConnectionStringMySql: defaultConnectionStringMySql);
    }
}