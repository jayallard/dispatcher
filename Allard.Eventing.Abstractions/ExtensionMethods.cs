namespace Allard.Eventing.Abstractions;

public static class ExtensionMethods
{
    public static TimeSpan Elapsed(this DateTimeOffset start) => DateTimeOffset.Now.Subtract(start);

    public static bool IsDispatchMessage(this MessageEnvelope message) => message.MessageType.StartsWith("dispatch::");
}