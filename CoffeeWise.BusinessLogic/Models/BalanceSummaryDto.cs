namespace CoffeeWise.BusinessLogic.Models;

public record BalanceSummaryDto(
    Guid PersonId,
    string Name,
    string Email,
    decimal Paid,
    decimal Owes,
    decimal Balance
);
