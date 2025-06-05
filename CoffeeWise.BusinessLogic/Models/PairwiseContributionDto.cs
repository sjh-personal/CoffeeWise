namespace CoffeeWise.BusinessLogic.Models;

public record PairwiseContributionDto(
    Guid PayerPersonId,
    Guid RecipientPersonId,
    decimal AmountPaidForRecipient
);