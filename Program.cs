using cBrain.Flows.Batch;
using cBrain.Flows.Ordering.Rpc;
using Cleipnir.Flows.AspNet;
using Cleipnir.Flows.PostgresSql;
using Cleipnir.ResilientFunctions.PostgreSQL;
using Rebus.Config;
using Rebus.Persistence.InMem;
using Rebus.Transport.InMem;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

const string connectionString = "Server=localhost;Port=5432;Userid=postgres;Password=Pa55word!;Database=flows;";
await DatabaseHelper.CreateDatabaseIfNotExists(connectionString); //use to create db initially or clean existing state in database

builder.Services.AddControllers();
builder.Services.AddFlows(c => c
    .UsePostgresStore(connectionString)
    .WithOptions(new Cleipnir.Flows.Options(leaseLength: TimeSpan.FromSeconds(5), messagesDefaultMaxWaitForCompletion: TimeSpan.Zero))
    .RegisterFlowsAutomatically()
);

builder.Services.AutoRegisterHandlersFromAssembly(typeof(Program).Assembly);
builder.Services.AddRebus(configure =>
    configure
        .Options(c => c.SetNumberOfWorkers(10))
        .Transport(t => t.UseInMemoryTransport(new InMemNetwork(), "who cares"))
        .Timeouts(t => t.StoreInMemory())
);
builder.Services.AddBatchOrderFlows();
builder.Services.AddRpcOrderFlows();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/openapi/v1.json", "OpenAPI V1");
    });
}

app.MapControllerRoute(name: "default", pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapGet("", context =>
{
    context.Response.Redirect("/swagger");
    return Task.CompletedTask;
});

app.Run();