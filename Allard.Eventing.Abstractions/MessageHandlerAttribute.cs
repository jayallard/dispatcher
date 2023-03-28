namespace Allard.Eventing.Abstractions;

[AttributeUsage(AttributeTargets.Method, AllowMultiple = true, Inherited = false)]
public class MessageHandlerAttribute : Attribute
{
    public MessageHandlerAttribute(string messageType)
    {
        MessageType = messageType;
    }

    public string MessageType { get; }
}