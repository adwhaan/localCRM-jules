using LocalCRM.Application.DTOs;
using HotChocolate.Types;
using HotChocolate;
using LocalCRM.Application.Interfaces;
using LocalCRM.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using AutoMapper;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

namespace LocalCRM.WebApi.GraphQL.Types
{
    public class CompanyType : ObjectType<CompanyDto>
    {
        protected override void Configure(IObjectTypeDescriptor<CompanyDto> descriptor)
        {
            descriptor.Field(c => c.CompanyId).Type<NonNullType<IdType>>();

            descriptor.Field("contacts")
                .Resolve(async (ctx, ct) => {
                    var id = ctx.Parent<CompanyDto>().CompanyId;
                    var context = ctx.Service<LocalCRMContext>();
                    var mapper = ctx.Service<IMapper>();
                    var items = await context.CompanyContactLinks.Where(l => l.CompanyId == id).Select(l => l.Contact).ToListAsync();
                    return mapper.Map<IEnumerable<ContactDto>>(items);
                })
                .Type<ListType<ContactType>>();

            descriptor.Field("notes")
                .Resolve(async (ctx, ct) => {
                    var id = ctx.Parent<CompanyDto>().CompanyId;
                    var context = ctx.Service<LocalCRMContext>();
                    var mapper = ctx.Service<IMapper>();
                    var items = await context.CompanyNoteLinks.Where(l => l.CompanyId == id).Select(l => l.Note).ToListAsync();
                    return mapper.Map<IEnumerable<NoteDto>>(items);
                })
                .Type<ListType<NoteType>>();

            descriptor.Field("documents")
                .Resolve(async (ctx, ct) => {
                    var id = ctx.Parent<CompanyDto>().CompanyId;
                    var context = ctx.Service<LocalCRMContext>();
                    var mapper = ctx.Service<IMapper>();
                    var items = await context.CompanyDocumentLinks.Where(l => l.CompanyId == id).Select(l => l.Document).ToListAsync();
                    return mapper.Map<IEnumerable<DocumentDto>>(items);
                })
                .Type<ListType<DocumentType>>();
        }
    }

    public class ContactType : ObjectType<ContactDto>
    {
        protected override void Configure(IObjectTypeDescriptor<ContactDto> descriptor)
        {
            descriptor.Field(t => t.ContactId).Type<NonNullType<IdType>>();

            descriptor.Field("notes")
                .Resolve(async (ctx, ct) => {
                    var id = ctx.Parent<ContactDto>().ContactId;
                    var context = ctx.Service<LocalCRMContext>();
                    var mapper = ctx.Service<IMapper>();
                    var items = await context.ContactNoteLinks.Where(l => l.ContactId == id).Select(l => l.Note).ToListAsync();
                    return mapper.Map<IEnumerable<NoteDto>>(items);
                })
                .Type<ListType<NoteType>>();
        }
    }

    public class InteractionType : ObjectType<InteractionDto>
    {
        protected override void Configure(IObjectTypeDescriptor<InteractionDto> descriptor)
        {
            descriptor.Field(t => t.InteractionId).Type<NonNullType<IdType>>();

            descriptor.Field("contact")
                .Resolve(async (ctx, ct) => {
                    var id = ctx.Parent<InteractionDto>().ContactId;
                    return id.HasValue ? await ctx.Service<IContactService>().GetByIdAsync(id.Value) : null;
                })
                .Type<ContactType>();

            descriptor.Field("company")
                .Resolve(async (ctx, ct) => {
                    var id = ctx.Parent<InteractionDto>().CompanyId;
                    return id.HasValue ? await ctx.Service<ICompanyService>().GetByIdAsync(id.Value) : null;
                })
                .Type<CompanyType>();

            descriptor.Field("notes")
                .Resolve(async (ctx, ct) => {
                    var id = ctx.Parent<InteractionDto>().InteractionId;
                    var context = ctx.Service<LocalCRMContext>();
                    var mapper = ctx.Service<IMapper>();
                    var items = await context.InteractionNoteLinks.Where(l => l.InteractionId == id).Select(l => l.Note).ToListAsync();
                    return mapper.Map<IEnumerable<NoteDto>>(items);
                })
                .Type<ListType<NoteType>>();

            descriptor.Field("documents")
                .Resolve(async (ctx, ct) => {
                    var id = ctx.Parent<InteractionDto>().InteractionId;
                    var context = ctx.Service<LocalCRMContext>();
                    var mapper = ctx.Service<IMapper>();
                    var items = await context.InteractionDocumentLinks.Where(l => l.InteractionId == id).Select(l => l.Document).ToListAsync();
                    return mapper.Map<IEnumerable<DocumentDto>>(items);
                })
                .Type<ListType<DocumentType>>();
        }
    }

