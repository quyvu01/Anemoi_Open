using System;

namespace Anemoi.BuildingBlock.Application.Abstractions;

// ReSharper disable once UnusedTypeParameter
// I write this to get the first generic argument type, do not remove it as the ReSharper mention!
public interface INameMapIdOf<T> where T : Attribute, INameMapIdContract;

public interface INameMapIdContract
{
    string PropertyName { get; }
}