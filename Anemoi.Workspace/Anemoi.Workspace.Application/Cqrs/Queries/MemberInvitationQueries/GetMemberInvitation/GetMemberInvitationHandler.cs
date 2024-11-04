using Anemoi.BuildingBlock.Application.Abstractions;
using Anemoi.BuildingBlock.Application.Cqrs.Queries.QueryFlow.QueryOneFlow;
using Anemoi.BuildingBlock.Infrastructure.RequestHandlers.Queries.EntityFramework.EfQueryOne;
using Anemoi.Contract.Workspace.Errors;
using Anemoi.Contract.Workspace.Queries.MemberInvitationQueries.GetMemberInvitation;
using Anemoi.Contract.Workspace.Responses;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Serilog;
using Anemoi.Workspace.Domain.Models;

namespace Anemoi.Workspace.Application.Cqrs.Queries.MemberInvitationQueries.GetMemberInvitation;

public sealed class
    GetMemberInvitationHandler(ISqlRepository<MemberInvitation> sqlRepository, IMapper mapper, ILogger logger)
    : EfQueryOneHandler<MemberInvitation, GetMemberInvitationQuery, MemberInvitationResponse>(sqlRepository, mapper,
        logger)
{
    protected override IQueryOneFlowBuilder<MemberInvitation, MemberInvitationResponse> BuildQueryFlow(
        IQueryOneFilter<MemberInvitation, MemberInvitationResponse> fromFlow, GetMemberInvitationQuery query)
        => fromFlow
            .WithFilter(x => x.Id == query.Id)
            .WithSpecialAction(x => x.ProjectTo<MemberInvitationResponse>(Mapper.ConfigurationProvider))
            .WithErrorIfNull(WorkspaceErrorDetail.MemberInvitationError.NotFound());
}