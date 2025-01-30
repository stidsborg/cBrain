using Cleipnir.Flows;
using Cleipnir.ResilientFunctions.Reactive.Extensions;

namespace cBrain.Flows.Invoice;

[GenerateFlows]
public class InvoiceFlow(ILogger<InvoiceFlow> logger) : Flow<CustomerNumber>
{
    public override async Task Run(CustomerNumber customerNumber)
    {
        logger.LogInformation($"CUSTOMER_{customerNumber}: (Re)started flow");
        var invoiceDate = await Capture(
            () => DateTime.UtcNow.ToFirstOfMonth().AddMonths(1) //.AddSeconds(5)
        );

        while (true)
        {
            var option = await Message<CustomerRelationshipTerminated>(timesOutAt: invoiceDate);
            if (option.HasValue)
                return;
            
            await Capture(() => SendInvoice(customerNumber, invoiceDate));
            invoiceDate = invoiceDate.AddMonths(1); //.AddSeconds(5)
        }
    }
    
    private async Task SendInvoice(CustomerNumber customerNumber, DateTime invoiceDate)
    {
        logger.LogInformation($"CUSTOMER_{customerNumber}: Sending invoice '{invoiceDate:s}'");
        var outstandingAmount = await CalculateInvoiceAmount(customerNumber);
        await EmailInvoice(customerNumber, invoiceDate, outstandingAmount);
    }

    private Task<decimal> CalculateInvoiceAmount(CustomerNumber customerNumber) => Task.FromResult(1.2M);
    private Task EmailInvoice(CustomerNumber customerNumber, DateTime invoiceDate, decimal invoiceAmount) => Task.CompletedTask;
    private record CustomerRelationshipTerminated();
}

internal static class DateTimeExtensions 
{
    public static DateTime ToFirstOfMonth(this DateTime date) 
        => new(date.Year, date.Month, day: 1, hour: 0, minute: 0, second: 0, kind: date.Kind);
}