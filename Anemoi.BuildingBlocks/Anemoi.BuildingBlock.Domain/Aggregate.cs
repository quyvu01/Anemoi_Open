using System.Collections.Generic;
using Anemoi.BuildingBlock.Domain.Abstractions;

namespace Anemoi.BuildingBlock.Domain;

public abstract class Aggregate<TState, TKey> :
    IAggregate<TKey>
    where TKey : IAggregateRoot
    where TState : Aggregate<TState, TKey>
{
    private readonly List<IDomainEvent> _domainEvents = new();

    /// <summary>
    /// Domain events occurred.
    /// </summary>
    public IReadOnlyCollection<IDomainEvent> GetUncommittedEvents() => _domainEvents.AsReadOnly();

    public void CommitEvents() => _domainEvents.Clear();

    /// <summary>
    /// Add domain event.
    /// Todo: Currently, I am using dynamic for Apply method calling, will investigate and update later!
    /// </summary>
    /// <param name="domainEvent">Domain event.</param>
    private void AddEvent(IDomainEvent domainEvent) => _domainEvents.Add(domainEvent);

    public void Emit(IDomainEvent domainEvent) => AddEvent(domainEvent);

    public TKey Id { get; set; }
}