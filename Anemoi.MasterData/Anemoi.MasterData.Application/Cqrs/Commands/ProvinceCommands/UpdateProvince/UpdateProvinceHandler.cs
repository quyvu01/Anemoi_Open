using Anemoi.BuildingBlock.Application.Abstractions;
using Anemoi.BuildingBlock.Application.Cqrs.Commands.CommandFlow.CommandOneFlow;
using Anemoi.BuildingBlock.Application.Results;
using Anemoi.BuildingBlock.Infrastructure.RequestHandlers.Commands.EntityFramework.EfCommandOne;
using Anemoi.Contract.MasterData.Commands.ProvinceCommands.UpdateProvince;
using Anemoi.Contract.MasterData.Errors;
using Anemoi.MasterData.Domain.Models;
using AutoMapper;
using Serilog;

namespace Anemoi.MasterData.Application.Cqrs.Commands.ProvinceCommands.UpdateProvince;

public sealed class UpdateProvinceHandler(
    ISqlRepository<Province> sqlRepository,
    IUnitOfWork unitOfWork,
    IMapper mapper,
    ILogger logger)
    : EfCommandOneVoidHandler<Province, UpdateProvinceCommand>(sqlRepository, unitOfWork, mapper, logger)
{
    protected override ICommandOneFlowBuilderVoid<Province> BuildCommand(
        IStartOneCommandVoid<Province> fromFlow, UpdateProvinceCommand command,
        CancellationToken cancellationToken)
        => fromFlow
            .UpdateOne(x => x.Id == command.Id)
            .WithSpecialAction(null)
            .WithCondition(_ => None.Value)
            .WithModify(province => Mapper.Map(command, province))
            .WithErrorIfNull(MasterDataErrorDetail.ProvinceError.NotFound())
            .WithErrorIfSaveChange(MasterDataErrorDetail.ProvinceError.UpdateFailed());
}