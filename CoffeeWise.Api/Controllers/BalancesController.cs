using CoffeeWise.BusinessLogic.Models;
using Microsoft.AspNetCore.Mvc;
using CoffeeWise.BusinessLogic.Services;

namespace CoffeeWise.Api.Controllers;

[ApiController]
[Route("api/groups/{groupId}/balances")]
public class BalancesController(IBalanceService balanceService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<List<BalanceSummaryDto>>> GetBalances(Guid groupId)
    {
        var list = await balanceService.GetBalancesAsync(groupId);
        return Ok(list);
    }
    
    [HttpGet("ledger")]
    public async Task<ActionResult<List<PairwiseContributionDto>>> GetLedger(Guid groupId)
    {
        var ledger = await balanceService.GetPairwiseContributionsAsync(groupId);
        return Ok(ledger);
    }
    
    [HttpGet("net-positions")]
    public async Task<ActionResult<List<NetPositionDto>>> GetNetPositions(Guid groupId)
    {
        var positions = await balanceService.GetNetPositionsAsync(groupId);
        return Ok(positions);
    }
    
    [HttpGet("next")]
    public async Task<ActionResult<PersonDto>> GetNextPayer(Guid groupId, [FromQuery] DateOnly? date)
    {
        var useDate = date ?? DateOnly.FromDateTime(DateTime.UtcNow);

        try
        {
            var next = await balanceService.GetNextPayerAsync(groupId, useDate);
            return Ok(next);
        }
        catch (Exception ex) when (ex.Message.Contains("No one is present"))
        {
            return NoContent();
        }
    }
}
