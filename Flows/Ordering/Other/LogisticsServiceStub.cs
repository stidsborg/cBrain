using Rebus.Bus;
using Rebus.Handlers;

namespace cBrain.Flows.Ordering.Other;

public class LogisticsServiceStub(IBus bus) : IHandleMessages<ShipProducts>
{
    public async Task Handle(ShipProducts command)
    {
        await Task.Delay(1_000);
        await bus.SendLocal(
            new ProductsShipped(command.OrderId, TrackAndTraceNumber: Guid.NewGuid().ToString("N"))
        );
    }
}