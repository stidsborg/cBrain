using Npgsql;

namespace cBrain.Flows.Outbox;

public class ClassicalOutbox
{
    public async Task Handle(Order order)
    {
        await using var transaction = StartTransaction();
        
        await ProcessAndPersistOrder(order, transaction);
        await PersistToOutboxTable(new OrderHandled(order.OrderId), transaction);

        await transaction.CommitAsync();
    }

    #region SaveOrderStateToDatabase

    private Task ProcessAndPersistOrder(Order order, NpgsqlTransaction transaction) => Task.CompletedTask;
    private Task PersistToOutboxTable(object message, NpgsqlTransaction transaction) => Task.CompletedTask;
    private NpgsqlTransaction StartTransaction() => throw new NotImplementedException();
    
    private record OrderHandled(string OrderNumber);

    #endregion
}