namespace Allard.Eventing.Abstractions.Source;

public interface ISourceHandlerFactory
{
    ISourceHandler CreateHandler(string key);
}