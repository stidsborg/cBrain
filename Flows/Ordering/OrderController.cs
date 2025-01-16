using Cleipnir.ResilientFunctions;
using Microsoft.AspNetCore.Mvc;

namespace cBrain.Flows.Ordering;

[ApiController]
[Route("[controller]")]
public class OrderController(OrderFlows orderFlows, ILogger<OrderController> logger) : Controller
{
    [HttpPost]
    public async Task<ActionResult> Post(Order order)
    {
        logger.LogInformation($"{order.OrderId.ToUpper()}: Order processing started");
        await orderFlows.Schedule(order.OrderId, order).Completion();
        logger.LogInformation($"{order.OrderId.ToUpper()}: Order processing completed");
        return Ok();
    }
}