using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using Kradis.Domain.Discord.Core;
using Kradis.Domain.Discord.Hosted;
using Kradis.Helper;
using Kradis.Service;
using Serilog;

LoggerConfigurationHelper.Configure();

var builder = Host.CreateApplicationBuilder(args);

// Configure Logging
builder.Logging.ClearProviders();
builder.Logging.AddSerilog();

// Cache
// builder.Services.AddMemoryCache();

// Http
// builder.Services.AddHttpClient();

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

var handlersDiscordApplicationServices = typeof(Program).Assembly
    .GetTypes()
    .Where(type => !type.IsAbstract && typeof(IDiscordHandler).IsAssignableFrom(type));

handlersDiscordApplicationServices.ToList().ForEach(handler =>
{
    Log.Information("Registering discord application handler {handlerName}", handler.Name);
    builder.Services.AddSingleton(typeof(IDiscordHandler), handler);
});

builder.Services.AddHostedService<DiscordInitiatorHosted>();

// builder.Services.AddHostedService<Worker>();

var host = builder.Build();
host.Run();