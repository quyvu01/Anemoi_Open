using Anemoi.BuildingBlock.Application.Cqrs.Queries;
using Anemoi.Contract.Secure.Responses;

namespace Anemoi.Contract.Secure.Queries.OtpQueries.VerifyOtp;

public sealed record VerifyOtpQuery(string PhoneNumber, string Otp) : IQueryOne<OtpVerificationResponse>;