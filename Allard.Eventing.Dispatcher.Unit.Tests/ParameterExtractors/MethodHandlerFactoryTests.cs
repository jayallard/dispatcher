using Allard.Eventing.Abstractions;
using Allard.Eventing.Dispatcher.ParameterExtractors;
using FluentAssertions;
using static Allard.Eventing.Dispatcher.Unit.Tests.ParameterExtractors.TestClasses;

namespace Allard.Eventing.Dispatcher.Unit.Tests.ParameterExtractors;

public class Blah
{
    [Fact]
    public void FindsSingleMessageContextParameter()
    {
        var handler = MethodHandlerFactory.GetHandlers<MessageContextHandler>().Single();
        handler.Extractors.Single().Should().BeOfType<MessageContextExtractor>();
    }
    
    [Fact]
    public void FindsSingleMessageEnvelopeParameter()
    {
        var handler = MethodHandlerFactory.GetHandlers<MessageEnvelopeHandler>().Single();
        handler.Extractors.Single().Should().BeOfType<MessageEnvelopeExtractor>();
    }

    [Fact]
    public void FindsSingleMessageOriginParameter()
    {
        var handler = MethodHandlerFactory.GetHandlers<MessageOriginHandler>().Single();
        handler.Extractors.Single().Should().BeOfType<MessageOriginExtractor>();
    }
}

public class TestClasses
{



    public class MessageContextHandler
    {
        [MessageHandler("abc")]
        public Task MessageContext(MessageContext mc)
        {
            return Task.CompletedTask;
        }
    }

    public class MessageEnvelopeHandler
    {
        [MessageHandler("abc")]
        public Task MessageEnvelope(MessageEnvelope e)
        {
            return Task.CompletedTask;
        }
    }

    public class MessageOriginHandler
    {
        [MessageHandler("abc")]
        public Task MessageEnvelope(MessageOrigin e)
        {
            return Task.CompletedTask;
        }
    }

}