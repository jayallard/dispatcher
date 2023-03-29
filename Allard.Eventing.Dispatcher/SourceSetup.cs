using Allard.Eventing.Abstractions.Source;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Allard.Eventing.Dispatcher;

// TODO: need to REPLACE existing registrations, not append them.
// Use the REPLACE method.

/// <summary>
/// Define a message source for the Message Dispatcher.
/// </summary>
public class SourceSetup
{
    public SourceSetup(string id)
    {
        Id = id;
    }

    /// <summary>
    /// Gets the services to be used by the source.
    /// Preloaded with essentials.
    /// </summary>
    public IServiceCollection Services { get; } = new ServiceCollection()
        .AddLogging()
        .AddSingleton<ISourceHandler, DispatchSourceHandler>()
        .AddSingleton<SourceReader>()
        .AddSingleton<MessageBuffers>()
        .AddSingleton<IMessagePartitioner, PartitionByStreamId>()
        .AddTransient<MessageBuffer>();

    /// <summary>
    /// Gets the Id of this source.
    /// The Id must be unique across all sources in the dispatcher.
    /// </summary>
    public string Id { get; }

    private readonly List<Type> _subscriberTypes = new();

    /// <summary>
    /// Get the subscriber types that have been added to the setup.
    /// </summary>
    public IEnumerable<Type> SubscriberTypes => _subscriberTypes.AsEnumerable();

    /// <summary>
    /// Set the source type. IT will be resolved by the
    /// service provider.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public SourceSetup SetSource<T>() where T : class, ISource
    {
        Services.AddScoped<ISource, T>();
        return this;
    }

    /// <summary>
    /// Set the source specifically.
    /// </summary>
    /// <param name="source"></param>
    /// <returns></returns>
    public SourceSetup SetSource(ISource source)
    {
        Services.AddSingleton(source);
        return this;
    }

    /// <summary>
    /// Sets the partitioner.
    /// The partitioner determines which buffer to send
    /// each message to.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public SourceSetup SetPartitioner<T>() where T : class, IMessagePartitioner
    {
        Services.AddSingleton<IMessagePartitioner, T>();
        return this;
    }

    public SourceSetup AddSubscriber<T>() where T : class, ISubscriberMarker
    {
        // there can be multiple subscribers. don't REPLACE.
        Services.AddScoped<ISubscriberMarker, T>();
        Services.AddScoped<T>();
        _subscriberTypes.Add(typeof(T));
        return this;
    }
}