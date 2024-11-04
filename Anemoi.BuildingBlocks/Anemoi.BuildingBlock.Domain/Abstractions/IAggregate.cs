namespace Anemoi.BuildingBlock.Domain.Abstractions;

public interface IAggregate<out TKey> where TKey : IAggregateRoot
{
    TKey Id { get; }
}