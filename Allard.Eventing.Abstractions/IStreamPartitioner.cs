namespace Allard.Eventing.Abstractions;

public interface IStreamPartitioner
{
    public StreamKey GetStreamKey(MessageOrigin origin);
}