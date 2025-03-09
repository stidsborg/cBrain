namespace cBrain.Flows.Outbox;

public static class Module
{
    public static IServiceCollection AddOutboxFlows(this IServiceCollection services)
    {
        services.AddSingleton<IBus>(_ => new BusStub());
        return services;
    } 
}