namespace cBrain.Flows.Outbox;

public record Order(string OrderId, Guid CustomerId, IEnumerable<Guid> ProductIds, decimal TotalPrice);