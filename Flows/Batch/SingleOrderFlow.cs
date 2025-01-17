using cBrain.Flows.Ordering;
using cBrain.Flows.Ordering.Clients;
using Cleipnir.Flows;
using Rebus.Bus;

namespace cBrain.Flows.Batch;

[GenerateFlows]
public class SingleOrderFlow(
    IBus bus,
    ILogger<SingleOrderFlow> logger
) : Flow<Order, TransactionIdAndTrackAndTrace>
{
    public override async Task<TransactionIdAndTrackAndTrace> Run(Order order)
    {
        var transactionId = await Capture(Guid.NewGuid);

        await ReserveFunds(order, transactionId);
        await Message<FundsReserved>();
        await Capture(() => logger.LogInformation($"{order.OrderId.ToUpper()}: FundsReserved"));
        
        await ShipProducts(order);
        var productsShipped = await Message<ProductsShipped>();
        var trackAndTrace = productsShipped.TrackAndTraceNumber;
        await Capture(() => logger.LogInformation($"{order.OrderId.ToUpper()}: ProductsShipped"));
        
        await CaptureFunds(order, transactionId);
        await Message<FundsCaptured>();
        await Capture(() => logger.LogInformation($"{order.OrderId.ToUpper()}: FundsCaptured"));
        
        await SendOrderConfirmationEmail(order, trackAndTrace);
        await Message<OrderConfirmationEmailSent>();
        await Capture(() => logger.LogInformation($"{order.OrderId.ToUpper()}: OrderConfirmationEmailSent"));

        return new TransactionIdAndTrackAndTrace(order.OrderId, transactionId, trackAndTrace);
    }
    
    private Task ReserveFunds(Order order, Guid transactionId) 
        => Capture(async () => await bus.SendLocal(new ReserveFunds(order.OrderId, order.TotalPrice, transactionId, order.CustomerId)));
    private Task ShipProducts(Order order)
        => Capture(async () => await bus.SendLocal(new ShipProducts(order.OrderId, order.CustomerId, order.ProductIds)));
    private Task CaptureFunds(Order order, Guid transactionId)
        => Capture(async () => await bus.SendLocal(new CaptureFunds(order.OrderId, order.CustomerId, transactionId)));
    private Task SendOrderConfirmationEmail(Order order, string trackAndTraceNumber)
        => Capture(async () => await bus.SendLocal(new SendOrderConfirmationEmail(order.OrderId, order.CustomerId, trackAndTraceNumber)));
    private Task CancelProductsShipment(Order order)
        => Capture(async () => await bus.SendLocal(new CancelProductsShipment(order.OrderId)));
    private Task CancelFundsReservation(Order order, Guid transactionId)
        => Capture(async () => await bus.SendLocal(new CancelFundsReservation(order.OrderId, transactionId)));
    private Task ReversePayment(Order order, Guid transactionId)
        => Capture(async () => await bus.SendLocal(new ReverseTransaction(order.OrderId, transactionId)));
}