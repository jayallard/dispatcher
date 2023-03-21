using Allard.Eventing.Abstractions;

namespace Allard.Eventing.Dispatcher;

public class TriggerActionCommit : ITriggerAction
{
    public Task<MessageEnvelope?> Trigger(DispatchContext context)
    {
        var message =
            MessageEnvelopeBuilder
                .CreateMessage("dispatch::commit")
                .SetOrigin("dispatch::", "dispatch::", 0)
                .Build();
        
        Console.WriteLine("======= commit");
        return Task.FromResult(message);
    }
}