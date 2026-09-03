using System.Collections.ObjectModel;
using Kradis.Core.Pattern;
using Kradis.Domain.Discord.Core;
using Kradis.Domain.Discord.Repository.DbContext;
using Kradis.Domain.Discord.Repository.Model;
using Medo;
using Microsoft.EntityFrameworkCore;

namespace Kradis.Domain.Discord.Repository.Adapter;

public class MySqlDiscordGuildRepository : IDiscordGuildRepository
{
    private readonly MySqlGuildDbContext _context;
    
    public MySqlDiscordGuildRepository(MySqlGuildDbContext context)
    {
        _context = context;
    }

    public async Task<Result<GuildModel, string>> SaveAsync(GuildModel guild, CancellationToken cancellationToken)
    {
        try
        {
            var value = _context.Guild.Add(guild);
            await _context.SaveChangesAsync(cancellationToken);
            return Result<GuildModel, string>.Ok(value.Entity);
        }
        catch (Exception e)
        {
            return Result<GuildModel, string>.Fail(e.Message);
        }
    }

    public async Task<Result<GuildModel, string>> UpdateAsync(GuildModel guild, CancellationToken cancellationToken)
    {
        try
        {
            var entity = _context.Guild.Update(guild);
            await _context.SaveChangesAsync(cancellationToken);
            return Result<GuildModel, string>.Ok(entity.Entity);
        }
        catch (Exception e)
        {
            return Result<GuildModel, string>.Fail(e.Message);
        }
    }

    public async Task<Result<GuildModel, string>> GetAsync(Uuid7 guildUuid, CancellationToken cancellationToken)
    {
        var entityEntry = await _context.Guild
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == guildUuid, cancellationToken);
        if (entityEntry is null)
            return Result<GuildModel, string>.Fail("Guild not found");
        
        return Result<GuildModel, string>.Ok(entityEntry);
    }

    public async Task<Result<GuildModel, string>> DeleteAsync(Uuid7 guildUuid, CancellationToken cancellationToken)
    {
        var entityEntry = await GetAsync(guildUuid, cancellationToken);
        if (entityEntry.IsFailure)
            return Result<GuildModel, string>.Fail("Guild not found");

        try
        {
            var guildDeleted = _context.Guild.Remove(entityEntry.Value);
            await _context.SaveChangesAsync(cancellationToken);
            return Result<GuildModel, string>.Ok(guildDeleted.Entity);
        }
        catch (Exception e)
        {
            return Result<GuildModel, string>.Fail(e.Message);
        }
    }

    public async Task<Result<Uuid7, string>> GetUuidOfGuildAsync(ulong discordGuildId, CancellationToken cancellationToken)
    {
        var entityEntry = await _context.Guild
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.GuildId == discordGuildId, cancellationToken);
        
        if (entityEntry is null)
            return Result<Uuid7, string>.Fail("Guild not found");

        return Result<Uuid7, string>.Ok(entityEntry.Id);
    }

    public async Task<Result<Collection<GuildModel>, string>> GetAllAsync(CancellationToken cancellationToken)
    {
        var data = await _context.Guild
            .AsNoTracking()
            .ToListAsync(cancellationToken);
        
        return Result<Collection<GuildModel>, string>.Ok(new Collection<GuildModel>(data));
    }
}