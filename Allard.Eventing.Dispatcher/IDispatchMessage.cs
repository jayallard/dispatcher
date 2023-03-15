namespace Allard.Eventing.Dispatcher;

internal interface IDispatchMessage
{
    public string MessageType { get; }
}