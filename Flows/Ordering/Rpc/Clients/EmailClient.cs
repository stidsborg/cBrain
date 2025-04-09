namespace cBrain.Flows.Ordering.Rpc.Clients;

public interface IEmailClient
{
    Task SendOrderConfirmation(Guid customerId, IEnumerable<Guid> productIds);
    Task SendFollowUpMail(Guid customerId);
}

public class EmailClientStub : IEmailClient
{
    public Task SendOrderConfirmation(Guid customerId, IEnumerable<Guid> productIds)
        => Task.Delay(100).ContinueWith(_ => 
            Console.WriteLine("EMAIL_SERVER: Order confirmation emailed")
        );

    public Task SendFollowUpMail(Guid customerId)
    {
        throw new NotImplementedException();
    }
}