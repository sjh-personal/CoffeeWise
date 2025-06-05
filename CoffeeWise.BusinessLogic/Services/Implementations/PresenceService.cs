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
        return await db.Presences
            .Join(
                db.GroupMembers,
                presence => presence.PersonId,
                groupMember => groupMember.PersonId,
                (p, gm) => new { p, gm }
            )
            .Where(x => x.gm.GroupId == groupId)
            .Where(x =>  x.p.Date == date)
            .Select(x => new PresenceDto(x.p.PersonId, x.p.Date, x.p.IsPresent))
            .ToListAsync();
    }
}
