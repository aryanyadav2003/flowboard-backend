using AutoMapper;
using FlowBoard.AuthService.DTOs;
using FlowBoard.AuthService.Entities;

namespace FlowBoard.AuthService.Mappings;

public class AuthMappingProfile : Profile
{
    public AuthMappingProfile()
    {
        CreateMap<User, UserDto>();
    }
}