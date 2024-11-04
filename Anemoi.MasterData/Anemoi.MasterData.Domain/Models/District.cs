using Anemoi.BuildingBlock.Domain;
using Anemoi.Contract.MasterData.ModelIds;

namespace Anemoi.MasterData.Domain.Models;

public sealed class District : ValueObject
{
    public DistrictId Id { get; set; }
    public ProvinceId ProvinceId { get; set; }
    public Province Province { get; set; }
    public string Name { get; set; }
    public string Slug { get; set; }
    public string SearchHint { get; set; }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Name;
    }
}