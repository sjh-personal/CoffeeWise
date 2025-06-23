using CoffeeWise.BusinessLogic.Models;

namespace CoffeeWise.BusinessLogic.Services;

public interface IBalanceService
{
    Task<PersonDto> GetNextPayerAsync(Guid groupId, DateOnly date);
    Task<List<BalanceSummaryDto>> GetBalancesAsync(Guid groupId);
    Task<PairwiseBalanceDto> GetPairwiseBalanceAsync(Guid groupId, Guid personAId, Guid personBId);
    Task SettleUpAsync(Guid groupId, Guid fromPersonId, Guid toPersonId, decimal amount);
    Task<List<SettlementDto>> GetSimplifiedSettlementsAsync(Guid groupId);
    Task<List<PairwiseBalanceDto>> GetAllPairwisePositionsAsync(Guid groupId);
}