using cBrain.Flows.Ordering;
using Cleipnir.Flows;
using Cleipnir.ResilientFunctions.Reactive.Extensions;
using Rebus.Bus;

namespace cBrain.Flows.DeferedMessaging;

[GenerateFlows]
public class DeferredFlow(IBus bus) : Flow<Order>
{
    public override async Task Run(Order order)
    {
        Console.WriteLine($"Processing order: '{order.OrderId}'");
        
        await Capture(() => bus.DeferLocal(TimeSpan.FromSeconds(5), new DeferredMessage(order.OrderId)));

        Console.WriteLine("Waiting for DeferredMessage...");
        await Messages.OfType<DeferredMessage>().First();
        Console.WriteLine("Received DeferredMessage...");
    }
}