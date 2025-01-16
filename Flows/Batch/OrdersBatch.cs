using cBrain.Flows.Ordering;

namespace cBrain.Flows.Batch;

public record OrdersBatch(string BatchId, List<Order> Orders);