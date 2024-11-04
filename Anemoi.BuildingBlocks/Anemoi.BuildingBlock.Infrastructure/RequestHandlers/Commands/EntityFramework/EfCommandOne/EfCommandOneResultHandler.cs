using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Anemoi.BuildingBlock.Application.Abstractions;
using Anemoi.BuildingBlock.Application.Cqrs.Commands;
using Anemoi.BuildingBlock.Application.Cqrs.Commands.CommandFlow.CommandOneFlow;
using Anemoi.BuildingBlock.Application.Extensions;
using Anemoi.BuildingBlock.Application.Responses;
using AutoMapper;
using OneOf;
using Serilog;

namespace Anemoi.BuildingBlock.Infrastructure.RequestHandlers.Commands.EntityFramework.EfCommandOne;

public abstract class EfCommandOneResultHandler<TModel, TCommand, TResult>(
    ISqlRepository<TModel> sqlRepository,
    IUnitOfWork unitOfWork,
    IMapper mapper,
    ILogger logger)
    : ICommandHandler<TCommand, OneOf<TResult, ErrorDetailResponse>>
    where TModel : class
    where TCommand : class, ICommand<OneOf<TResult, ErrorDetailResponse>>
{
    protected IMapper Mapper { get; } = mapper;
    protected ISqlRepository<TModel> SqlRepository { get; } = sqlRepository;
    protected IUnitOfWork UnitOfWork { get; } = unitOfWork;
    protected ILogger Logger { get; } = logger;

    private readonly IStartOneCommandResult<TModel, TResult> _startManyCommandFlow =
        new CommandOneResultFlow<TModel, TResult>();

    public virtual async Task<OneOf<TResult, ErrorDetailResponse>> Handle(TCommand request,
        CancellationToken cancellationToken)
    {
        Logger.Information("Start command {@RequestType}: {@Request}", request.GetType().Name, request);
        var buildResult = BuildCommand(_startManyCommandFlow, request, cancellationToken);
        var commandType = buildResult.CommandTypeOne;
        TModel item;
        switch (commandType)
        {
            case CommandTypeOne.Create:
                var createItem = await buildResult.ModelCreateFunc.Invoke();
                var createOneCondition = await buildResult.CommandOneCondition.Invoke(createItem);
                if (createOneCondition.IsT1) return createOneCondition.AsT1.ToErrorDetailResponse();
                await SqlRepository.CreateOneAsync(createItem, token: cancellationToken);
                item = createItem;
                break;
            case CommandTypeOne.Update:
                var updateItem = await SqlRepository
                    .GetFirstByConditionAsync(buildResult.CommandFilter, buildResult.CommandSpecialAction,
                        token: cancellationToken);
                var updateManyCondition = await buildResult.CommandOneCondition.Invoke(updateItem);
                if (updateManyCondition.IsT1) return updateManyCondition.AsT1.ToErrorDetailResponse();
                await buildResult.UpdateOneFunc.Invoke(updateItem);
                item = updateItem;
                break;
            case CommandTypeOne.Remove:
                var removeItem = await SqlRepository.GetFirstByConditionAsync(buildResult.CommandFilter,
                    buildResult.CommandSpecialAction, token: cancellationToken);
                var removeManyCondition = await buildResult.CommandOneCondition.Invoke(removeItem);
                if (removeManyCondition.IsT1) return removeManyCondition.AsT1.ToErrorDetailResponse();
                await SqlRepository.RemoveOneAsync(removeItem, cancellationToken);
                item = removeItem;
                break;
            case CommandTypeOne.Unknown:
            default:
                throw new UnreachableException($"Command {commandType.GetName()} does not support!");
        }

        var saveResult = await UnitOfWork.SaveChangesAsync(cancellationToken);
        if (saveResult.IsT1)
            return buildResult.SaveChangesErrorDetail.ToErrorDetailResponse();
        var result = buildResult.ResultFunc.Invoke(item);
        await AfterSaveChangesAsync(request, item, result, cancellationToken);
        return result;
    }

    protected virtual Task AfterSaveChangesAsync(TCommand command, TModel model, TResult result,
        CancellationToken cancellationToken) => Task.CompletedTask;

    protected abstract ICommandOneFlowBuilderResult<TModel, TResult> BuildCommand(
        IStartOneCommandResult<TModel, TResult> fromFlow, TCommand command, CancellationToken cancellationToken);
}