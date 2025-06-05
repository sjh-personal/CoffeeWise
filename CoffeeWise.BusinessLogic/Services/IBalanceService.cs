using CoffeeWise.BusinessLogic.Models;

namespace CoffeeWise.BusinessLogic.Services;

public interface IBalanceService
{
    Task<List<PairwiseContributionDto>> GetPairwiseContributionsAsync(Guid groupId);
    Task<List<NetPositionDto>> GetNetPositionsAsync(Guid groupId);
    Task<PersonDto> GetNextPayerAsync(Guid groupId, DateOnly date);
    Task<List<BalanceSummaryDto>> GetBalancesAsync(Guid groupId);
}