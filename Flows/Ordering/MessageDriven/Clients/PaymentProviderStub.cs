using Cleipnir.Flows;
using Rebus.Bus;
using Rebus.Handlers;
using Rebus.Transport;

namespace cBrain.Flows.Ordering.MessageDriven.Clients;

public class PaymentProviderStub(PaymentProviderFlows flows) : 
    IHandleMessages<ReserveFunds>,
    IHandleMessages<CaptureFunds>,
    IHandleMessages<CancelFundsReservation>
{
    private async Task MessageHandler(EventsAndCommands message, string orderId)
    {
        await flows.SendMessage(orderId, message);
    }

    public Task Handle(ReserveFunds message) => MessageHandler(message, message.OrderId);
    public Task Handle(CaptureFunds message) => MessageHandler(message, message.OrderId);
    public Task Handle(CancelFundsReservation message) => MessageHandler(message, message.OrderId);
}

[GenerateFlows]
public class PaymentProviderFlow(IBus bus) : Flow
{
    public override async Task Run()
    {
        AmbientTransactionContext.SetCurrent(null); //rebus hack

        await foreach (var msg in Messages)
        {
            var reply = msg switch
            {
                CaptureFunds captureFunds => new FundsCaptured(captureFunds.OrderId),
                ReserveFunds reserveFunds => new FundsReserved(reserveFunds.OrderId),
                CancelFundsReservation cancelFundsReservation => new FundsReservationCancelled(
                    cancelFundsReservation.OrderId),
                _ => default(EventsAndCommands)
            };

            if (reply != null)
                await Capture(() => bus.SendLocal(reply));
        }
    }
}