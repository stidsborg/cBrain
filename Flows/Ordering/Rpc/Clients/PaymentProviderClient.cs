﻿namespace cBrain.Flows.Ordering.Rpc.Clients;

public interface IPaymentProviderClient
{
    Task Reserve(Guid customerId, Guid transactionId, decimal amount);
    Task Capture(Guid transactionId);
    Task CancelReservation(Guid transactionId);
}

public class PaymentProviderClientStub : IPaymentProviderClient
{
    public Task Reserve(Guid customerId, Guid transactionId, decimal amount)
        => Task.Delay(100).ContinueWith(_ =>
            Console.WriteLine($"PAYMENT_PROVIDER: Reserved '{amount}'")
        );
    
    public Task Capture(Guid transactionId) 
        => Task.Delay(100).ContinueWith(_ => 
            Console.WriteLine("PAYMENT_PROVIDER: Reserved amount captured")
        );
    public Task CancelReservation(Guid transactionId) 
        => Task.Delay(100).ContinueWith(_ => 
            Console.WriteLine("PAYMENT_PROVIDER: Reservation cancelled")
        );
}