using Anemoi.BuildingBlock.Application.Responses;
using Anemoi.Contract.Secure.Queries.OtpQueries.GenerateOtp;
using Anemoi.Contract.Secure.Responses;
using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Anemoi.Orchestration.Contract.SecureContract.Events.OtpCreationEvents;
using Anemoi.Orchestration.Contract.SecureContract.Instances;

namespace Anemoi.Orchestrator.Machine.Activities.SecureActivities.OtpOperationActivities;

public sealed class CreatePhoneNumberOtpActivity(ILogger logger, IServiceProvider serviceProvider) :
    IStateMachineActivity<OtpOperationInstance, CreatePhoneNumberOtp>
{
    public void Probe(ProbeContext context) => context.CreateScope("create-phone-number-otp");

    public void Accept(StateMachineVisitor visitor) => visitor.Visit(this);

    public async Task Execute(BehaviorContext<OtpOperationInstance, CreatePhoneNumberOtp> context,
        IBehavior<OtpOperationInstance, CreatePhoneNumberOtp> next)
    {
        var saga = context.Saga;
        var otpClient = context.CreateRequestClient<GenerateOtpQuery>(serviceProvider.GetRequiredService<IBus>());
        var otpResult = await otpClient
            .GetResponse<OtpGeneratedResponse, ErrorDetailResponse>(new GenerateOtpQuery(saga.PhoneNumber));
        var otp = otpResult.Is(out Response<OtpGeneratedResponse> otpResponse) ? otpResponse.Message.Otp : null;
        await context.Publish<PhoneNumberOtpSent>(new { saga.CorrelationId, saga.PhoneNumber, Otp = otp });
    }

    public Task Faulted<TException>(
        BehaviorExceptionContext<OtpOperationInstance, CreatePhoneNumberOtp, TException> context,
        IBehavior<OtpOperationInstance, CreatePhoneNumberOtp> next) where TException : Exception
    {
        logger.Error("[CreatePhoneNumberOtpActivity] has been faulted: {@Error}", context.Exception.Message);
        return next.Faulted(context);
    }
}