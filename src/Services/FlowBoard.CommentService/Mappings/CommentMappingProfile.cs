using AutoMapper;
using FlowBoard.CommentService.DTOs;
using FlowBoard.CommentService.Entities;

namespace FlowBoard.CommentService.Mappings;

public class CommentMappingProfile : Profile
{
    public CommentMappingProfile()
    {
        CreateMap<Comment, CommentDto>();
    }
}