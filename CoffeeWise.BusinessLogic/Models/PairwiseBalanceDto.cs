namespace CoffeeWise.BusinessLogic.Models;

public record PairwiseBalanceDto(
    Guid FromPersonId,
    string FromPersonName,
    Guid ToPersonId,
    string ToPersonName,
    decimal Amount 
);