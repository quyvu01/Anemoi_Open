namespace Anemoi.BuildingBlock.Domain.Abstractions;

public interface IEventApply;

public interface IEventApply<in TEvent> : IEventApply where TEvent : DomainEvent
{
    void AppendEvent(TEvent aggregate);
    void Apply(TEvent aggregate);
}