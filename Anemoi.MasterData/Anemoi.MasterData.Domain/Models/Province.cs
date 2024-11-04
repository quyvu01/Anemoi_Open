using Anemoi.BuildingBlock.Domain;
using Anemoi.Contract.MasterData.ModelIds;

namespace Anemoi.MasterData.Domain.Models;

public sealed class Province : ValueObject
{
    public ProvinceId Id { get; set; }
    public string Name { get; set; }
    public string Slug { get; set; }
    public string SearchHint { get; set; }
    public int Priority { get; set; }
    public List<District> Districts { get; set; }
    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Name;
    }
}