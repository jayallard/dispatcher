namespace Allard.Eventing.Dispatcher;

public class DispatcherSetup
{
    public List<SourceSetup> Sources { get; } = new();

    public DispatcherSetup AddSource(
        string id,
        Action<SourceSetup> setup)
    {
        if (Sources.Any(s => s.Id == "id"))
        {
            throw new InvalidOperationException("a source with that id already exists");
        }

        var s = new SourceSetup(id);
        setup(s);
        Sources.Add(s);
        return this;
    }
}