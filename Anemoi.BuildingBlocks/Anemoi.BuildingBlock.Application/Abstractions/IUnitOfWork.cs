using System;
using System.Threading;
using System.Threading.Tasks;
using Anemoi.BuildingBlock.Application.Results;
using OneOf;

namespace Anemoi.BuildingBlock.Application.Abstractions;

public interface IUnitOfWork
{
    Task<OneOf<None, Exception>> SaveChangesAsync(CancellationToken token = default);
}