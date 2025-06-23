using CoffeeWise.BusinessLogic.Models;
using CoffeeWise.BusinessLogic.Services;
using Microsoft.AspNetCore.Mvc;

namespace CoffeeWise.Api.Controllers;

[ApiController]
[Route("api/orders")]
public class OrdersController(IOrderService orderService) : ControllerBase
{
    [HttpPost("{groupId:guid}")]
    public async Task<ActionResult<Guid>> Submit(Guid groupId, [FromBody] SubmitOrderRequest request)
    {
        var id = await orderService.SubmitOrderAsync(
            groupId,
            request.PayerPersonId,
            request.Date,
            request.Items);

        return Ok(id);
    }
    
    [HttpGet("{groupId:guid}")]
    public async Task<ActionResult<List<OrderDto>>> GetAll(Guid groupId) =>
        Ok(await orderService.GetOrdersForGroupAsync(groupId));
}