using CoffeeWise.BusinessLogic.Models;
using CoffeeWise.Data;
using CoffeeWise.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace CoffeeWise.BusinessLogic.Services.Implementations;

public class PresenceService(CoffeeWiseDbContext db) : IPresenceService
{
    public async Task MarkPresenceAsync(Guid groupId, Guid personId, bool isPresent)
    {
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var presence = await db.Presences
            .Where(p => p.PersonId == personId)
            .Where(p => p.Date == today)
            .FirstOrDefaultAsync();

        if (presence is not null)
        {
            presence.IsPresent = isPresent;
        }
        else
        {
            db.Presences.Add(new Presence
            {
                Id = Guid.NewGuid(),
                PersonId = personId,
                Date = today,
                IsPresent = isPresent
            });
        }

        await db.SaveChangesAsync();
    }

    public async Task<List<PresenceDto>> GetPresenceForDateAsync(Guid groupId, DateOnly date)
    {
        var memberIds = await db.GroupMembers
            .Where(gm => gm.GroupId == groupId)
            .Select(gm => gm.PersonId)
            .ToListAsync();

        var presences = await db.Presences
            .Where(p => p.Date == date)
            .Where(p => memberIds.Contains(p.PersonId))
            .ToListAsync();

        return presences
            .Select(p => new PresenceDto(p.PersonId, p.Date, p.IsPresent))
            .ToList();
    }

    public async Task<List<Guid>> GetPresentGroupMemberIdsAsync(Guid groupId, DateOnly date)
    {
        var memberIds = await db.GroupMembers
            .Where(gm => gm.GroupId == groupId)
            .Select(gm => gm.PersonId)
            .ToListAsync();

        return await db.Presences
            .Where(p => p.Date == date)
            .Where(p => p.IsPresent)
            .Where(p => memberIds.Contains(p.PersonId))
            .Select(p => p.PersonId)
            .ToListAsync();
    }
}
