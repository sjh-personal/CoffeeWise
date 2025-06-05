namespace CoffeeWise.BusinessLogic.Models;

public record SettleUpRequestDto(
    Guid FromPersonId,
    Guid ToPersonId,
    decimal Amount
);