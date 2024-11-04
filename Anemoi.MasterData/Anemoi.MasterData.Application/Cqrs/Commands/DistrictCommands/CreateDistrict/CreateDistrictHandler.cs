using Anemoi.BuildingBlock.Application.Abstractions;
using Anemoi.BuildingBlock.Application.Cqrs.Commands.CommandFlow.CommandOneFlow;
using Anemoi.BuildingBlock.Application.Results;
using Anemoi.BuildingBlock.Infrastructure.RequestHandlers.Commands.EntityFramework.EfCommandOne;
using Anemoi.Contract.MasterData.Commands.DistrictCommands.CreateDistrict;
using Anemoi.Contract.MasterData.Errors;
using Anemoi.MasterData.Domain.Models;
using AutoMapper;
using Serilog;

namespace Anemoi.MasterData.Application.Cqrs.Commands.DistrictCommands.CreateDistrict;

public sealed class CreateDistrictHandler(
    ISqlRepository<District> sqlRepository,
    IUnitOfWork unitOfWork,
    IMapper mapper,
    ILogger logger)
    : EfCommandOneVoidHandler<District, CreateDistrictCommand>(sqlRepository, unitOfWork, mapper, logger)
{
    protected override ICommandOneFlowBuilderVoid<District> BuildCommand(
        IStartOneCommandVoid<District> fromFlow, CreateDistrictCommand command,
        CancellationToken cancellationToken)
        => fromFlow
            .CreateOne(Mapper.Map<District>(command))
            .WithCondition(_ => None.Value)
            .WithErrorIfSaveChange(MasterDataErrorDetail.DistrictError.CreateFailed());
}