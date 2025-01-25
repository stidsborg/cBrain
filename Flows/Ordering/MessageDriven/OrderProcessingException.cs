namespace cBrain.Flows.Ordering.MessageDriven;

public class OrderProcessingException : Exception
{
    public OrderProcessingException(string message) : base(message) { }
}