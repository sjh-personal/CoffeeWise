using CoffeeWise.BusinessLogic.Models;

namespace CoffeeWise.BusinessLogic.Services;

public interface IOrderService
{
    Task<Guid> SubmitOrderAsync(Guid groupId, Guid payerPersonId, DateTime date, List<OrderItemDto> items);
    Task<List<OrderDto>> GetOrdersForGroupAsync(Guid groupId);
}