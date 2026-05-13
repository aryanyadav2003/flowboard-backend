using AutoMapper;
using FlowBoard.CardService.DTOs;
using FlowBoard.CardService.Entities;

namespace FlowBoard.CardService.Mappings;

public class CardMappingProfile : Profile
{
    public CardMappingProfile()
    {
        CreateMap<Card, CardDto>().ForMember(d => d.IsOverdue, o => o.MapFrom(s =>
                s.DueDate.HasValue &&
                s.DueDate.Value < DateTime.UtcNow &&
                s.Status != "DONE"));

        CreateMap<CardAssignee, CardAssigneeDto>();
    }
}