using Rebus.Bus;
using Rebus.Sagas;

namespace cBrain.RebusFlows.Ordering;

public class SagaData : ISagaData
{
    public Guid Id { get; set; }
    public int Revision { get; set; }
}

#pragma warning disable
public class OrderSaga(IBus bus) : Saga<SagaData>
#pragma warning restore
{
    protected override void CorrelateMessages(ICorrelationConfig<SagaData> config)
    {
        
        throw new NotImplementedException();
    }
}