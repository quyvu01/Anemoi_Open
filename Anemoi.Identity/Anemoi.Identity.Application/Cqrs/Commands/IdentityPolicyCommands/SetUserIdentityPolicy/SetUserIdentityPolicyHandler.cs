using System.Linq;
using System.Security.Claims;
using System.Threading;
using Anemoi.BuildingBlock.Application.Abstractions;
using Anemoi.BuildingBlock.Application.Cqrs.Commands.CommandFlow.CommandOneFlow;
using Anemoi.BuildingBlock.Application.Results;
using Anemoi.BuildingBlock.Infrastructure.RequestHandlers.Commands.EntityFramework.EfCommandOne;
using Anemoi.Contract.Identity.Commands.IdentityPolicyCommands.SetUserIdentityPolicy;
using Anemoi.Contract.Identity.Errors;
using Anemoi.Identity.Application.Abstractions;
using Anemoi.Identity.Domain.Models;
using AutoMapper;
using Serilog;

namespace Anemoi.Identity.Application.Cqrs.Commands.IdentityPolicyCommands.SetUserIdentityPolicy;

public sealed class SetUserIdentityPolicyHandler(
    ISqlRepository<User> sqlRepository,
    ISqlRepository<IdentityPolicy> identityPolicyRepository,
    IUserClaimRepository userClaimRepository,
    IUnitOfWork unitOfWork,
    IMapper mapper,
    ILogger logger)
    : EfCommandOneVoidHandler<User, SetUserIdentityPolicyCommand>(sqlRepository, unitOfWork, mapper, logger)
{
    protected override ICommandOneFlowBuilderVoid<User> BuildCommand(IStartOneCommandVoid<User> fromFlow,
        SetUserIdentityPolicyCommand command,
        CancellationToken cancellationToken)
        => fromFlow
            .UpdateOne(x => x.UserId == command.UserId)
            .WithSpecialAction(null)
            .WithCondition(_ => None.Value)
            .WithModify(async user =>
            {
                var existPolicy = await identityPolicyRepository
                    .GetFirstByConditionAsync(x => x.Id == command.IdentityPolicyId, token: cancellationToken);
                if (existPolicy is null) return;
                var claims = await userClaimRepository
                    .GetUserClaimsAsync(user.UserId, CancellationToken.None);
                if (claims.Any(x => x.Type == existPolicy.Key && x.Value == existPolicy.Value)) return;
                await userClaimRepository.AddClaimsAsync(user.UserId,
                    [new Claim(existPolicy.Key, existPolicy.Value)], CancellationToken.None);
            })
            .WithErrorIfNull(IdentityErrorDetail.UserError.NotFound())
            .WithErrorIfSaveChange(IdentityErrorDetail.UserError.UpdateFailed());
}