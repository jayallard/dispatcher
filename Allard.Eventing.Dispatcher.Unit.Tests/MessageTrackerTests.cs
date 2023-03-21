using Allard.Eventing.Abstractions;
using FluentAssertions;

namespace Allard.Eventing.Dispatcher.Unit.Tests;

public class MessageTrackerTests
{
    [Fact]
    public void ThrowsExceptionIfSequenceNumberEqualLast()
    {
        var tracker = new MessageTracker(PartitionKey);
        tracker.Add(Create(10));
        var test = () => tracker.Add(Create(10));
        test.Should().Throw<InvalidOperationException>().WithMessage("message received out of order");
    }

    [Fact]
    public void ThrowsExceptionWhenSequenceNumberLessThanLast()
    {
        var tracker = new MessageTracker(PartitionKey);
        tracker.Add(Create(10));
        var test = () => tracker.Add(Create(9));
        test.Should().Throw<InvalidOperationException>().WithMessage("message received out of order");
    }

    [Fact]
    public void AddsMessagesWhenInOrder()
    {
        var tracker = new MessageTracker(PartitionKey);
        tracker.Add(Create(10));
        tracker.Add(Create(11));
        tracker.Add(Create(12));
        tracker.Add(Create(13));
        tracker.Messages.Count().Should().Be(4);
    }

    private MessageContext Create(int sequenceId)
    {
        var envelope =
            MessageEnvelopeBuilder
                .CreateMessage("blah")
            .SetOrigin(PartitionKey.StreamId, PartitionKey.PartitionId, sequenceId)
            .Build();

        return new MessageContext(envelope);
    }

    private PartitionKey PartitionKey { get; } = new("stream", "partition");
}