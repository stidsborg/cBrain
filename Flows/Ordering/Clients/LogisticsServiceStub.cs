using Cleipnir.Flows;
using Rebus.Bus;
using Rebus.Handlers;
using Rebus.Transport;

namespace cBrain.Flows.Ordering.Clients;

public class LogisticsServiceStub(LogisticsServiceFlows flows) : IHandleMessages<ShipProducts>
{
    public async Task Handle(ShipProducts command)
    {
        await flows.Schedule(command.OrderId, command);
    }
}

[GenerateFlows]
public class LogisticsServiceFlow(IBus bus) : Flow<ShipProducts>
{
    public override async Task Run(ShipProducts command)
    {
        AmbientTransactionContext.SetCurrent(null); //rebus hack
        
        await Delay(TimeSpan.FromSeconds(1));
        await bus.SendLocal(new ProductsShipped(command.OrderId, TrackAndTraceNumber: Guid.NewGuid().ToString("N")));
    }
}