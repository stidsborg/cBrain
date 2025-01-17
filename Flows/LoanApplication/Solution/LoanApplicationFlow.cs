﻿/*
[GenerateFlows]
public class LoanApplicationFlow : Flow<LoanApplication>
{
    public override async Task Run(LoanApplication loanApplication)
    {
        
        await Effect.Capture(
            () => Bus.Publish(new PerformCreditCheck(loanApplication.Id, loanApplication.CustomerId, loanApplication.Amount))
        );
        
        var outcomes = await Messages
            .TakeUntilTimeout(TimeSpan.FromMinutes(15))
            .OfType<CreditCheckOutcome>()
            .Take(3)
            .Completion();
        
        if (outcomes.Count < 2)
            await Bus.Publish(new LoanApplicationRejected(loanApplication));
        else
            await Bus.Publish(
                outcomes.All(o => o.Approved)
                    ? new LoanApplicationApproved(loanApplication)
                    : new LoanApplicationRejected(loanApplication)
            );
    }
}*/