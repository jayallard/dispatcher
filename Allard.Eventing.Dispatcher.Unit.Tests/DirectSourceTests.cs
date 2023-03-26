using Allard.Eventing.Abstractions;
using FluentAssertions;

namespace Allard.Eventing.Dispatcher.Unit.Tests;

public class DirectSourceTests
{
    [Fact]
    public async Task ReturnsInOrder()
    {
        var source = new DirectSource();
        source.Send(Create(0));
        await source.PausePartitions(new[] { new MessageOrigin("", "0", 0) });
        (await source.Get()).Should().BeNull();

        await source.ResumePartition(new[] { new MessageOrigin("", "0", 0) });
        (await source.Get())!.Origin.PartitionId.Should().Be("0");
    }

    private static MessageEnvelope Create(int partition)
    {
        return MessageEnvelopeBuilder
            .CreateMessage("a")
            .SetOrigin("stream", partition.ToString(), 0)
            .Build();
    }
}