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
    
    [HttpGet("/api/groups/{groupId}/pairwise")]
    public async Task<ActionResult<PairwiseBalanceDto>> GetPairwiseBalance(Guid groupId, [FromQuery] Guid personA, [FromQuery] Guid personB)
    {
        if (personA == personB) return BadRequest("Cannot compare same person.");
        var result = await balanceService.GetPairwiseBalanceAsync(groupId, personA, personB);
        return Ok(result);
    }

    [HttpPost("/api/groups/{groupId}/settle")]
    public async Task<IActionResult> SettleUp(Guid groupId, [FromBody] SettleUpRequestDto request)
    {
        if (request.FromPersonId == request.ToPersonId) return BadRequest("Cannot settle up with self.");
        await balanceService.SettleUpAsync(groupId, request.FromPersonId, request.ToPersonId, request.Amount);
        return Ok();
    }
}
