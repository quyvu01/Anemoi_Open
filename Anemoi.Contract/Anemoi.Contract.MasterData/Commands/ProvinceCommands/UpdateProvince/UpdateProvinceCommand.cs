using Anemoi.BuildingBlock.Application.Cqrs.Commands;
using Anemoi.Contract.MasterData.ModelIds;
using Newtonsoft.Json;

namespace Anemoi.Contract.MasterData.Commands.ProvinceCommands.UpdateProvince;

public sealed record UpdateProvinceCommand([property: JsonIgnore] ProvinceId Id, string Name) : ICommandVoid;