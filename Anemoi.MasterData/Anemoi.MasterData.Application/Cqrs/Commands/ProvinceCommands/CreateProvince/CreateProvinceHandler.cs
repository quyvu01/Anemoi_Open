using Anemoi.BuildingBlock.Application.Abstractions;
using Anemoi.BuildingBlock.Application.Cqrs.Commands.CommandFlow.CommandOneFlow;
using Anemoi.BuildingBlock.Application.Results;
using Anemoi.BuildingBlock.Infrastructure.RequestHandlers.Commands.EntityFramework.EfCommandOne;
using Anemoi.Contract.MasterData.Commands.ProvinceCommands.CreateProvince;
using Anemoi.Contract.MasterData.Errors;
using Anemoi.MasterData.Domain.Models;
using AutoMapper;
using Serilog;

namespace Anemoi.MasterData.Application.Cqrs.Commands.ProvinceCommands.CreateProvince;

public sealed class CreateProvinceHandler(
    ISqlRepository<Province> sqlRepository,
    IUnitOfWork unitOfWork,
    IMapper mapper,
    ILogger logger)
    : EfCommandOneVoidHandler<Province, CreateProvinceCommand>(sqlRepository, unitOfWork, mapper, logger)
{
    protected override ICommandOneFlowBuilderVoid<Province> BuildCommand(
        IStartOneCommandVoid<Province> fromFlow, CreateProvinceCommand command,
        CancellationToken cancellationToken) => fromFlow
        .CreateOne(Mapper.Map<Province>(command))
        .WithCondition(_ => None.Value)
        .WithErrorIfSaveChange(MasterDataErrorDetail.ProvinceError.CreateFailed());
}