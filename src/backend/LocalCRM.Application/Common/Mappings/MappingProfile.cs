using AutoMapper;
using LocalCRM.Application.DTOs;
using LocalCRM.Domain.Entities;

namespace LocalCRM.Application.Common.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<Company, CompanyDto>().ReverseMap();
        CreateMap<Contact, ContactDto>().ReverseMap();
        CreateMap<Interaction, InteractionDto>().ReverseMap();
        CreateMap<Engagement, EngagementDto>().ReverseMap();
        CreateMap<Note, NoteDto>().ReverseMap();
        CreateMap<Document, DocumentDto>().ReverseMap();
        CreateMap<ApplicationUser, UserDto>()
            .ForMember(d => d.Username, opt => opt.MapFrom(s => s.UserName))
            .ForMember(d => d.IsActive, opt => opt.MapFrom(s => s.LockoutEnd == null || s.LockoutEnd < DateTime.UtcNow))
            .ForMember(d => d.MustChangePassword, opt => opt.MapFrom(s => s.MustChangePassword));
    }
}
