using cBrain.Flows.Ordering.Rpc.Clients;

namespace cBrain.Flows.Ordering.Rpc;

public static class Module
{
    public static IServiceCollection AddRpcOrderFlows(this IServiceCollection services)
    {
        services.AddSingleton<IEmailClient, EmailClientStub>();
        services.AddSingleton<ILogisticsClient, LogisticsClientStub>();
        services.AddSingleton<IPaymentProviderClient, PaymentProviderClientStub>();
        
        return services;
    } 
}