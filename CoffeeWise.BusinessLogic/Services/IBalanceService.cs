using CoffeeWise.BusinessLogic.Models;

namespace CoffeeWise.BusinessLogic.Services;

public interface IBalanceService
{
    Task<PersonDto> GetNextPayerAsync(Guid groupId, DateOnly date);
    Task<List<BalanceSummaryDto>> GetBalancesAsync(Guid groupId);
}