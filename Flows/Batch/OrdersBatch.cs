using cBrain.Flows.Ordering;
using cBrain.Flows.Ordering.MessageDriven;

namespace cBrain.Flows.Batch;

public record OrdersBatch(string BatchId, List<Order> Orders);