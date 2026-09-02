using System.Reflection;
using Discord.Interactions;
using Discord.WebSocket;
using Kradis.Domain.Discord.Core;

namespace Kradis.Domain.Discord.Handler;

public class DiscordInteractionHandler (
    DiscordSocketClient discordSocketClient,
    InteractionService interactionService,
    IServiceProvider serviceProvider,
    ILogger<DiscordInteractionHandler> logger
) : IDiscordHandler
{
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        logger.LogInformation("Starting Discord interaction handler.");
        discordSocketClient.InteractionCreated += OnInteractionCreatedAsync;
        
        logger.LogInformation("Adding interaction modules");
        await interactionService.AddModulesAsync(Assembly.GetEntryAssembly(), serviceProvider);
        logger.LogInformation("Added interaction modules");
        logger.LogInformation("Started Discord interaction handler.");
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        logger.LogInformation("Stopping Discord interaction handler.");
        discordSocketClient.InteractionCreated -= OnInteractionCreatedAsync;
        return Task.CompletedTask;
    }

    private async Task OnInteractionCreatedAsync(SocketInteraction interaction)
    {
        try
        {
            SocketInteractionContext context = new(client: discordSocketClient, interaction: interaction);
            await interactionService.ExecuteCommandAsync(context: context, services: serviceProvider);
        }
        catch (Exception e)
        {
            logger.LogError(e, "An error occured when trying to start the interaction.");
        }
    }
}