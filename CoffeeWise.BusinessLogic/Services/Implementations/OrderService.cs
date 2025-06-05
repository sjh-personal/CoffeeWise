using CoffeeWise.BusinessLogic.Models;
using CoffeeWise.Data;
using CoffeeWise.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace CoffeeWise.BusinessLogic.Services.Implementations;

public class OrderService(CoffeeWiseDbContext db) : IOrderService
{
     public async Task<Guid> SubmitOrderAsync(Guid groupId, Guid payerPersonId, DateTime date, List<OrderItemDto> items)
    {
        var groupMembers = await db.GroupMembers
            .Where(gm => gm.GroupId == groupId)
            .ToDictionaryAsync(gm => gm.PersonId, gm => gm.Id);

        if (!groupMembers.TryGetValue(payerPersonId, out var payerGroupMemberId))
        {
            throw new Exception("Payer is not a group member");
        }
        
        var order = new Order
        {
            Id = Guid.NewGuid(),
            GroupId = groupId,
            PayerGroupMemberId = payerGroupMemberId,
            Date = date.Kind == DateTimeKind.Utc ? date : date.ToUniversalTime(),
            Items = items.Select(i => new OrderItem
            {
                Id = Guid.NewGuid(),
                GroupMemberId = groupMembers.TryGetValue(i.ConsumerPersonId, out var gmId)
                    ? gmId
                    : throw new Exception($"Person {i.ConsumerPersonId} is not a group member"),
                Description = i.Description,
                Price = i.Price
            }).ToList()
        };

        db.Orders.Add(order);
        await db.SaveChangesAsync();
        return order.Id;
    }

    public async Task<List<OrderDto>> GetOrdersForGroupAsync(Guid groupId)
    {
        var orders = await db.Orders
            .Where(o => o.GroupId == groupId)
            .Include(o => o.PayerGroupMember).ThenInclude(gm => gm.Person)
            .Include(o => o.Items).ThenInclude(i => i.GroupMember).ThenInclude(gm => gm.Person)
            .ToListAsync();

        return orders
            .Select(o => new OrderDto(
                o.Id,
                o.PayerGroupMember.Person.Id,
                o.Date, 
                o.Items.Select(i => new OrderItemDto(
                    i.GroupMember.Person.Id,
                    i.Description,
                    i.Price
                )).ToList()
            ))
            .OrderByDescending(o => o.Date)
            .ToList();
    }
}
