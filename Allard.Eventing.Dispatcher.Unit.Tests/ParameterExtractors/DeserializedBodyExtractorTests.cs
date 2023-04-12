using System.Reflection;
using Allard.Eventing.Abstractions;
using Allard.Eventing.Abstractions.Model;
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
        var messageContext = new MessageContext(message, "source 1", Substitute.For<IServiceProvider>());
        var paramInfo = Substitute.For<ParameterInfo>();
        paramInfo.ParameterType.Returns(typeof(Junk));
        var bodyExtractor = new DeserializedBodyExtractor(paramInfo);

        // act
        var result = (Junk)bodyExtractor.ExtractParameter(messageContext);
        
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