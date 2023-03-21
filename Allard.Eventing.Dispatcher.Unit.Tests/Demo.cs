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
    public void Dispatch()
    {
    }
}