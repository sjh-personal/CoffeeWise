using CoffeeWise.BusinessLogic.Models;
using CoffeeWise.BusinessLogic.Services;
using Microsoft.AspNetCore.Mvc;

namespace CoffeeWise.Api.Controllers;

[ApiController]
[Route("api/groups/{groupId}/orders")]
public class OrdersController(IOrderService orderService) : ControllerBase
{
    [HttpPost]
    public async Task<ActionResult<Guid>> Submit(
        Guid groupId,
        [FromQuery] Guid payerPersonId,
        [FromQuery] DateOnly? date,
        [FromBody] List<OrderItemDto> items)
    {
        var useDate = date ?? DateOnly.FromDateTime(DateTime.UtcNow);
        var id = await orderService.SubmitOrderAsync(groupId, payerPersonId, useDate, items);
        return Ok(id);
    }

    [HttpGet]
    public async Task<ActionResult<List<OrderDto>>> GetAll(Guid groupId) =>
        Ok(await orderService.GetOrdersForGroupAsync(groupId));

    [HttpGet("{orderId}")]
    public async Task<ActionResult<OrderDto>> GetById(Guid groupId, Guid orderId) =>
        Ok(await orderService.GetOrderAsync(orderId));
}
