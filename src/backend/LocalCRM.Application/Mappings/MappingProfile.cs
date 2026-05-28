using LocalCRM.Application.DTOs;
using LocalCRM.Domain.Entities;
using AutoMapper;

namespace LocalCRM.Application.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Company, CompanyDto>();
            CreateMap<CreateCompanyDto, Company>();
            CreateMap<UpdateCompanyDto, Company>();

            CreateMap<Contact, ContactDto>();
            CreateMap<CreateContactDto, Contact>();
            CreateMap<UpdateContactDto, Contact>();

            CreateMap<Interaction, InteractionDto>()
                .ForMember(dest => dest.ContactId, opt => opt.MapFrom(src => src.InteractionLink != null ? src.InteractionLink.ContactId : null))
                .ForMember(dest => dest.CompanyId, opt => opt.MapFrom(src => src.InteractionLink != null ? src.InteractionLink.CompanyId : null))
                .ForMember(dest => dest.EngagementId, opt => opt.MapFrom(src => src.InteractionLink != null ? src.InteractionLink.EngagementId : null));
            CreateMap<CreateInteractionDto, Interaction>();
            CreateMap<UpdateInteractionDto, Interaction>();

            CreateMap<Engagement, EngagementDto>();
            CreateMap<CreateEngagementDto, Engagement>();
            CreateMap<UpdateEngagementDto, Engagement>();

            CreateMap<Note, NoteDto>();
            CreateMap<CreateNoteDto, Note>();
            CreateMap<UpdateNoteDto, Note>();

            CreateMap<Document, DocumentDto>();
            CreateMap<AuditLog, AuditLogDto>();
            CreateMap<CreateDocumentDto, Document>();
            CreateMap<UpdateDocumentDto, Document>();
        }
    }
}
