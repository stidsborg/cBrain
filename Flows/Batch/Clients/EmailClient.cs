﻿namespace cBrain.Flows.Batch.Clients;

public interface IEmailClient
{
    Task SendOrderConfirmation(Guid customerId, TrackAndTrace trackAndTrace, IEnumerable<Guid> productIds);
}

public class EmailClientStub(ILogger<EmailClientStub> logger) : IEmailClient
{
    public Task SendOrderConfirmation(Guid customerId, TrackAndTrace trackAndTrace, IEnumerable<Guid> productIds)
        => Task.Delay(ClientSettings.Delay).ContinueWith(_ =>
            logger.LogInformation("EMAIL_SERVER: Order confirmation emailed")
        );
}