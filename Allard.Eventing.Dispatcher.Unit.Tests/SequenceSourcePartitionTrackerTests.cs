using Allard.Eventing.Abstractions.Model;
using FluentAssertions;

namespace Allard.Eventing.Dispatcher.Unit.Tests;

public class SequenceSourcePartitionTrackerTests
{
    [Fact]
    public void CantCompleteAnItemTwice()
    {
        var tracker = new SequenceSourcePartitionTracker();
        var key = new MessageOrigin("s", "p", 0);
        tracker.Start(key);
        tracker.Complete(key);
        var test = () => tracker.Complete(key);
        test.Should().Throw<InvalidOperationException>().WithMessage("already complete");
    }

    [Fact]
    public void CantStartOutOfOrder()
    {
        var tracker = new SequenceSourcePartitionTracker();
        var high = new MessageOrigin("s", "p", 20);
        tracker.Start(high);

        var low = new MessageOrigin("s", "p", 19);
        var test = () => tracker.Start(low);
        test.Should().Throw<InvalidOperationException>().WithMessage("can't start items out of order");
    }

    [Fact]
    public void CantStartTwice()
    {
        var tracker = new SequenceSourcePartitionTracker();
        var key = new MessageOrigin("s", "p", 0);
        tracker.Start(key);
        var test = () => tracker.Start(key);
        test.Should().Throw<InvalidOperationException>().WithMessage("already started");
    }

    [Fact]
    public void CantCompleteAnItemThatWasNotStarted()
    {
        var tracker = new SequenceSourcePartitionTracker();
        var key = new MessageOrigin("s", "p", 0);
        var test = () => tracker.Complete(key);
        test.Should().Throw<InvalidOperationException>().WithMessage("unknown id");
    }
    
    [Fact]
    public void ItemsReturnWhenInSequence()
    {
        var tracker = new SequenceSourcePartitionTracker();
        var ids = Enumerable
            .Range(1, 11)
            .Select(i => new MessageOrigin("s1", "p1", i))
            .ToArray();
        foreach (var id in ids)
        {
            tracker.Start(id);
        }

        // none complete, so won't return any.
        // current state = 0000000000
        tracker.GetCompleteAndClear().Should().BeEmpty();
        
        // set the third one to complete
        // set current state = 0010000000
        tracker.Complete(ids[2]);
        
        // still none, because 1 and 2 aren't done yet
        tracker.GetCompleteAndClear().Should().BeEmpty();
        
        // finish the first one
        // set current state = 1010000000
        tracker.Complete(ids[0]);
        
        // one will be returned
        // there are now 9 items left
        tracker.GetCompleteAndClear().Single().SequenceNumber.Should().Be(1);
        
        // current state = x010000000
        // wont' return any because the first isn't set
        tracker.GetCompleteAndClear().Should().BeEmpty();
        
        // set current state = x111100000
        tracker.Complete(ids[1]);
        tracker.Complete(ids[3]);
        tracker.Complete(ids[4]);
        
        tracker.GetCompleteAndClear().Last().SequenceNumber.Should().Be(5);
    }
}

public static class TrackerExtensionMethods {
    public static IEnumerable<MessageOrigin> GetCompleteAndClear(this SequenceSourcePartitionTracker tracker)
    {
        var complete = tracker.GetComplete().ToArray();
        tracker.Clear(complete);
        return complete;
    }
}