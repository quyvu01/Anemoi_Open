using Anemoi.Contract.Secure.Queries.OtpQueries.GenerateOtp;
using Anemoi.Contract.Secure.Queries.OtpQueries.VerifyOtp;
using Anemoi.Contract.Secure.Responses;
using Anemoi.Secure.Application.Abstractions;
using MassTransit;

namespace Anemoi.Secure.Application.EventDriven.QueryHandlers;

public sealed class OtpHandlers(IOtpService otpService) :
    IConsumer<GenerateOtpQuery>,
    IConsumer<VerifyOtpQuery>
{
    public async Task Consume(ConsumeContext<GenerateOtpQuery> context)
    {
        var message = context.Message;
        var otp = await otpService.GenerateOtpAsync(message.PhoneNumber);
        await context.RespondAsync(new OtpGeneratedResponse { Otp = otp });
    }

    public async Task Consume(ConsumeContext<VerifyOtpQuery> context)
    {
        var message = context.Message;
        var isOtpValid = await otpService.ValidateOtpAsync(message.PhoneNumber, message.Otp);
        await context.RespondAsync(new OtpVerificationResponse { IsSucceed = isOtpValid });
    }
}