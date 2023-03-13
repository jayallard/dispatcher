namespace Allard.Eventing.Dispatcher;

public class DispatchEnvelope
{
    public DispatchEnvelope(string messageType)
    {
        MessageType = messageType;
    }

    public string MessageType { get; }
}