using Anemoi.BuildingBlock.Application.Abstractions;

namespace Anemoi.Contract.CrossCuttingConcern.Attributes;

[AttributeUsage(AttributeTargets.Property)]
public class DistrictOfAttribute(string propertyName) : Attribute, IDataMappableCore
{
    public string PropertyName { get; } = propertyName;
    public string Expression { get; set; }
    public int Order { get; set; }
}