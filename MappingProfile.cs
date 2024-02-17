using AutoMapper;
using FFrelloApi.Models;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<Card, CardDto>();
    }
}
