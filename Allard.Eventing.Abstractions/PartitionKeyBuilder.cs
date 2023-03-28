namespace Allard.Eventing.Abstractions;

public class PartitionKeyBuilder
{
    private PartitionKeyBuilder()
    {
    }

    public static PartitionKeyBuilder CreatePartitionKey()
    {
        return new PartitionKeyBuilder();
    }
    
    private List<PartitionKeyField> Fields { get; } = new();

    public PartitionKeyBuilder Add(string key, string value)
    {
        Fields.Add(new PartitionKeyField(key, value));
        return this;
    }

    public string Build()
    {
        return Fields.Aggregate(string.Empty, (current, f) => current + $"${f.Key}={f.Value};");
    }
}