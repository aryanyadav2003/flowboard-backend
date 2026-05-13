using AutoMapper;
using FlowBoard.ChecklistService.DTOs;
using FlowBoard.ChecklistService.Entities;

namespace FlowBoard.ChecklistService.Mappings;

public class ChecklistMappingProfile : Profile
{
    public ChecklistMappingProfile()
    {
        CreateMap<ChecklistItem, ChecklistItemDto>();
    }
}