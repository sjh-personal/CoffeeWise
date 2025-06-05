namespace CoffeeWise.BusinessLogic.Models;

public record OrderDto(Guid OrderId, Guid PayerPersonId, DateOnly Date, List<OrderItemDto> Items);
