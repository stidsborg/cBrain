using cBrain.Flows.Batch.Clients;

namespace cBrain.Flows.Batch;

public record TransactionIdAndTrackAndTrace(string OrderId, Guid TransactionId, TrackAndTrace TrackAndTraceNumber);