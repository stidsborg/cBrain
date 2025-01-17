using cBrain.Flows.Ordering.Clients;
using Rebus.Bus;
using Rebus.Handlers;
using Rebus.Sagas;

namespace cBrain.RebusFlows.Ordering;

public class SagaData : ISagaData
{
    public Guid Id { get; set; }
    public int Revision { get; set; }
    public Guid TransactionId { get; set; }
}

public class OrderSaga(IBus bus) : 
    Saga<SagaData>,
    IAmInitiatedBy<OrderCreated>,
    IHandleMessages<FundsReserved>,
    IHandleMessages<ProductsShipped>,
    IHandleMessages<FundsCaptured>,
    IHandleMessages<OrderConfirmationEmailSent>
{
    protected override void CorrelateMessages(ICorrelationConfig<SagaData> config)
    {
        config.Correlate<OrderCreated>(msg  => msg.Order.OrderId, data => data.Id);
        config.Correlate<FundsReserved>(msg  => msg.OrderId, data => data.Id);
        config.Correlate<ProductsShipped>(msg  => msg.OrderId, data => data.Id);
        config.Correlate<OrderConfirmationEmailSent>(msg  => msg.OrderId, data => data.Id);
    }
    
    public async Task Handle(OrderCreated message)
    {
        var order = message.Order;
        Data.TransactionId = Guid.NewGuid();
        await bus.SendLocal(
            new ReserveFunds(
                order.OrderId,
                order.TotalPrice,
                Data.TransactionId,
                order.CustomerId
            )
        );
    }

    public Task Handle(FundsReserved message)
    {
        throw new NotImplementedException();
    }

    public Task Handle(ProductsShipped message)
    {
        throw new NotImplementedException();
    }

    public Task Handle(FundsCaptured message)
    {
        throw new NotImplementedException();
    }

    public Task Handle(OrderConfirmationEmailSent message)
    {
        throw new NotImplementedException();
    }
}