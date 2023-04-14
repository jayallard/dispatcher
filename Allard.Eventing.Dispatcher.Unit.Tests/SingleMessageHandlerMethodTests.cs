using Allard.Eventing.Abstractions;
using Allard.Eventing.Abstractions.Model;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;

namespace Allard.Eventing.Dispatcher.Unit.Tests;

public class SingleMessageHandlerMethodTests
{
    /// <summary>
    /// SingleMessageHandlerMethod wraps and executes a MethodInfo.
    /// Show that the SingleMessageHandlerMethod.ExecuteMethod
    /// identifies and extracts all of the parameter methods.
    /// This includes the various properties of a MessageContext,
    /// and also a dependency provided by the service provider
    /// (dependency injection).
    /// </summary>
    [Fact]
    public async Task ExtractAllParametersTheMethodNeeds()
    {
        // -------------------------------------------------
        // arrange
        // -------------------------------------------------
        var dep = new ExtractorTest1Dependency();
        var services = new ServiceCollection()
            .AddSingleton(dep)
            .BuildServiceProvider();

        var method = typeof(ExtractorTest1)
            .GetMethods()
            .Single(m => m.Name == nameof(ExtractorTest1.Handle));

        // -------------------------------------------------
        // act
        // -------------------------------------------------
        var envelope = MessageEnvelopeBuilder
            .CreateMessage("abc")
            .Build();
        var context = new MessageContext(envelope, "source", services);
        var instance = new ExtractorTest1();
        await new SingleMessageHandlerMethod(method).Execute(context, instance);

        // -------------------------------------------------
        // assert
        // -------------------------------------------------
        instance.Context.Should().Be(context);
        instance.Envelope.Should().Be(context.Message);
        instance.Origin.Should().Be(context.Message.Origin);
        instance.Dependency.Should().Be(dep);
    }

    /// <summary>
    /// If the method has a dependency that's not registered in the container,
    /// then it will throw an exception at resolve time.
    /// </summary>
    [Fact]
    public async Task ThrowsExceptionIfServiceCantBeResolved()
    {
        // -------------------------------------------------
        // arrange
        // -------------------------------------------------
        var dep = new ExtractorTest1Dependency();
        
        // don't register the dependency
        var services = new ServiceCollection()
            .BuildServiceProvider();

        var method = typeof(ExtractorTest1)
            .GetMethods()
            .Single(m => m.Name == nameof(ExtractorTest1.Handle));

        // -------------------------------------------------
        // act
        // -------------------------------------------------
        var envelope = MessageEnvelopeBuilder
            .CreateMessage("atc")
            .Build();
        var context = new MessageContext(envelope, "source", services);
        var instance = new ExtractorTest1();
        var test = async () => await new SingleMessageHandlerMethod(method).Execute(context, instance);

        // -------------------------------------------------
        // assert
        // -------------------------------------------------
        await test
            .Should()
            .ThrowAsync<InvalidOperationException>()
            .WithMessage("No service for type * has been registered.");
    }

    public class ExtractorTest1
    {
        public MessageContext? Context { get; set; }
        public MessageEnvelope? Envelope { get; set; }
        public MessageOrigin? Origin { get; set; }
        public ExtractorTest1Dependency Dependency { get; set; }

        [MessageHandler("xyz")]
        public Task Handle(MessageContext c, MessageEnvelope e, MessageOrigin o, ExtractorTest1Dependency dep)
        {
            Context = c;
            Envelope = e;
            Origin = o;
            Dependency = dep;
            return Task.CompletedTask;
        }
    }

    public class ExtractorTest1Dependency
    {
    }
}