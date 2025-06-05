using CoffeeWise.BusinessLogic.Models;
using CoffeeWise.Data;
using Microsoft.EntityFrameworkCore;

namespace CoffeeWise.BusinessLogic.Services.Implementations;

public class BalanceService(CoffeeWiseDbContext db) : IBalanceService
{
    public async Task<List<PairwiseContributionDto>> GetPairwiseContributionsAsync(Guid groupId)
    {
        return await db.Orders
            .Where(o => o.GroupId == groupId)
            .SelectMany(o => o.Items.Select(i => new
            {
                PayerId = o.PayerGroupMember.Person.Id,
                RecipientId = i.GroupMember.Person.Id,
                Amount = i.Price
            }))
            .Where(x => x.PayerId != x.RecipientId) // exclude self-payments
            .GroupBy(x => new { x.PayerId, x.RecipientId })
            .Select(g => new PairwiseContributionDto(
                g.Key.PayerId,
                g.Key.RecipientId,
                g.Sum(x => x.Amount)
            ))
            .ToListAsync();
    }
    
    public async Task<List<NetPositionDto>> GetNetPositionsAsync(Guid groupId)
    {
        var pairwise = await GetPairwiseContributionsAsync(groupId);
        
        var members = await db.GroupMembers
            .Include(gm => gm.Person)
            .Where(gm => gm.GroupId == groupId)
            .ToListAsync();

        var paidForOthers = new Dictionary<Guid, decimal>();
        var paidByOthers = new Dictionary<Guid, decimal>();
        
        foreach (var member in members)
        {
            paidForOthers[member.Person.Id] = 0m;
            paidByOthers[member.Person.Id] = 0m;
        }
        
        foreach (var entry in pairwise)
        {
            paidForOthers[entry.PayerPersonId] += entry.AmountPaidForRecipient;
            paidByOthers[entry.RecipientPersonId] += entry.AmountPaidForRecipient;
        }
        
        var netPositions = members
            .Select(m =>
            {
                var pid = m.Person.Id;
                var netBalance = paidForOthers[pid] - paidByOthers[pid];
                return new NetPositionDto(pid, m.Person.Name, netBalance);
            })
            .OrderBy(np => np.NetBalance)
            .ToList();

        return netPositions;
    }

    public async Task<PersonDto> GetNextPayerAsync(Guid groupId, DateOnly date)
    {
        var positions = await GetNetPositionsAsync(groupId);

        var presentIds = await db.Presences
            .Where(p => p.IsPresent)
            .Where(p => p.Date == date)
            .Select(p => p.PersonId)
            .ToListAsync();

        var eligible = positions
            .Where(np => presentIds.Contains(np.PersonId))
            .OrderBy(np => np.NetBalance)
            .ToList();

        if (!eligible.Any())
            throw new Exception("No one is present today.");

        var next = eligible.First();

        var person = await db.Persons
            .Where(p => p.Id == next.PersonId)
            .Select(p => new PersonDto(p.Id, p.Name, p.Email))
            .FirstAsync();

        return person;
    }
    
    public async Task<List<BalanceSummaryDto>> GetBalancesAsync(Guid groupId)
    {
        var members = await db.GroupMembers
            .Include(gm => gm.Person)
            .Where(gm => gm.GroupId == groupId)
            .ToListAsync();

        var orders = await db.Orders
            .Where(o => o.GroupId == groupId)
            .Include(o => o.Items)
            .ToListAsync();

        var paidTotals = orders
            .GroupBy(o => o.PayerGroupMember.Person.Id)
            .ToDictionary(
                g => g.Key,
                g => g.SelectMany(o => o.Items).Sum(i => i.Price)
            );

        var owedTotals = orders
            .SelectMany(o => o.Items)
            .GroupBy(i => i.GroupMember.Person.Id)
            .ToDictionary(
                g => g.Key,
                g => g.Sum(i => i.Price)
            );

        return members.Select(m =>
        {
            var pid = m.Person.Id;
            var paid = paidTotals.GetValueOrDefault(pid, 0m);
            var owes = owedTotals.GetValueOrDefault(pid, 0m);
            return new BalanceSummaryDto(
                pid,
                m.Person.Name,
                m.Person.Email,
                paid,
                owes,
                paid - owes
            );
        })
        .OrderBy(b => b.Balance)
        .ToList();
    }
}
