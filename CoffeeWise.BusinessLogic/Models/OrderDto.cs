namespace CoffeeWise.BusinessLogic.Models;

public record OrderDto(Guid OrderId, Guid PayerPersonId, DateTime Date, List<OrderItemDto> Items);
