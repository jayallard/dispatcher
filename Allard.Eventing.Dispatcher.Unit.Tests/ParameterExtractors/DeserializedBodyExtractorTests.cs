using System.Reflection;
using Allard.Eventing.Abstractions;
using Allard.Eventing.Dispatcher.ParameterExtractors;
using FluentAssertions;
using NSubstitute;

namespace Allard.Eventing.Dispatcher.Unit.Tests.ParameterExtractors;

public class DeserializedBodyExtractorTests
{
    [Fact]
    public void ExtractsBodyAsObject()
    {
        // arrange
        const string json = "{ \"FirstName\": \"Santa\", \"LastName\": \"Claus\" }";
        var message = MessageEnvelopeBuilder
            .CreateMessage("a")
            .SetMessage(json)
            .Build();
        var messageContext = new MessageContext(message);
        var bodyExtractor = new DeserializedBodyExtractor();
        var paramInfo = Substitute.For<ParameterInfo>();
        paramInfo.ParameterType.Returns(typeof(Junk));

        // act
        var result = (Junk)bodyExtractor.ExtractParameter(paramInfo, messageContext);
        
        // assert
        result.FirstName.Should().Be("Santa");
        result.LastName.Should().Be("Claus");
    }
}

public class Junk
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
}