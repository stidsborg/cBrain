using Microsoft.AspNetCore.Mvc;

namespace cBrain.Flows.Tickering;

[ApiController]
[Route("[controller]")]
public class TickeringController(TickFlows tickFlows) : Controller
{
    [HttpPost]
    public async Task<ActionResult> Post(string tickFlowId)
    {
        await tickFlows.Schedule(tickFlowId, tickFlowId);
        return Ok();
    }
}