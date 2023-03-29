using System.Collections.Immutable;
using static System.StringComparer;

namespace Allard.Eventing.Abstractions.Model;

public class MessageEnvelope
{
    public MessageEnvelope(
        string messageType, 
        byte[] key, 
        byte[] body, 
        MessageOrigin origin, 
        IEnumerable<KeyValuePair<string, byte[][]>> headers)
    {
        // TODO: validations
        MessageType = messageType;
        Key = key;
        Body = body;
        Origin = origin;
        Headers = headers.ToImmutableDictionary(OrdinalIgnoreCase);
    }

    public MessageOrigin Origin { get; }
    public string MessageType { get; }
    public byte[] Key { get; }
    public byte[] Body { get; }
    public ImmutableDictionary<string, byte[][]> Headers { get; }
}