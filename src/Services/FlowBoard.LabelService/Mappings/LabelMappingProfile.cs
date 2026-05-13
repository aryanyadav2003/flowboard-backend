using AutoMapper;
using FlowBoard.LabelService.DTOs;
using FlowBoard.LabelService.Entities;

namespace FlowBoard.LabelService.Mappings;

public class LabelMappingProfile : Profile
{
    public LabelMappingProfile()
    {
        CreateMap<Label, LabelDto>();

        CreateMap<CardLabel, CardLabelDto>()
            .ForMember(d => d.LabelName,
                o => o.MapFrom(s => s.Label.Name))
            .ForMember(d => d.LabelColor,
                o => o.MapFrom(s => s.Label.Color));
    }
}