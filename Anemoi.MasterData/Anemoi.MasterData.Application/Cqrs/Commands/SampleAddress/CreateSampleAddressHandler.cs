using Anemoi.BuildingBlock.Application.Abstractions;
using Anemoi.BuildingBlock.Application.Cqrs.Commands.CommandFlow.CommandManyFlow;
using Anemoi.BuildingBlock.Application.Extensions;
using Anemoi.BuildingBlock.Application.Helpers;
using Anemoi.BuildingBlock.Application.Results;
using Anemoi.BuildingBlock.Infrastructure.RequestHandlers.Commands.EntityFramework.EfCommandMany;
using Anemoi.Contract.MasterData.Commands.SampleAddressCommands.CreateSampleAddress;
using Anemoi.Contract.MasterData.Errors;
using Anemoi.Contract.MasterData.ModelIds;
using Anemoi.MasterData.Application.Configurations;
using Anemoi.MasterData.Domain.Models;
using AutoMapper;
using Newtonsoft.Json;
using Serilog;

namespace Anemoi.MasterData.Application.Cqrs.Commands.SampleAddress;

public sealed class CreateSampleAddressHandler(
    ISqlRepository<Province> sqlRepository,
    IUnitOfWork unitOfWork,
    IMapper mapper,
    ILogger logger,
    ISqlRepository<Province> provinceRepository)
    : EfCommandManyVoidHandler<Province, CreateSampleAddressCommand>(sqlRepository, unitOfWork, mapper, logger)
{
    protected override ICommandManyFlowBuilderVoid<Province> BuildCommand(
        IStartManyCommandVoid<Province> fromFlow, CreateSampleAddressCommand command,
        CancellationToken cancellationToken)
        => fromFlow
            .CreateMany(async () =>
            {
                var rawTextData = await File.ReadAllTextAsync("./Assets/RawVnAddress.json", cancellationToken);
                var rawAddressData = JsonConvert.DeserializeObject<List<RawAddress>>(rawTextData);
                return rawAddressData.GroupBy(x => x.ProvinceName)
                    .Where(x => x.Key is { })
                    .Select(x =>
                    {
                        var provinceId = new ProvinceId(IdGenerator.NextGuid());
                        return new Province
                        {
                            Id = provinceId, Name = x.Key, Slug = x.Key.GenerateSlug(),
                            SearchHint = x.Key.GenerateSearchHint(),
                            Districts = x.GroupBy(k => k.DistrictName)
                                .Where(k => k.Key is { })
                                .Select(d =>
                                {
                                    var districtId = new DistrictId(IdGenerator.NextGuid());
                                    return new District
                                    {
                                        Id = districtId, Name = d.Key,
                                        ProvinceId = provinceId,
                                        Slug = d.Key.GenerateSlug(),
                                        SearchHint = d.Key.GenerateSearchHint()
                                    };
                                }).ToList()
                        };
                    }).ToList();
            })
            .WithCondition(async _ =>
            {
                var isDataInit = await provinceRepository
                    .ExistByConditionAsync(_ => true, cancellationToken);
                if (isDataInit) return MasterDataErrorDetail.ProvinceError.AlreadyExist();
                return None.Value;
            })
            .WithErrorIfSaveChange(MasterDataErrorDetail.ProvinceError.CreateFailed());
}