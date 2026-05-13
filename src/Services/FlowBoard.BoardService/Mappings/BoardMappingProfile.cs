using AutoMapper;
using FlowBoard.BoardService.DTOs;
using FlowBoard.BoardService.Entities;

namespace FlowBoard.BoardService.Mappings;

public class BoardMappingProfile : Profile
{
    public BoardMappingProfile()
    {
        CreateMap<Board,       BoardDto>()
            .ForMember(d => d.MemberCount, o => o.MapFrom(s => s.Members.Count));
        CreateMap<BoardMember, BoardMemberDto>();
    }
}