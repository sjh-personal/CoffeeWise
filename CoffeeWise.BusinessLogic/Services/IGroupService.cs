using CoffeeWise.BusinessLogic.Models;

namespace CoffeeWise.BusinessLogic.Services;

public interface IGroupService
{
    Task<GroupDto> GetGroupAsync(Guid groupId);
}