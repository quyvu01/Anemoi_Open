using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Anemoi.BuildingBlock.Application.Abstractions;
using Anemoi.BuildingBlock.Application.Cqrs.Commands;
using Anemoi.BuildingBlock.Application.Cqrs.Commands.CommandFlow.CommandOneFlow;
using Anemoi.BuildingBlock.Application.Extensions;
using Anemoi.BuildingBlock.Application.Responses;
using Anemoi.BuildingBlock.Application.Results;
using AutoMapper;
using OneOf;
using Serilog;

namespace Anemoi.BuildingBlock.Infrastructure.RequestHandlers.Commands.EntityFramework.EfCommandOne;

public abstract class EfCommandOneVoidHandler<TModel, TCommand>(
    ISqlRepository<TModel> sqlRepository,
    IUnitOfWork unitOfWork,
    IMapper mapper,
    ILogger logger)
    : ICommandHandler<TCommand, OneOf<None, ErrorDetailResponse>>
    where TModel : class
    where TCommand : class, ICommand<OneOf<None, ErrorDetailResponse>>
{
    protected IMapper Mapper { get; } = mapper;
    protected ISqlRepository<TModel> SqlRepository { get; } = sqlRepository;
    protected IUnitOfWork UnitOfWork { get; } = unitOfWork;
    protected ILogger Logger { get; } = logger;
    private readonly IStartOneCommandVoid<TModel> _startManyCommandFlow = new CommandOneVoidFlow<TModel>();

    public virtual async Task<OneOf<None, ErrorDetailResponse>> Handle(TCommand request,
        CancellationToken cancellationToken)
    {
        Logger.Information("Start command {@RequestType}: {@Request}", request.GetType().Name, request);
        var buildResult = BuildCommand(_startManyCommandFlow, request, cancellationToken);
        var commandType = buildResult.CommandTypeOne;
        TModel model;
        switch (commandType)
        {
            case CommandTypeOne.Create:
                var createItem = await buildResult.ModelCreateFunc.Invoke();
                var createManyCondition = await buildResult.CommandOneCondition.Invoke(createItem);
                if (createManyCondition.IsT1) return createManyCondition.AsT1.ToErrorDetailResponse();
                model = createItem;
                await SqlRepository.CreateOneAsync(createItem, token: cancellationToken);
                break;
            case CommandTypeOne.Update:
                var updateItem = await SqlRepository
                    .GetFirstByConditionAsync(buildResult.CommandFilter, buildResult.CommandSpecialAction,
                        token: cancellationToken);
                if (updateItem is null) return buildResult.NullErrorDetail.ToErrorDetailResponse();
                var updateOneCondition = await buildResult.CommandOneCondition.Invoke(updateItem);
                if (updateOneCondition.IsT1) return updateOneCondition.AsT1.ToErrorDetailResponse();
                await buildResult.UpdateOneFunc.Invoke(updateItem);
                model = updateItem;
                break;
            case CommandTypeOne.Remove:
                var removeItem = await SqlRepository.GetFirstByConditionAsync(buildResult.CommandFilter,
                    buildResult.CommandSpecialAction, token: cancellationToken);
                if (removeItem is null) return Mapper.Map<ErrorDetailResponse>(buildResult.NullErrorDetail);
                var removeOneCondition = await buildResult.CommandOneCondition.Invoke(removeItem);
                if (removeOneCondition.IsT1) return Mapper.Map<ErrorDetailResponse>(removeOneCondition.AsT1);
                await SqlRepository.RemoveOneAsync(removeItem, cancellationToken);
                model = removeItem;
                break;
            case CommandTypeOne.Unknown:
            default:
                throw new UnreachableException($"Command {commandType.GetName()} does not support!");
        }

        var saveResult = await UnitOfWork.SaveChangesAsync(cancellationToken);
        if (saveResult.IsT1)
            return Mapper.Map<ErrorDetailResponse>(buildResult.SaveChangesErrorDetail);
        await AfterSaveChangesAsync(request, model, cancellationToken);
        return None.Value;
    }

    protected virtual Task AfterSaveChangesAsync(TCommand command, TModel model,
        CancellationToken cancellationToken) => Task.CompletedTask;

    protected abstract ICommandOneFlowBuilderVoid<TModel> BuildCommand(
        IStartOneCommandVoid<TModel> fromFlow, TCommand command, CancellationToken cancellationToken);
}