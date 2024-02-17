using AutoMapper;
using FFrelloApi.Models;
using FFrelloApi.Models.Dto;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<Card, CardDto>()
            .ForMember(dest => dest.Comments, opt => opt.MapFrom(src => src.Comments.OrderByDescending(x => x.DateTime)));

        CreateMap<CardComment, CardCommentDto>()
            .ForMember(dest => dest.Username, opt => opt.MapFrom(src => src.User.Name))
            .ForMember(dest => dest.ProfilePhotoUrl, opt => opt.MapFrom(src => src.User.ProfilePhotoUrl))
            .ForMember(dest => dest.Timestamp, opt => opt.MapFrom(src => src.DateTime.ToString("MMMM d, yyyy")));

        CreateMap<List<CardComment>, List<CardCommentDto>>().ConvertUsing<ListConverter>();
    }

    public class ListConverter : ITypeConverter<List<CardComment>, List<CardCommentDto>>
    {
        public List<CardCommentDto> Convert(List<CardComment> source, List<CardCommentDto> destination, ResolutionContext context)
        {
            var dtoList = new List<CardCommentDto>();
            foreach (var comment in source)
            {
                var dto = context.Mapper.Map<CardCommentDto>(comment);
                dtoList.Add(dto);
            }
            return dtoList;
        }
    }
}
