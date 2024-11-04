using System;
using MediatR;

namespace Anemoi.BuildingBlock.Domain.Abstractions;

public interface IDomainEvent : INotification
{
    Guid AggregateId { get; set; }

    DateTime OccurredAt { get; set; }

    string EventType { get; set; }
    string AssemblyType { get; set; }
}