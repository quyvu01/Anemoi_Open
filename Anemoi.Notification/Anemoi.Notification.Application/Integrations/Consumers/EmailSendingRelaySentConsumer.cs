using Anemoi.Orchestration.Contract.EmailSendingContract.Events.EmailSendingRelayEvents;
using MassTransit;
using Serilog;

namespace Anemoi.Notification.Application.Integrations.Consumers;

public sealed class EmailSendingRelaySentConsumer(ILogger logger) : IConsumer<EmailSendingRelaySent>
{
    public async Task Consume(ConsumeContext<EmailSendingRelaySent> context)
    {
        var message = context.Message;
        logger.Information("[EmailSendingRelaySent] message: {@Message}", context.Message);
        await context.Publish<EmailSendingRelaySentResult>(new { message.CorrelationId, IsSucceed = true });
    }
}