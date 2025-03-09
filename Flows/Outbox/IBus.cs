namespace cBrain.Flows.Outbox;

public interface IBus
{
    public Task Publish(object message);
}

public class BusStub : IBus
{
    public Task Publish(object message) => Task.CompletedTask;
}