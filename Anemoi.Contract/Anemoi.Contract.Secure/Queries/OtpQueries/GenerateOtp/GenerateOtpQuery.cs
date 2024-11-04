using Anemoi.BuildingBlock.Application.Cqrs.Queries;
using Anemoi.Contract.Secure.Responses;

namespace Anemoi.Contract.Secure.Queries.OtpQueries.GenerateOtp;

public sealed record GenerateOtpQuery(string PhoneNumber) : IQuery<OtpGeneratedResponse>;