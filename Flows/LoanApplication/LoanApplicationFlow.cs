using Cleipnir.Flows;
using Cleipnir.ResilientFunctions.Reactive.Extensions;
using Rebus.Bus;

namespace cBrain.Flows.LoanApplication;

[GenerateFlows]
public class LoanApplicationFlow(IBus bus) : Flow<LoanApplication>
{
    public override async Task Run(LoanApplication loanApplication)
    {
        await bus.SendLocal(new PerformCreditCheck(loanApplication.Id, loanApplication.CustomerId, loanApplication.Amount));
        
        //replies are of type CreditCheckOutcome
        
        var outcomes = await Messages
            .OfType<CreditCheckOutcome>()
            .Take(3)
            .Completion();

        CommandAndEvents decision = DateTime.Now.Ticks % 2 == 0
            ? new LoanApplicationApproved(loanApplication)
            : new LoanApplicationRejected(loanApplication);

        await bus.SendLocal(decision);
    }
}