using Anemoi.BuildingBlock.Application.Cqrs.Commands;
using Anemoi.Contract.MasterData.ModelIds;
using Newtonsoft.Json;

namespace Anemoi.Contract.MasterData.Commands.DistrictCommands.UpdateDistrict;

public sealed record UpdateDistrictCommand([property: JsonIgnore] DistrictId Id, ProvinceId ProvinceId,
    string Name) : ICommandVoid;