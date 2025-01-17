namespace cBrain.Flows.Batch;

public record TransactionIdAndTrackAndTrace(string OrderId, Guid TransactionId, string TrackAndTraceNumber);