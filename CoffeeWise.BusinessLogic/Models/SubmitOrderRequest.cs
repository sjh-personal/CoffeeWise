namespace CoffeeWise.BusinessLogic.Models;

public record SubmitOrderRequest(
    Guid PayerPersonId,
    DateTime Date,
    List<OrderItemDto> Items
);
