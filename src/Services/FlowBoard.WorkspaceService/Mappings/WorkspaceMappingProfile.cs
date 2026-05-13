using AutoMapper;
using FlowBoard.WorkspaceService.DTOs;
using FlowBoard.WorkspaceService.Entities;

namespace FlowBoard.WorkspaceService.Mappings;

public class WorkspaceMappingProfile : Profile
{
    public WorkspaceMappingProfile()
    {
        CreateMap<Workspace,       WorkspaceDto>()
            .ForMember(d => d.MemberCount, o => o.MapFrom(s => s.Members.Count));
        CreateMap<WorkspaceMember, WorkspaceMemberDto>();
    }
}