    public class EngagementType : ObjectType<EngagementDto>
    {
        protected override void Configure(IObjectTypeDescriptor<EngagementDto> descriptor)
        {
            descriptor.Field(t => t.EngagementId).Type<NonNullType<IdType>>();

            descriptor.Field("companies")
                .Resolve(async (ctx, ct) => {
                    var id = ctx.Parent<EngagementDto>().EngagementId;
                    var context = ctx.Service<LocalCRMContext>();
                    var mapper = ctx.Service<IMapper>();
                    var items = await context.EngagementCompanyLinks.Where(l => l.EngagementId == id).Select(l => l.Company).ToListAsync();
                    return mapper.Map<IEnumerable<CompanyDto>>(items);
                })
                .Type<ListType<CompanyType>>();

            descriptor.Field("contacts")
                .Resolve(async (ctx, ct) => {
                    var id = ctx.Parent<EngagementDto>().EngagementId;
                    var context = ctx.Service<LocalCRMContext>();
                    var mapper = ctx.Service<IMapper>();
                    var items = await context.EngagementContactLinks.Where(l => l.EngagementId == id).Select(l => l.Contact).ToListAsync();
                    return mapper.Map<IEnumerable<ContactDto>>(items);
                })
                .Type<ListType<ContactType>>();

            descriptor.Field("notes")
                .Resolve(async (ctx, ct) => {
                    var id = ctx.Parent<EngagementDto>().EngagementId;
                    var context = ctx.Service<LocalCRMContext>();
                    var mapper = ctx.Service<IMapper>();
                    var items = await context.EngagementNoteLinks.Where(l => l.EngagementId == id).Select(l => l.Note).ToListAsync();
                    return mapper.Map<IEnumerable<NoteDto>>(items);
                })
                .Type<ListType<NoteType>>();
        }
    }

    public class NoteType : ObjectType<NoteDto>
    {
        protected override void Configure(IObjectTypeDescriptor<NoteDto> descriptor)
        {
            descriptor.Field(t => t.NoteId).Type<NonNullType<IdType>>();
        }
    }

    public class DocumentType : ObjectType<DocumentDto>
    {
        protected override void Configure(IObjectTypeDescriptor<DocumentDto> descriptor)
        {
            descriptor.Field(t => t.DocumentId).Type<NonNullType<IdType>>();
        }
    }

    public class UserType : ObjectType<UserDto>
    {
        protected override void Configure(IObjectTypeDescriptor<UserDto> descriptor)
        {
            descriptor.Field(t => t.Id).Type<NonNullType<IdType>>();
        }
    }

    public class AuditLogType : ObjectType<AuditLogDto>
    {
        protected override void Configure(IObjectTypeDescriptor<AuditLogDto> descriptor)
        {
            descriptor.Field(t => t.AuditId).Type<NonNullType<IdType>>();
        }
    }

    public class DashboardType : ObjectType<DashboardDto> { }
    public class SystemMetricsType : ObjectType<SystemMetricsDto> { }
}
