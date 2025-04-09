using cBrain.Flows.Ordering.Rpc.Clients;
using Cleipnir.Flows;
using Cleipnir.ResilientFunctions.Domain;

namespace cBrain.Flows.Ordering.Rpc;

[GenerateFlows]
public class OrderFlow(
    IPaymentProviderClient paymentProviderClient,
    IEmailClient emailClient,
    ILogisticsClient logisticsClient
) : Flow<Order>
{
    public override async Task Run(Order order)
    {
        var transactionId = await Capture(Guid.NewGuid);
        await Capture(
            () => paymentProviderClient.Reserve(order.CustomerId, transactionId, order.TotalPrice),
            ResiliencyLevel.AtLeastOnceDelayFlush
        );
        await Capture(() => logisticsClient.ShipProducts(order.CustomerId, order.ProductIds));
        await Capture(() => paymentProviderClient.Capture(transactionId));
        await Capture(() => emailClient.SendOrderConfirmation(order.CustomerId, order.ProductIds));

        await Delay(TimeSpan.FromDays(1));
        await emailClient.SendFollowUpMail(order.CustomerId);
    }
    
    //what should be sent the day after?
    //follow up email with
}