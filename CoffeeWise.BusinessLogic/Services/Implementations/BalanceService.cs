using CoffeeWise.BusinessLogic.Models;
using CoffeeWise.Data;
using Microsoft.EntityFrameworkCore;

namespace CoffeeWise.BusinessLogic.Services.Implementations;

public class BalanceService(CoffeeWiseDbContext db) : IBalanceService
{
    public async Task<List<PairwiseContributionDto>> GetPairwiseContributionsAsync(Guid groupId)
    {
        var orders = await db.Orders
            .Include(o => o.PayerGroupMember).ThenInclude(gm => gm.Person)
            .Include(o => o.Items).ThenInclude(i => i.GroupMember).ThenInclude(gm => gm.Person)
            .Where(o => o.GroupId == groupId)
            .ToListAsync();
        
        var pairwise = new Dictionary<(Guid payer, Guid recipient), decimal>();

        foreach (var order in orders)
        {
            var payerId = order.PayerGroupMember.Person.Id;

            foreach (var item in order.Items)
            {
                var recipientId = item.GroupMember.Person.Id;
                var price = item.Price;
                
                if (recipientId == payerId) continue;

                var key = (payer: payerId, recipient: recipientId);
                pairwise.TryAdd(key, 0m);
                pairwise[key] += price;
            }
        }
        
        return pairwise
            .Select(kvp => new PairwiseContributionDto(
                PayerPersonId: kvp.Key.payer,
                RecipientPersonId: kvp.Key.recipient,
                AmountPaidForRecipient: kvp.Value
            ))
            .ToList();
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

        foreach (var gm in members)
        {
            paidForOthers[gm.Person.Id] = 0m;
            paidByOthers[gm.Person.Id] = 0m;
        }

        foreach (var entry in pairwise)
        {
            paidForOthers[entry.PayerPersonId] += entry.AmountPaidForRecipient;
            paidByOthers[entry.RecipientPersonId] += entry.AmountPaidForRecipient;
        }

        var positions = members
            .Select(gm =>
            {
                var pid = gm.Person.Id;
                var net = paidForOthers[pid] - paidByOthers[pid];
                return new NetPositionDto(pid, gm.Person.Name, net);
            })
            .OrderBy(np => np.NetBalance)
            .ToList();

        return positions;
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
