using Anemoi.BuildingBlock.Application.Cqrs.Queries;
using Anemoi.Contract.Secure.Responses;

namespace Anemoi.Contract.Secure.Queries.GetEncryptData;

public sealed record GetEncryptDataQuery(byte[] RawBytesData) : IQueryOne<CryptographyResponse>;