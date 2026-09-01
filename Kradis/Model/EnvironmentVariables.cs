namespace Kradis.Model;

public record EnvironmentVariables(
    string DiscordApplicationToken,
    string? DefaultConnectionStringMySql
);