using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Anemoi.BuildingBlock.Application.Abstractions;
using Anemoi.BuildingBlock.Application.Cqrs.Commands;
using Anemoi.BuildingBlock.Application.Cqrs.Commands.CommandFlow.CommandManyFlow;
using Anemoi.BuildingBlock.Application.Extensions;
using Anemoi.BuildingBlock.Application.Responses;
using Anemoi.BuildingBlock.Application.Results;
using AutoMapper;
using OneOf;
using Serilog;

namespace Anemoi.BuildingBlock.Infrastructure.RequestHandlers.Commands.EntityFramework.EfCommandMany;

public abstract class EfCommandManyVoidHandler<TModel, TCommand>(
    ISqlRepository<TModel> sqlRepository,
    IUnitOfWork unitOfWork,
    IMapper mapper,
    ILogger logger)
    :
        ICommandHandler<TCommand, OneOf<None, ErrorDetailResponse>>
    where TModel : class
    where TCommand : class, ICommand<OneOf<None, ErrorDetailResponse>>
{
    protected IMapper Mapper { get; } = mapper;
    protected ISqlRepository<TModel> SqlRepository { get; } = sqlRepository;
    protected IUnitOfWork UnitOfWork { get; } = unitOfWork;
    protected ILogger Logger { get; } = logger;
    private readonly IStartManyCommandVoid<TModel> _startManyCommandFlow = new CommandManyWithVoidFlow<TModel>();

    public virtual async Task<OneOf<None, ErrorDetailResponse>> Handle(TCommand request,
        CancellationToken cancellationToken)
    {
        Logger.Information("Start command {@RequestType}: {@Request}", request.GetType().Name, request);
        var buildResult = BuildCommand(_startManyCommandFlow, request, cancellationToken);
        var commandType = buildResult.CommandTypeMany;
        List<TModel> models;
        switch (commandType)
        {
            case CommandTypeMany.Create:
                var createItems = await buildResult.ModelsCreateFunc.Invoke();
                var createManyCondition = await buildResult.CommandManyCondition.Invoke(createItems);
                if (createManyCondition.IsT1) return createManyCondition.AsT1.ToErrorDetailResponse();
                await SqlRepository.CreateManyAsync(createItems, token: cancellationToken);
                models = createItems;
                break;
            case CommandTypeMany.Update:
                var updateItems = await SqlRepository
                    .GetManyByConditionAsync(buildResult.CommandFilter, buildResult.CommandSpecialAction,
                        token: cancellationToken);
                var updateManyCondition = await buildResult.CommandManyCondition.Invoke(updateItems);
                if (updateManyCondition.IsT1) return Mapper.Map<ErrorDetailResponse>(updateManyCondition.AsT1);
                await buildResult.UpdateManyFunc.Invoke(updateItems);
                models = updateItems;
                break;
            case CommandTypeMany.Remove:
                var removeItems = await SqlRepository.GetManyByConditionAsync(buildResult.CommandFilter,
                    buildResult.CommandSpecialAction, token: cancellationToken);
                var removeManyCondition = await buildResult.CommandManyCondition.Invoke(removeItems);
                if (removeManyCondition.IsT1) return Mapper.Map<ErrorDetailResponse>(removeManyCondition.AsT1);
                await SqlRepository.RemoveManyAsync(removeItems, cancellationToken);
                models = removeItems;
                break;
            case CommandTypeMany.Unknown:
            default:
                throw new UnreachableException($"Command {commandType.GetName()} does not support!");
        }

        var saveResult = await UnitOfWork.SaveChangesAsync(cancellationToken);
        if (saveResult.IsT1)
            return Mapper.Map<ErrorDetailResponse>(buildResult.SaveChangesErrorDetail);
        await AfterSaveChangesAsync(request, models, cancellationToken);
        return None.Value;
    }

    protected virtual Task AfterSaveChangesAsync(TCommand command,
        List<TModel> models, CancellationToken cancellationToken) => Task.CompletedTask;

    protected abstract ICommandManyFlowBuilderVoid<TModel> BuildCommand(
        IStartManyCommandVoid<TModel> fromFlow, TCommand command, CancellationToken cancellationToken);
}