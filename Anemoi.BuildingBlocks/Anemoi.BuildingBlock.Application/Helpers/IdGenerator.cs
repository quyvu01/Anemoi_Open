using System;

namespace Anemoi.BuildingBlock.Application.Helpers;

public static class IdGenerator
{
    public static Guid NextGuid() => MassTransit.NewId.NextGuid();
}