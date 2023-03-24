using System.Threading.Channels;
using Allard.Eventing.Abstractions;
using FluentAssertions;

namespace Allard.Eventing.Dispatcher.Unit.Tests;

public class Demo
{
    private string[] _items = new string[] { "a", "b", "c" };

    [Fact]
    public void Swap()
    {
        var a = _items;

        var replacementList = new string[] { "a", "b", "c", "d", "e" };
        var swap = Interlocked.Exchange(ref _items, replacementList);

        var remainder = a.ToArray();
        remainder[0].Should().Be("a");
        remainder[1].Should().Be("b");
        remainder[2].Should().Be("c");
        remainder.Length.Should().Be(3);
        
        _items.Length.Should().Be(5);
    }

    [Fact]
    public void InterlockedCompareExchange()
    {
        var x = 0;
        var original = Interlocked.CompareExchange(ref x, 1, 0);
        original.Should().Be(0);

        var next = Interlocked.CompareExchange(ref x, 1, 0);
        next.Should().Be(1);
    }

    [Fact]
    public async Task Junk()
    {
        var source = Channel.CreateBounded<string>(10);
        var target = Channel.CreateBounded<string>(5);
        for (var i = 0; i < 10; i++)
        {
            await source.Writer.WriteAsync(i.ToString());
        }
    }

    /// <summary>
    /// demonstrating how the reset event works... just making
    /// sure that calling SET when already SET isn't a
    /// problem
    /// </summary>
    [Fact]
    public void Reset()
    {
        var evt = new ManualResetEventSlim();
        evt.IsSet.Should().BeFalse();
        evt.Set();
        evt.Set();
        evt.Set();
        evt.IsSet.Should().BeTrue();
        evt.Reset();
        evt.IsSet.Should().BeFalse();
    }
}