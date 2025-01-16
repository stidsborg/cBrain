using cBrain.Flows.Ordering.Clients;
using Cleipnir.Flows;
using Rebus.Bus;
using Rebus.Transport;

namespace cBrain.Flows.Ordering.Solution;

public class MessageDrivenOrderFlow(IBus bus) : Flow<Order>
{
   public override async Task Run(Order order)
    {
        AmbientTransactionContext.SetCurrent(null); //rebus hack
        var transactionId = await Capture(Guid.NewGuid);

        await ReserveFunds(order, transactionId);
        var reservation = await Message<FundsReserved, FundsReservationFailed>(TimeSpan.FromSeconds(10));
        if (!reservation.HasFirst)
            return;

        await ShipProducts(order);
        var productsShipped = await Message<ProductsShipped, ProductsShipmentFailed>(TimeSpan.FromMinutes(5));
        if (!productsShipped.HasFirst)
            await CleanUp(FailedAt.ProductsShipped, order, transactionId);
        var trackAndTraceNumber = productsShipped.First.TrackAndTraceNumber;
        
        await CaptureFunds(order, transactionId);
        var capture = await Message<FundsCaptured, FundsCaptureFailed>(TimeSpan.FromSeconds(10));
        if (!capture.HasFirst)
            await CleanUp(FailedAt.FundsCaptured, order, transactionId);
        
        await SendOrderConfirmationEmail(order, trackAndTraceNumber);
        await Message<OrderConfirmationEmailSent, OrderConfirmationEmailFailed>(TimeSpan.FromSeconds(10));
    }
   
    private async Task CleanUp(FailedAt failedAt, Order order, Guid transactionId)
    {
        switch (failedAt) 
        {
            case FailedAt.FundsReserved:
                break;
            case FailedAt.ProductsShipped:
                await CancelFundsReservation(order, transactionId);
                break;
            case FailedAt.FundsCaptured:
                await CancelFundsReservation(order, transactionId);
                await CancelProductsShipment(order);
                break;
            case FailedAt.OrderConfirmationEmailSent:
                await CancelProductsShipment(order);
                await ReversePayment(order, transactionId);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(failedAt), failedAt, null);
        }

        throw new OrderProcessingException($"Order processing failed at: '{failedAt}'");
    }

    private enum FailedAt
    {
        FundsReserved,
        ProductsShipped,
        FundsCaptured,
        OrderConfirmationEmailSent,
    }
    
    private Task ReserveFunds(Order order, Guid transactionId) 
        => Capture(() => bus.SendLocal(new ReserveFunds(order.OrderId, order.TotalPrice, transactionId, order.CustomerId)));
    private Task ShipProducts(Order order)
        => Capture(() => bus.SendLocal(new ShipProducts(order.OrderId, order.CustomerId, order.ProductIds)));
    private Task CaptureFunds(Order order, Guid transactionId)
        => Capture(() => bus.SendLocal(new CaptureFunds(order.OrderId, order.CustomerId, transactionId)));
    private Task SendOrderConfirmationEmail(Order order, string trackAndTraceNumber)
        => Capture(() => bus.SendLocal(new SendOrderConfirmationEmail(order.OrderId, order.CustomerId, trackAndTraceNumber)));
    private Task CancelProductsShipment(Order order)
        => Capture(() => bus.SendLocal(new CancelProductsShipment(order.OrderId)));
    private Task CancelFundsReservation(Order order, Guid transactionId)
        => Capture(() => bus.SendLocal(new CancelFundsReservation(order.OrderId, transactionId)));
    private Task ReversePayment(Order order, Guid transactionId)
        => Capture(() => bus.SendLocal(new ReverseTransaction(order.OrderId, transactionId)));
}
