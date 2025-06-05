using CoffeeWise.BusinessLogic.Models;
using CoffeeWise.BusinessLogic.Services;
using Microsoft.AspNetCore.Mvc;

namespace CoffeeWise.Api.Controllers;

[ApiController]
[Route("api/groups/{groupId}/orders")]
public class OrdersController(IOrderService orderService) : ControllerBase
{
    [HttpPost]
    public async Task<ActionResult<Guid>> Submit(Guid groupId, [FromBody] SubmitOrderRequest request)
    {
        var id = await orderService.SubmitOrderAsync(groupId, request.PayerPersonId, request.Date, request.Items);
        return Ok(id);
    }

    [HttpGet]
    public async Task<ActionResult<List<OrderDto>>> GetAll(Guid groupId) =>
        Ok(await orderService.GetOrdersForGroupAsync(groupId));
}
