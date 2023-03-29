using System.Reflection;
using Allard.Eventing.Abstractions;

namespace Allard.Eventing.Dispatcher.ParameterExtractors;

public static class MethodHandlerFactory
{
    public static IEnumerable<SingleMessageHandlerMethod> GetHandlers<T>() where T : ISubscriberMarker
    {
        return GetHandlers(typeof(T));
    }

    public static IEnumerable<SingleMessageHandlerMethod> GetHandlers(Type t)
    {
        return t
            .GetMethods()
            .Where(m => m.GetCustomAttributes<MessageHandlerAttribute>().Any())
            .Select(m => new SingleMessageHandlerMethod(m));
    }
}