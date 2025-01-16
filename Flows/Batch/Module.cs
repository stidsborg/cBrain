using cBrain.Flows.Batch.Clients;

namespace cBrain.Flows.Batch;

public static class Module
{
    public static IServiceCollection AddBatchOrderFlows(this IServiceCollection services)
    {
        services.AddSingleton<IPaymentProviderClient, PaymentProviderClientStub>();
        services.AddSingleton<IEmailClient, EmailClientStub>();
        services.AddSingleton<ILogisticsClient, LogisticsClientStub>();
        return services;
    }
}