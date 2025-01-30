using Rebus.Bus;
using Rebus.Handlers;

namespace cBrain.Flows.LoanApplication;

public class CreditChecker1(IBus bus) : IHandleMessages<PerformCreditCheck>
{
    public async Task Handle(PerformCreditCheck cmd)
    {
        await Task.Delay(10);
        await bus.Publish(
            new CreditCheckOutcome(nameof(CreditChecker1), cmd.Id, Approved: true)
        );
    }
}

public class CreditChecker2(IBus bus) : IHandleMessages<PerformCreditCheck>
{
    public async Task Handle(PerformCreditCheck cmd)
    {
        await Task.Delay(10);
        await bus.SendLocal(
            new CreditCheckOutcome(nameof(CreditChecker2), cmd.Id, Approved: true)
        );
    }
}

public class CreditChecker3(IBus bus) : IHandleMessages<PerformCreditCheck> 
{
    public async Task Handle(PerformCreditCheck cmd)
    { 
        await Task.Delay(10);
        await bus.SendLocal(
            new CreditCheckOutcome(nameof(CreditChecker3), cmd.Id, Approved: true)
        );
    }
}