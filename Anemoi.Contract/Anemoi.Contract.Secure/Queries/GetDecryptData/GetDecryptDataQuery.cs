using Anemoi.BuildingBlock.Application.Cqrs.Queries;
using Anemoi.Contract.Secure.Responses;

namespace Anemoi.Contract.Secure.Queries.GetDecryptData;

public sealed record GetDecryptDataQuery(byte[] EncryptBytes) : IQueryOne<CryptographyResponse>;