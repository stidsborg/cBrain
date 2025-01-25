using cBrain.Flows.Ordering.MessageDriven;
using Cleipnir.ResilientFunctions;
using Microsoft.AspNetCore.Mvc;

namespace cBrain.Flows.Ordering.Rpc;

[ApiController]
[Route("[controller]")]
public class OrderController(OrderFlows orderFlows, ILogger<MessageDrivenOrderController> logger) : Controller
{
    [HttpPost]
    public async Task<ActionResult> Post(Order order)
    {
        logger.LogInformation($"{order.OrderId.ToUpper()}: Order processing started");
        await orderFlows.Run(order.OrderId, order);
        logger.LogInformation($"{order.OrderId.ToUpper()}: Order processing completed");
        return Ok();
    }
}