using System.Text;
using Anemoi.BuildingBlock.Application.Abstractions;
using Anemoi.BuildingBlock.Application.Cqrs.Commands.CommandFlow.CommandOneFlow;
using Anemoi.BuildingBlock.Application.Responses;
using Anemoi.BuildingBlock.Application.Results;
using Anemoi.BuildingBlock.Infrastructure.RequestHandlers.Commands.EntityFramework.EfCommandOne;
using Anemoi.Contract.Notification.Commands.EmailConfigurationCommands.CreateEmailConfiguration;
using Anemoi.Contract.Notification.Errors;
using Anemoi.Contract.Secure.Queries.GetEncryptData;
using Anemoi.Contract.Secure.Responses;
using Anemoi.Notification.Application.Abstractions;
using AutoMapper;
using MassTransit;
using Serilog;
using Anemoi.Notification.Domain.Models;

namespace Anemoi.Notification.Application.Cqrs.Commands.EmailConfigurationCommands.CreateEmailConfiguration;

public sealed class CreateEmailConfigurationHandler(
    ISqlRepository<EmailConfiguration> sqlRepository,
    IUnitOfWork unitOfWork,
    IMapper mapper,
    ILogger logger,
    IWorkspaceIdGetter workspaceIdGetter,
    IRequestClient<GetEncryptDataQuery> encryptDataClient,
    IEmailService emailService)
    : EfCommandOneVoidHandler<EmailConfiguration, CreateEmailConfigurationCommand>(sqlRepository,
        unitOfWork, mapper, logger)
{
    protected override ICommandOneFlowBuilderVoid<EmailConfiguration> BuildCommand(
        IStartOneCommandVoid<EmailConfiguration> fromFlow, CreateEmailConfigurationCommand command,
        CancellationToken cancellationToken)
        => fromFlow
            .CreateOne(async () =>
            {
                if (command.IsDefault)
                {
                    var removeDefault = await SqlRepository.GetManyByConditionAsync(
                        x => x.WorkspaceId == workspaceIdGetter.WorkspaceId,
                        token: cancellationToken);
                    removeDefault.ForEach(a => a.IsDefault = false);
                }

                var newOne = Mapper.Map<EmailConfiguration>(command);
                newOne.WorkspaceId = workspaceIdGetter.WorkspaceId;
                var getList = await SqlRepository.GetManyByConditionAsync(
                    x => x.WorkspaceId == workspaceIdGetter.WorkspaceId,
                    token: cancellationToken);
                if (getList.Count == 0) newOne.IsDefault = true;
                var passwordBytesResponse = await encryptDataClient
                    .GetResponse<CryptographyResponse, ErrorDetailResponse>(
                        new GetEncryptDataQuery(Encoding.UTF8.GetBytes(command.Password)), cancellationToken);
                if (passwordBytesResponse.Is(out Response<CryptographyResponse> data))
                    newOne.PasswordBytes = data.Message.DecryptData;
                return newOne;
            })
            .WithCondition(async _ =>
            {
                var authenticateResult = await emailService
                    .AuthenticateSmtpAsync(command.Host, command.Port, command.UserName, command.Password,
                        cancellationToken);
                if (authenticateResult.IsT1)
                    return NotificationErrorDetail.EmailConfigurationError.AuthenticateFailed();
                return None.Value;
            })
            .WithErrorIfSaveChange(NotificationErrorDetail.EmailConfigurationError.CreateFailed());
}