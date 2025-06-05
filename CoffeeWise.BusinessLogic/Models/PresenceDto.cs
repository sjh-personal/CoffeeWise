namespace CoffeeWise.BusinessLogic.Models;

public record PresenceDto(Guid PersonId, DateOnly Date, bool IsPresent);