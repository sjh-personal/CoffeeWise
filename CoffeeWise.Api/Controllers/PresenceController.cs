using CoffeeWise.BusinessLogic.Models;
using CoffeeWise.BusinessLogic.Services;
using Microsoft.AspNetCore.Mvc;

namespace CoffeeWise.Api.Controllers;

[ApiController]
[Route("api/presences")]
public class PresenceController(IPresenceService presenceService) : ControllerBase
{
    [HttpPost("{groupId:guid}/people/{personId:guid}")]
    public async Task<IActionResult> MarkPresence(Guid groupId, Guid personId, [FromQuery] bool isPresent)
    {
        await presenceService.MarkPresenceAsync(groupId, personId, isPresent);
        return NoContent();
    }

    [HttpGet("{groupId:guid}")]
    public async Task<ActionResult<List<PresenceDto>>> GetPresences(Guid groupId, [FromQuery] DateOnly? date)
    {
        var useDate = date ?? DateOnly.FromDateTime(DateTime.UtcNow);
        var presences = await presenceService.GetPresenceForDateAsync(groupId, useDate);
        return Ok(presences);
    }
}