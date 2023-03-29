using System.Collections.ObjectModel;
using Allard.Eventing.Abstractions.Model;
using static System.Text.Encoding;

namespace Allard.Eventing.Abstractions;

public class MessageEnvelopeBuilder
{
    public string MessageType { get; }
    public byte[]? Message { get; private set; }
    public byte[]? Key { get; private set; }
    public MessageOrigin Origin { get; private set; }

    public ReadOnlyDictionary<string, byte[][]> Headers =>
        _headers
            .ToDictionary(kv => kv.Key, kv => kv.Value.ToArray())
            .AsReadOnly();

    private Dictionary<string, List<byte[]>> _headers { get; } = new();

    private MessageEnvelopeBuilder(string messageType)
    {
        MessageType = messageType;
    }

    public static MessageEnvelopeBuilder CreateMessage(string messageType)
    {
        return new MessageEnvelopeBuilder(messageType);
    }

    public MessageEnvelopeBuilder SetMessage(string message)
    {
        Message = UTF8.GetBytes(message);
        return this;
    }

    public MessageEnvelopeBuilder SetKey(string key)
    {
        Key = UTF8.GetBytes(key);
        return this;
    }

    public MessageEnvelopeBuilder SetOrigin(string streamId, string partitionId, long sequenceNumber)
    {
        Origin = new MessageOrigin(streamId, partitionId, sequenceNumber);
        return this;
    }

    public MessageEnvelopeBuilder AddHeaderValue(string headerName, string headerValue)
    {
        if (!_headers.ContainsKey(headerName))
        {
            _headers.Add(headerName, new List<byte[]>());
        }

        _headers[headerName].Add(UTF8.GetBytes(headerValue));
        return this;
    }

    public MessageEnvelope Build()
    {
        return new MessageEnvelope(
            messageType: MessageType,
            key: Key ?? Array.Empty<byte>(),
            body: Message ?? Array.Empty<byte>(),
            origin: Origin,
            headers: _headers.Select(h => new KeyValuePair<string, byte[][]>(h.Key, h.Value.ToArray())));
    }
}