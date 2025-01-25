using cBrain.Flows.Ordering.MessageDriven.Clients;
using Rebus.Handlers;

namespace cBrain.Flows.Ordering.MessageDriven;

public record DeferredMessage(string OrderId);
public class OrderHandler(MessageDrivenOrderFlows flows) : 
    IHandleMessages<DeferredMessage>,
    IHandleMessages<FundsReserved>,
    IHandleMessages<FundsReservationFailed>,
    IHandleMessages<FundsCaptured>,
    IHandleMessages<FundsCaptureFailed>,
    IHandleMessages<ProductsShipped>,
    IHandleMessages<ProductsShipmentFailed>,
    IHandleMessages<OrderConfirmationEmailSent>,
    IHandleMessages<OrderConfirmationEmailFailed>
{
    public Task Handle(DeferredMessage message) => flows.SendMessage(message.OrderId, message);
    public Task Handle(FundsReserved message) => flows.SendMessage(message.OrderId, message);
    public Task Handle(FundsReservationFailed message) => flows.SendMessage(message.OrderId, message);
    public Task Handle(FundsCaptured message) => flows.SendMessage(message.OrderId, message);
    public Task Handle(FundsCaptureFailed message) => flows.SendMessage(message.OrderId, message);
    public Task Handle(ProductsShipped message) => flows.SendMessage(message.OrderId, message);
    public Task Handle(ProductsShipmentFailed message) => flows.SendMessage(message.OrderId, message);
    public Task Handle(OrderConfirmationEmailSent message) => flows.SendMessage(message.OrderId, message);
    public Task Handle(OrderConfirmationEmailFailed message) => flows.SendMessage(message.OrderId, message);
}
