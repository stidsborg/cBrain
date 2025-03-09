using Cleipnir.Flows;

namespace cBrain.Flows.Outbox;

[GenerateFlows]
public class OutboxFlow(IBus bus) : Flow<Order>
{
    public override async Task Run(Order order)
    {
        await Capture(() => ProcessAndPersistOrder(order));
        
        await Capture(() => bus.Publish(new OrderHandled(order.OrderId)));
    }

    #region SaveOrderStateToDatabase

    private Task ProcessAndPersistOrder(Order order) => Task.CompletedTask;
    private Task PersistToOutboxTable(object message) => Task.CompletedTask;

    private record OrderHandled(string OrderNumber);

    #endregion
}