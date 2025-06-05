using CoffeeWise.BusinessLogic.Models;

namespace CoffeeWise.BusinessLogic.Services;

public interface IPresenceService
{
    Task MarkPresenceAsync(Guid groupId, Guid personId, bool isPresent);
    Task<List<PresenceDto>> GetPresenceForDateAsync(Guid groupId, DateOnly date);
    Task<List<Guid>> GetPresentGroupMemberIdsAsync(Guid groupId, DateOnly date);
}