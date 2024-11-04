using Anemoi.BuildingBlock.Application.Abstractions;
using Anemoi.BuildingBlock.Application.Extensions;
using Anemoi.BuildingBlock.Application.Helpers;
using Anemoi.Contract.Workspace.Commands.WorkspaceCommands.CreateWorkspace;
using Anemoi.Contract.Workspace.Commands.WorkspaceCommands.UpdateWorkspace;
using Anemoi.Contract.Workspace.ModelIds;
using AutoMapper;
using Anemoi.Workspace.Domain.Models;

namespace Anemoi.Workspace.Application.Mappings.WorkspaceMappings;

public sealed class ToWorkspaceMapping : Profile
{
    public ToWorkspaceMapping()
    {
        CreateMap<CreateWorkspaceCommand, Anemoi.Workspace.Domain.Models.Workspace>()
            .ForMember(x => x.Id, opt => opt.MapFrom(_ => new WorkspaceId(IdGenerator.NextGuid())))
            .ForMember(x => x.SearchHint, opt => opt.MapFrom(x => x.Name.GenerateSearchHint()))
            .ForMember(x => x.UserId, opt => opt.MapFrom<UserIdValueResolver>())
            .ForMember(x => x.CreatedTime, opt => opt.MapFrom(_ => DateTime.UtcNow))
            .AfterMap((src, des) =>
            {
                var member = new Member
                {
                    Id = new MemberId(IdGenerator.NextGuid()), UserId = des.UserId,
                    WorkspaceId = des.Id, IsActivated = true,
                    CreatedTime = DateTime.UtcNow
                };
                var organizationId = new OrganizationId(IdGenerator.NextGuid());
                des.Organizations =
                [
                    new Organization
                    {
                        Id = organizationId, WorkspaceId = des.Id,
                        Name = src.Name, SearchHint = src.Name.GenerateSearchHint(),
                        SubDomain = src.SubDomain,
                        CreatedTime = DateTime.UtcNow,
                        MemberMapOrganizations =
                        [
                            new MemberMapOrganization
                            {
                                Id = new MemberMapOrganizationId(IdGenerator.NextGuid()),
                                OrganizationId = organizationId, MemberId = member.Id
                            }
                        ]
                    }
                ];
                des.Members = [member];
            });
        CreateMap<UpdateWorkspaceCommand, Anemoi.Workspace.Domain.Models.Workspace>()
            .ForMember(x => x.Id, opt => opt.Ignore())
            .ForMember(x => x.SearchHint, opt =>
            {
                opt.PreCondition(x => x.Name is { });
                opt.MapFrom(x => x.Name.GenerateSearchHint());
            })
            .ForAllMembers(opts => opts.Condition((_, _, srcMember) => srcMember is { }));
    }

    private class UserIdValueResolver(IUserIdGetter userIdGetter)
        : IValueResolver<CreateWorkspaceCommand, Anemoi.Workspace.Domain.Models.Workspace, string>
    {
        public string Resolve(CreateWorkspaceCommand source, Anemoi.Workspace.Domain.Models.Workspace destination,
            string destMember, ResolutionContext context) => userIdGetter.UserId;
    }
}