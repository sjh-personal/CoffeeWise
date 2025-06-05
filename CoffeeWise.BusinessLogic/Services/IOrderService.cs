using CoffeeWise.BusinessLogic.Models;

namespace CoffeeWise.BusinessLogic.Services;

public interface IOrderService
{
    Task<Guid> SubmitOrderAsync(Guid groupId, Guid payerPersonId, DateOnly date, List<OrderItemDto> items);
    Task<List<OrderDto>> GetOrdersForGroupAsync(Guid groupId);
    Task<OrderDto> GetOrderAsync(Guid orderId);
}