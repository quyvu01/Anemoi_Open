using Anemoi.BuildingBlock.Application.Abstractions;
using Anemoi.BuildingBlock.Application.Cqrs.Commands.CommandFlow.CommandManyFlow;
using Anemoi.BuildingBlock.Application.Results;
using Anemoi.BuildingBlock.Infrastructure.RequestHandlers.Commands.EntityFramework.EfCommandMany;
using Anemoi.Contract.Notification.Commands.EmailConfigurationCommands.RemoveEmailConfigurations;
using Anemoi.Contract.Notification.Errors;
using AutoMapper;
using Serilog;
using Anemoi.Notification.Domain.Models;

namespace Anemoi.Notification.Application.Cqrs.Commands.EmailConfigurationCommands.RemoveEmailConfigurations;

public sealed class RemoveEmailConfigurationsHandler(
    ISqlRepository<EmailConfiguration> sqlRepository,
    IUnitOfWork unitOfWork,
    IMapper mapper,
    ILogger logger)
    : EfCommandManyVoidHandler<EmailConfiguration, RemoveEmailConfigurationsCommand>(sqlRepository, unitOfWork, mapper,
        logger)
{
    protected override ICommandManyFlowBuilderVoid<EmailConfiguration> BuildCommand(
        IStartManyCommandVoid<EmailConfiguration> fromFlow, RemoveEmailConfigurationsCommand command,
        CancellationToken cancellationToken)
        => fromFlow
            .RemoveMany(x => command.Ids.Contains(x.Id))
            .WithSpecialAction(null)
            .WithCondition(_ => None.Value)
            .WithErrorIfSaveChange(NotificationErrorDetail.EmailConfigurationError.RemoveFailed());
}