using CoffeeWise.BusinessLogic.Models;
using CoffeeWise.BusinessLogic.Services;
using Microsoft.AspNetCore.Mvc;

namespace CoffeeWise.Api.Controllers;

[ApiController]
[Route("api/groups")]
public class GroupsController(IGroupService groupService) : ControllerBase
{
    [HttpGet("{id}")]
    public async Task<ActionResult<GroupDto>> GetGroup(Guid id)
        => Ok(await groupService.GetGroupAsync(id));
}