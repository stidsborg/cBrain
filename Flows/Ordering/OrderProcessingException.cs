namespace cBrain.Flows.Ordering;

public class OrderProcessingException : Exception
{
    public OrderProcessingException(string message) : base(message) { }
}