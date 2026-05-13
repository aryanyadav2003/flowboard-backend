using AutoMapper;
using FlowBoard.ListService.DTOs;
using FlowBoard.ListService.Entities;

namespace FlowBoard.ListService.Mappings;

public class ListMappingProfile : Profile
{
    public ListMappingProfile()
    {
        CreateMap<TaskList, TaskListDto>();
    }
}