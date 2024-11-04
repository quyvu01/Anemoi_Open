using Anemoi.BuildingBlock.Application.Abstractions;
using Anemoi.BuildingBlock.Application.Cqrs.Queries.QueryFlow.QueryManyFlow;
using Anemoi.BuildingBlock.Application.Queries;
using Anemoi.BuildingBlock.Infrastructure.RequestHandlers.Queries.EntityFramework.EfQueryMany;
using Anemoi.Contract.Workspace.Queries.MemberQueries.GetMembersByIds;
using Anemoi.Contract.Workspace.Responses;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Serilog;
using Anemoi.Workspace.Domain.Models;

namespace Anemoi.Workspace.Application.Cqrs.Queries.MemberQueries.GetMembersByIds;

public sealed class GetMembersByIdsHandler(ISqlRepository<Member> sqlRepository, IMapper mapper, ILogger logger) :
    EfQueryCollectionHandler<Member, GetMembersByIdsQuery, MemberResponse>(sqlRepository, mapper, logger)
{
    protected override IQueryListFlowBuilder<Member, MemberResponse> BuildQueryFlow(
        IQueryListFilter<Member, MemberResponse> fromFlow, GetMembersByIdsQuery query)
        => fromFlow
            .WithFilter(a => query.Ids.Contains(a.Id))
            .WithSpecialAction(a => a.ProjectTo<MemberResponse>(Mapper.ConfigurationProvider))
            .WithSortFieldWhenNotSet(x => x.CreatedTime)
            .WithSortedDirectionWhenNotSet(SortedDirection.Descending);
}