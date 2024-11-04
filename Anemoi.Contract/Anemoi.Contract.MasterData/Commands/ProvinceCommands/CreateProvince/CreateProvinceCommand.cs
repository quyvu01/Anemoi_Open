using Anemoi.BuildingBlock.Application.Cqrs.Commands;

namespace Anemoi.Contract.MasterData.Commands.ProvinceCommands.CreateProvince;

public sealed record CreateProvinceCommand(string Name): ICommandVoid;