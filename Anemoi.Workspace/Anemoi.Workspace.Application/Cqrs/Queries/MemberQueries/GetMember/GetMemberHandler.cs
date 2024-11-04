using Anemoi.BuildingBlock.Application.Abstractions;
using Anemoi.BuildingBlock.Application.Cqrs.Queries.QueryFlow.QueryOneFlow;
using Anemoi.BuildingBlock.Infrastructure.RequestHandlers.Queries.EntityFramework.EfQueryOne;
using Anemoi.Contract.Workspace.Errors;
using Anemoi.Contract.Workspace.Queries.MemberQueries.GetMember;
using Anemoi.Contract.Workspace.Responses;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Serilog;
using Anemoi.Workspace.Domain.Models;

namespace Anemoi.Workspace.Application.Cqrs.Queries.MemberQueries.GetMember;

public sealed class GetMemberHandler(ISqlRepository<Member> sqlRepository, IMapper mapper, ILogger logger)
    : EfQueryOneHandler<Member, GetMemberQuery, MemberResponse>(sqlRepository,
        mapper, logger)
{
    protected override IQueryOneFlowBuilder<Member, MemberResponse> BuildQueryFlow(
        IQueryOneFilter<Member, MemberResponse> fromFlow, GetMemberQuery query)
        => fromFlow
            .WithFilter(x => x.Id == query.Id)
            .WithSpecialAction(x => x.ProjectTo<MemberResponse>(Mapper.ConfigurationProvider))
            .WithErrorIfNull(WorkspaceErrorDetail.MemberError.NotFound());
}