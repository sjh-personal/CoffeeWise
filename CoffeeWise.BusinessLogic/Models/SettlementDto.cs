namespace CoffeeWise.BusinessLogic.Models;

public record SettlementDto(
    Guid FromPersonId,
    string FromName,
    Guid ToPersonId,
    string ToName,
    decimal Amount
);