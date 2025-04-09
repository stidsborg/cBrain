using System.Diagnostics;
using cBrain.Flows.Batch.Clients;
using cBrain.Flows.Ordering;
using cBrain.Flows.Ordering.MessageDriven;
using Cleipnir.Flows;
using Cleipnir.ResilientFunctions;
using Cleipnir.ResilientFunctions.Domain;

namespace cBrain.Flows.Batch;

[GenerateFlows]
public class BatchOrderFlow(
    ILogger<BatchOrderFlow> logger,
    SingleOrderFlows orderFlows,
    IPaymentProviderClient paymentProviderClient,
    IEmailClient emailClient,
    ILogisticsClient logisticsClient
) : Flow<List<Order>>
{
    public override async Task Run(List<Order> orders)
    {
        var stopWatch = Stopwatch.StartNew();
        // APPROACH_2: parallel (in-process) //
        /*
        var tasks = orders
            .Select(order => Capture(() => ProcessOrder(order)))
            .ToList();

        var transactionIdAndTrackAndTraces = await Task.WhenAll(tasks);
        */
        
        // APPROACH_3: parallel (distributed) //
        
        var transactionIdAndTrackAndTraces = await orderFlows
            .BulkSchedule(orders.Select(order => new BulkWork<Order>(order.OrderId, order)))
            .Completion();

        await PublishEvent(new OrdersBatchProcessed(transactionIdAndTrackAndTraces));
        logger.LogInformation("Flow completed: {Duration}", stopWatch.Elapsed);
    }

    private async Task<TransactionIdAndTrackAndTrace> ProcessOrder(Order order)
    {
        var transactionId = await Capture(Guid.NewGuid);

        await paymentProviderClient.Reserve(transactionId, order.CustomerId, order.TotalPrice);
        var trackAndTrace = await Capture(
            () => logisticsClient.ShipProducts(order.CustomerId, order.ProductIds)
        );
        await paymentProviderClient.Capture(transactionId);
        await emailClient.SendOrderConfirmation(order.CustomerId, trackAndTrace, order.ProductIds);

        return new TransactionIdAndTrackAndTrace(order.OrderId, transactionId, trackAndTrace);
    }

    private Task PublishEvent(OrdersBatchProcessed ordersBatchProcessed) => Task.CompletedTask;
    public record OrdersBatchProcessed(IEnumerable<TransactionIdAndTrackAndTrace> Orders);
}