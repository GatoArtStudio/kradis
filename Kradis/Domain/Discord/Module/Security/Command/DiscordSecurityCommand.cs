using Discord;
using Discord.Interactions;
using Kradis.Domain.Discord.Core;

namespace Kradis.Domain.Discord.Module.Security.Command;

[DefaultMemberPermissions(GuildPermission.Administrator)]
[Group("security", "It allows you to configure security options for the guild.")]
public class DiscordSecurityCommand : InteractionModuleBase<SocketInteractionContext>
{
    [CommandContextType(InteractionContextType.Guild, InteractionContextType.PrivateChannel)]
    [DefaultMemberPermissions(GuildPermission.Administrator)]
    [Group("setup", "Configurations for the security system.")]
    public class SetupCommands (
        IServiceScopeFactory scopeFactory
    ) : InteractionModuleBase<SocketInteractionContext>
    {
        [SlashCommand("init", "Initialize the server configurations to be able to use its other functions.")]
        public async Task Init()
        {
            await DeferAsync();
            
            await using var scope = scopeFactory.CreateAsyncScope();
            var discordGuildService = scope.ServiceProvider.GetRequiredService<IDiscordGuildService>();
            
            ulong guildId = Context.Guild.Id;

            var createdGuildResult = await discordGuildService.CreateAsync(guildId, null, CancellationToken.None);
            if (createdGuildResult.IsFailure)
            {
                await FollowupAsync($"Failed to create guild configuration: {createdGuildResult.Error}");
                return;
            }
            
            await FollowupAsync($"Successfully created guild configuration: {createdGuildResult.Value.Id}");
        }
        
        [SlashCommand("antispam-channel", "It allows you to configure the anti-spam channel.")]
        public async Task AntispamChannel(ITextChannel channel)
        {
            await DeferAsync();
            
            await using var scope = scopeFactory.CreateAsyncScope();
            var discordGuildService = scope.ServiceProvider.GetRequiredService<IDiscordGuildService>();
            
            ulong guildId = Context.Guild.Id;
            
            var guildResult = await discordGuildService.GetAsync(guildId, CancellationToken.None);
            if (guildResult.IsFailure)
            {
                await FollowupAsync($"Failed the configure anti-spam channel: {guildResult.Error}");
                return;
            }
            
            var guild = guildResult.Value;

            if (guild.AntiSpamChannelId is not null && guild.AntiSpamChannelId == channel.Id)
            {
                await FollowupAsync("This channel is already configured; no changes were applied.");
                return;
            }

            var updatedAntiSpamChannelResult = guild.SetAntiSpamChannelId(channel.Id);
            if (updatedAntiSpamChannelResult.IsFailure)
            {
                await FollowupAsync("Failed to set anti-spam channel ID");
                return;
            }

            var updateGuildResult = await discordGuildService.UpdateAsync(updatedAntiSpamChannelResult.Value, CancellationToken.None);
            if (updateGuildResult.IsFailure)
            {
                await FollowupAsync($"Failed to update guild configuration: {updateGuildResult.Error}");
                return;
            }
            
            await FollowupAsync($"Successfully configured anti-spam channel: {guildResult.Value.AntiSpamChannelId}");
        }
    }
    
    [CommandContextType(InteractionContextType.Guild, InteractionContextType.PrivateChannel)]
    [DefaultMemberPermissions(GuildPermission.Administrator)]
    [Group("unconfigure", "Remove the security system settings.")]
    public class UnconfigureCommands (
        IServiceScopeFactory scopeFactory
    ) : InteractionModuleBase<SocketInteractionContext>
    {
        [SlashCommand("antispam-channel", "It allows you to disable the anti-spam channel.")]
        public async Task AntiSpamChannel()
        {
            await DeferAsync();
            
            await using var scope = scopeFactory.CreateAsyncScope();
            var discordGuildService = scope.ServiceProvider.GetRequiredService<IDiscordGuildService>();
            
            ulong guildId = Context.Guild.Id;
            
            var guildResult = await discordGuildService.GetAsync(guildId, CancellationToken.None);
            if (guildResult.IsFailure)
            {
                await FollowupAsync($"Failed the unconfigure anti-spam channel: {guildResult.Error}");
                return;
            }
            
            var guild = guildResult.Value;

            if (guild.AntiSpamChannelId is null)
            {
                await FollowupAsync("The option you want to disable is already disabled.");
                return;
            }

            var oldAntiSpamChannel = guild.AntiSpamChannelId;

            var updatedAntiSpamChannelResult = guild.SetAntiSpamChannelId(null);
            if (updatedAntiSpamChannelResult.IsFailure)
            {
                await FollowupAsync("Failed to  unset anti-spam channel ID");
                return;
            }

            var updateGuildResult = await discordGuildService.UpdateAsync(updatedAntiSpamChannelResult.Value, CancellationToken.None);
            if (updateGuildResult.IsFailure)
            {
                await FollowupAsync($"Failed to update guild configuration: {updateGuildResult.Error}");
                return;
            }
            
            await FollowupAsync($"Successfully unconfigured anti-spam channel: {oldAntiSpamChannel}");
        }
    }
}