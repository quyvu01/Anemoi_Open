namespace Anemoi.BuildingBlock.Application.Abstractions;

public interface ICrossCuttingConcernCore
{
    string PropertyName { get; }
    string Expression { get; set; }
    int Order { get; set; }
}

public interface IDataMappableCore : ICrossCuttingConcernCore;
public interface IDataCountingCore : ICrossCuttingConcernCore;