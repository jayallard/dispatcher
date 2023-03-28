using System.Reflection;
using Allard.Eventing.Abstractions;

namespace Allard.Eventing.Dispatcher.ParameterExtractors;

public static class MethodHandlerFactory
{
    public static IEnumerable<SingleMessageHandlerMethod> GetHandlers<T>()
    {
        return typeof(T)
            .GetMethods()
            .Where(m => m.GetCustomAttributes<MessageHandlerAttribute>().Any())
            .Select(m => new SingleMessageHandlerMethod(m));
    }
}