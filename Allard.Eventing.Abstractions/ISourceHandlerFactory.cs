namespace Allard.Eventing.Abstractions;

public interface ISourceHandlerFactory
{
    ISourceHandler CreateHandler(string key);
}