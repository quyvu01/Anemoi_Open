using Anemoi.BuildingBlock.Application.Cqrs.Commands;
using Anemoi.BuildingBlock.Application.Responses;
using Anemoi.BuildingBlock.Application.Results;
using Anemoi.Contract.Notification.Commands.EmailConfigurationCommands.SendTemporaryEmail;
using Anemoi.Notification.Application.Abstractions;
using OneOf;

namespace Anemoi.Notification.Application.Cqrs.Commands.EmailConfigurationCommands.SendTemporaryEmail;

public sealed class SendTemporaryEmailHandler(IEmailService emailService) :
    ICommandHandler<SendTemporaryEmailCommand, OneOf<None, ErrorDetailResponse>>
{
    public async Task<OneOf<None, ErrorDetailResponse>> Handle(SendTemporaryEmailCommand request,
        CancellationToken cancellationToken)
    {
        var testSendResult = await emailService
            .SendTemporaryEmailAsync(request.Host, request.Port, request.UserName, request.Password, request.EmailTo,
                "SendTemporaryEmail", cancellationToken);
        return testSendResult;
    }
}