using System.Collections.Immutable;
using System.Threading.Channels;

namespace Allard.Eventing.Dispatcher;

public class Subscription
{
    public Channel<DispatchEnvelope> SubscriptionChannel { get; }

    public ImmutableHashSet<string> MessageTypes { get; }

    public Subscription(
        Channel<DispatchEnvelope> subscriptionChannel,
        IEnumerable<string> messageTypes)
    {
        SubscriptionChannel = subscriptionChannel;
        MessageTypes = messageTypes.ToImmutableHashSet();
        if (!MessageTypes.Any())
        {
            throw new InvalidOperationException("Subscription must be fore at least one message type");
        }
    }

    public static Channel<DispatchEnvelope> MultiReaderChannel() => CreateChannel(true);
    public static Channel<DispatchEnvelope> SingleReaderChannel() => CreateChannel(false);
    private static Channel<DispatchEnvelope> CreateChannel(bool multiReader)
    {
        return Channel.CreateUnbounded<DispatchEnvelope>(new UnboundedChannelOptions
        {
            SingleReader = true,
            SingleWriter = true,
        });

    }
}