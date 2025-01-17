namespace cBrain.Flows.LoanApplication;

public record LoanApplication(string Id, Guid CustomerId, decimal Amount, DateTime Created);