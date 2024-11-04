using System;
using Anemoi.BuildingBlock.Domain.Abstractions;

namespace Anemoi.BuildingBlock.Domain;

public record DomainEvent : IDomainEvent
{
    public Guid AggregateId { get; set; }
    public DateTime OccurredAt { get; set; }
    public string EventType { get; set; }
    public string AssemblyType { get; set; }

    protected DomainEvent()
    {
        AggregateId = Guid.NewGuid();
        OccurredAt = DateTime.UtcNow;
        EventType = GetType().Name;
        AssemblyType = $"{GetType().FullName},{GetType().Assembly.GetName().Name}";
    }
}