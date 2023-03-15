using Allard.Eventing.Abstractions;

namespace Allard.Eventing.Dispatcher;

public class TriggerActionCommit : ITriggerAction
{
    public Task<MessageEnvelope?> Trigger(DispatchContext context)
    {
        var message =
            MessageEnvelopeBuilder
                .CreateMessage("dispatch::commit")
                .SetOrigin("dispatcher", string.Empty, 0)
                .Build();
        
        Console.WriteLine("======= commit");
        return Task.FromResult(message);
    }
}