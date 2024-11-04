using Anemoi.BuildingBlock.Application.Cqrs.Commands;
using Anemoi.Contract.MasterData.ModelIds;

namespace Anemoi.Contract.MasterData.Commands.DistrictCommands.CreateDistrict;

public sealed record CreateDistrictCommand(ProvinceId ProvinceId, string Name) : ICommandVoid;