using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using Kradis.Domain.Discord.Core;
using Kradis.Domain.Discord.Hosted;
using Kradis.Domain.Discord.Repository.Adapter;
using Kradis.Domain.Discord.Repository.DbContext;
using Kradis.Domain.Discord.Service;
using Kradis.Helper;
using Kradis.Service;
using Microsoft.EntityFrameworkCore;
using Serilog;

LoggerConfigurationHelper.Configure();

var builder = Host.CreateApplicationBuilder(args);

// Configure Logging
builder.Logging.ClearProviders();
builder.Logging.AddSerilog();

// Cache
builder.Services.AddMemoryCache();

// Http
// builder.Services.AddHttpClient();

// Databases
builder.Services.AddDbContext<MySqlGuildDbContext>((provider, options) =>
{
    var environmentService = provider.GetRequiredService<EnvironmentService>();

    if (environmentService.EnvironmentVariables.DefaultConnectionStringMySql is null)
    {
        throw new InvalidOperationException(
            $"{EnvironmentService.KeyDefaultConnectionStringMySql} environment variable is not set. " +
            "Please set it to a valid MySQL connection string.");
    }

    string connectionString = environmentService.EnvironmentVariables.DefaultConnectionStringMySql;

    options.UseMySql(
        connectionString,
        ServerVersion.AutoDetect(connectionString));
});

// Setup Environment
EnvironmentService environmentService = new EnvironmentService();
builder.Services.AddSingleton(environmentService);

// Setup Discord
builder.Services.AddSingleton(
    new DiscordSocketConfig()
    {
        GatewayIntents = GatewayIntents.All
    }
);

builder.Services.AddSingleton<DiscordSocketClient>();
builder.Services.AddSingleton(serviceProvider => 
    new InteractionService(
        serviceProvider.GetRequiredService<DiscordSocketClient>().Rest,
        new InteractionServiceConfig()
        {
            EnableAutocompleteHandlers = true
        }
    )
);

var discordApplicationHandlers = typeof(Program).Assembly
    .GetTypes()
    .Where(type => !type.IsAbstract && typeof(IDiscordHandler).IsAssignableFrom(type));

discordApplicationHandlers.ToList().ForEach(handler =>
{
    Log.Information("Registering discord application handler {handlerName}", handler.Name);
    builder.Services.AddSingleton(typeof(IDiscordHandler), handler);
});

builder.Services.AddHostedService<DiscordInitiatorHosted>();

builder.Services.AddScoped<IDiscordGuildRepository, MySqlDiscordGuildRepository>();
builder.Services.AddScoped<IDiscordGuildService, DiscordGuildService>();

// builder.Services.AddHostedService<Worker>();

var host = builder.Build();
host.Run();