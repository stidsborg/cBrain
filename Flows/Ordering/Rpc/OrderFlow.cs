using cBrain.Flows.Ordering.Rpc.Clients;
using Cleipnir.Flows;

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
        var transactionId = Guid.NewGuid();
        
        await paymentProviderClient.Reserve(order.CustomerId, transactionId, order.TotalPrice);

        await logisticsClient.ShipProducts(order.CustomerId, order.ProductIds);

        await paymentProviderClient.Capture(transactionId);

        await emailClient.SendOrderConfirmation(order.CustomerId, order.ProductIds);
    }
}