using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using OneOf;
using Serilog;
using Anemoi.BuildingBlock.Application.Abstractions;
using Anemoi.BuildingBlock.Application.Results;

namespace Anemoi.BuildingBlock.Infrastructure.Repositories;

public class EfUnitOfWork : IUnitOfWork
{
    private readonly DbContext _dbContext;
    private readonly ILogger _logger;

    protected EfUnitOfWork(DbContext dbContext, ILogger logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task<OneOf<None, Exception>> SaveChangesAsync(CancellationToken token = default)
    {
        _logger.Information("Save context async!");
        try
        {
            await _dbContext.SaveChangesAsync(token);
            return None.Value;
        }
        catch (Exception e)
        {
            _logger.Error("Error while save changes using dbContext: {Error}", e.Message);
            return e;
        }
    }
}