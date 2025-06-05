using CoffeeWise.BusinessLogic.Models;
using CoffeeWise.Data;
using CoffeeWise.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace CoffeeWise.BusinessLogic.Services.Implementations;

public class OrderService(CoffeeWiseDbContext db) : IOrderService
{
     public async Task<Guid> SubmitOrderAsync(Guid groupId, Guid payerPersonId, DateOnly date, List<OrderItemDto> items)
    {
        var groupMember = await db.GroupMembers
            .Where(gm => gm.GroupId == groupId)
            .Where(gm => gm.PersonId == payerPersonId)
            .FirstOrDefaultAsync();
        
        if (groupMember is null)
        {
            throw new Exception("Payer is not a group member");   
        }

        var order = new Order
        {
            Id = Guid.NewGuid(),
            GroupId = groupId,
            PayerGroupMemberId = groupMember.Id,
            Date = DateTime.SpecifyKind(date.ToDateTime(TimeOnly.MinValue), DateTimeKind.Utc),
            Items = items.Select(i => new OrderItem
            {
                Id = Guid.NewGuid(),
                GroupMemberId = i.ConsumerPersonId == payerPersonId
                    ? groupMember.Id
                    : db.GroupMembers
                        .Where(gm => gm.GroupId == groupId && gm.PersonId == i.ConsumerPersonId)
                        .Select(gm => gm.Id)
                        .First(), 
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
            .Include(o => o.PayerGroupMember).ThenInclude(gm => gm.Person)
            .Include(o => o.Items).ThenInclude(i => i.GroupMember).ThenInclude(gm => gm.Person)
            .Where(o => o.GroupId == groupId)
            .ToListAsync();

        return orders.Select(o =>
            new OrderDto(
                o.Id,
                o.PayerGroupMember.Person.Id,
                DateOnly.FromDateTime(o.Date),
                o.Items.Select(i => new OrderItemDto(
                    i.GroupMember.Person.Id,
                    i.Description,
                    i.Price
                )).ToList()
            )
        ).ToList();
    }

    public async Task<OrderDto> GetOrderAsync(Guid orderId)
    {
        var order = await db.Orders
            .Include(o => o.PayerGroupMember).ThenInclude(gm => gm.Person)
            .Include(o => o.Items).ThenInclude(i => i.GroupMember).ThenInclude(gm => gm.Person)
            .FirstOrDefaultAsync(o => o.Id == orderId);

        if (order is null)
        {
            throw new Exception($"Order not found: '{orderId}'");
        }

        return new OrderDto(
            order.Id,
            order.PayerGroupMember.Person.Id,
            DateOnly.FromDateTime(order.Date),
            order.Items.Select(i => new OrderItemDto(
                i.GroupMember.Person.Id,
                i.Description,
                i.Price
            )).ToList()
        );
    }
}
