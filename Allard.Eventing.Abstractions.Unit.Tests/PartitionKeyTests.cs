using FluentAssertions;

namespace Allard.Eventing.Abstractions.Unit.Tests;

public class PartitionKeyTests
{
    [Fact]
    public void KeysAreEqual()
    {
        var k1 = PartitionKeyBuilder
            .CreatePartitionKey()
            .Add("id", "a")
            .Add("hello", "world")
            .Build();
        
        var k2 = PartitionKeyBuilder
            .CreatePartitionKey()
            .Add("id", "a")
            .Add("hello", "world")
            .Build();

        k2.Should().Be(k1);
    }
}