namespace CoffeeWise.BusinessLogic.Models;

public record NetPositionDto(
    Guid PersonId,
    string Name,
    decimal NetBalance
);