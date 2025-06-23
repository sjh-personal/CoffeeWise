using CoffeeWise.BusinessLogic.Models;
using CoffeeWise.BusinessLogic.Services;
using Microsoft.AspNetCore.Mvc;

namespace CoffeeWise.Api.Controllers;

[ApiController]
[Route("api/balances")]
public class BalancesController(IBalanceService balanceService) : ControllerBase
{
    [HttpGet("{groupId:guid}")]
    public async Task<ActionResult<List<BalanceSummaryDto>>> GetBalances(Guid groupId) =>
        Ok(await balanceService.GetBalancesAsync(groupId));

    [HttpGet("{groupId:guid}/next")]
    public async Task<ActionResult<PersonDto>> GetNextPayer(Guid groupId, [FromQuery] DateOnly? date)
    {
        var useDate = date ?? DateOnly.FromDateTime(DateTime.UtcNow);
        try
        {
            var next = await balanceService.GetNextPayerAsync(groupId, useDate);
            return Ok(next);
        }
        catch (InvalidOperationException)
        {
            return NoContent();
        }
    }

    [HttpGet("{groupId:guid}/pairwise")]
    public async Task<ActionResult<PairwiseBalanceDto>> GetPairwiseBalance(
        Guid groupId,
        [FromQuery] Guid personA,
        [FromQuery] Guid personB)
    {
        if (personA == personB) return BadRequest("Cannot compare same person.");
        return Ok(await balanceService.GetPairwiseBalanceAsync(groupId, personA, personB));
    }

    [HttpPost("{groupId:guid}/settle")]
    public async Task<IActionResult> SettleUp(Guid groupId, [FromBody] SettleUpRequestDto request)
    {
        if (request.FromPersonId == request.ToPersonId) return BadRequest("Cannot settle up with self.");

        await balanceService.SettleUpAsync(
            groupId,
            request.FromPersonId,
            request.ToPersonId,
            request.Amount);

        return Ok();
    }

    [HttpGet("{groupId:guid}/simplified-settlements")]
    public async Task<ActionResult<List<SettlementDto>>> GetSimplifiedSettlements(Guid groupId) =>
        Ok(await balanceService.GetSimplifiedSettlementsAsync(groupId));

    [HttpGet("{groupId:guid}/pairwise-positions")]
    public async Task<ActionResult<List<PairwiseBalanceDto>>> GetPairwisePositions(Guid groupId) =>
        Ok(await balanceService.GetAllPairwisePositionsAsync(groupId));
}
