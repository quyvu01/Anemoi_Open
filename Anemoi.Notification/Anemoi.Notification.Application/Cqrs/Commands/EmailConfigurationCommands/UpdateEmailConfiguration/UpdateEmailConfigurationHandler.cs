using System.Text;
using Anemoi.BuildingBlock.Application.Abstractions;
using Anemoi.BuildingBlock.Application.Cqrs.Commands.CommandFlow.CommandOneFlow;
using Anemoi.BuildingBlock.Application.Responses;
using Anemoi.BuildingBlock.Application.Results;
using Anemoi.BuildingBlock.Infrastructure.RequestHandlers.Commands.EntityFramework.EfCommandOne;
using Anemoi.Contract.Notification.Commands.EmailConfigurationCommands.UpdateEmailConfiguration;
using Anemoi.Contract.Notification.Errors;
using Anemoi.Contract.Secure.Queries.GetEncryptData;
using Anemoi.Contract.Secure.Responses;
using AutoMapper;
using MassTransit;
using Serilog;
using Anemoi.Notification.Domain.Models;

namespace Anemoi.Notification.Application.Cqrs.Commands.EmailConfigurationCommands.UpdateEmailConfiguration;

public sealed class UpdateEmailConfigurationHandler(
    ISqlRepository<EmailConfiguration> sqlRepository,
    IUnitOfWork unitOfWork,
    IMapper mapper,
    ILogger logger,
    IRequestClient<GetEncryptDataQuery> encryptDataClient)
    : EfCommandOneVoidHandler<EmailConfiguration, UpdateEmailConfigurationCommand>(sqlRepository, unitOfWork, mapper,
        logger)
{
    protected override ICommandOneFlowBuilderVoid<EmailConfiguration> BuildCommand(
        IStartOneCommandVoid<EmailConfiguration> fromFlow, UpdateEmailConfigurationCommand command,
        CancellationToken cancellationToken)
        => fromFlow
            .UpdateOne(a => a.Id == command.Id)
            .WithSpecialAction(null)
            .WithCondition(_ => None.Value)
            .WithModify(async existOne =>
            {
                Mapper.Map(command, existOne);
                if (!string.IsNullOrEmpty(command.Password))
                {
                    var passwordBytesResponse = await encryptDataClient
                        .GetResponse<CryptographyResponse, ErrorDetailResponse>(
                            new GetEncryptDataQuery(Encoding.UTF8.GetBytes(command.Password)), cancellationToken);
                    if (passwordBytesResponse.Is(out Response<CryptographyResponse> data))
                        existOne.PasswordBytes = data.Message.DecryptData;
                }

                if (command.IsDefault == true)
                {
                    var removeDefault = await SqlRepository.GetManyByConditionAsync(
                        x => x.WorkspaceId == existOne.WorkspaceId && x.Id != existOne.Id && x.IsDefault,
                        token: cancellationToken);
                    removeDefault.ForEach(a => a.IsDefault = false);
                }
            })
            .WithErrorIfNull(NotificationErrorDetail.EmailConfigurationError.NotFound())
            .WithErrorIfSaveChange(NotificationErrorDetail.EmailConfigurationError.UpdateFailed());
}