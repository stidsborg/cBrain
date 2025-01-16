using Rebus.Bus;
using Rebus.Handlers;

namespace cBrain.Flows.Ordering.Other;

public class PaymentProviderStub(IBus bus) : 
    IHandleMessages<ReserveFunds>,
    IHandleMessages<CaptureFunds>,
    IHandleMessages<CancelFundsReservation>
{
    private async Task MessageHandler(EventsAndCommands message)
    {
        var response = message switch
        {
            CaptureFunds captureFunds => new FundsCaptured(captureFunds.OrderId),
            ReserveFunds reserveFunds => new FundsReserved(reserveFunds.OrderId),
            CancelFundsReservation cancelFundsReservation => new FundsReservationCancelled(cancelFundsReservation.OrderId),
            _ => default(EventsAndCommands)
        };
        if (response == null) return;

        await Task.Delay(1_000);
        await bus.SendLocal(response);
    }

    public Task Handle(ReserveFunds message) => MessageHandler(message);
    public Task Handle(CaptureFunds message) => MessageHandler(message);
    public Task Handle(CancelFundsReservation message) => MessageHandler(message);
}