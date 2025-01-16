using Cleipnir.Flows;
using Rebus.Bus;
using Rebus.Handlers;
using Rebus.Transport;

namespace cBrain.Flows.Ordering.Clients;

public class EmailServiceStub(EmailServiceFlows flows) : IHandleMessages<SendOrderConfirmationEmail>
{
    public async Task Handle(SendOrderConfirmationEmail command)
    {
        await flows.Schedule(command.OrderId, command);
    }
}

[GenerateFlows]
public class EmailServiceFlow(IBus bus) : Flow<SendOrderConfirmationEmail>
{
    public override async Task Run(SendOrderConfirmationEmail command)
    {
        AmbientTransactionContext.SetCurrent(null); //rebus hack
        
        await Delay(TimeSpan.FromSeconds(1));
        await bus.SendLocal(new OrderConfirmationEmailSent(command.OrderId, command.CustomerId));
    }
}