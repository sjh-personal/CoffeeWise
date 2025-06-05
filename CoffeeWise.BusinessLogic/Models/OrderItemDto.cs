namespace CoffeeWise.BusinessLogic.Models;

public record OrderItemDto(Guid ConsumerPersonId, string Description, decimal Price);
