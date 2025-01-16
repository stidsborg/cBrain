using Rebus.Bus;
using Rebus.Handlers;

namespace cBrain.Flows.Ordering.Other;

public class EmailServiceStub(IBus bus) : IHandleMessages<SendOrderConfirmationEmail>
{
    public async Task Handle(SendOrderConfirmationEmail command)
    {
        await Task.Delay(1_000);
        await bus.SendLocal(new OrderConfirmationEmailSent(command.OrderId, command.CustomerId));
    }
}