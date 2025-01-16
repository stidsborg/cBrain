namespace cBrain.Flows.BankTransfer;

public record Transfer(
    Guid TransactionId,
    string FromAccount,
    string ToAccount,
    decimal Amount
);