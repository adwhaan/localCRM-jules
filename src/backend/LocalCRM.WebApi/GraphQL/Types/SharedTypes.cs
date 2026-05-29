using LocalCRM.Application.DTOs;
using HotChocolate.Types;
using HotChocolate;
using LocalCRM.Application.Interfaces;
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
                    var service = ctx.Service<IContactService>();
                    // Simplified: In a real app we'd filter by CompanyId via a join or specific service method
                    return await service.GetAllAsync();
                })
                .Type<ListType<ContactType>>();
        }
    }

    public class ContactType : ObjectType<ContactDto>
    {
        protected override void Configure(IObjectTypeDescriptor<ContactDto> descriptor)
        {
            descriptor.Field(t => t.ContactId).Type<NonNullType<IdType>>();
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
        }
    }

    public class EngagementType : ObjectType<EngagementDto>
    {
        protected override void Configure(IObjectTypeDescriptor<EngagementDto> descriptor)
        {
            descriptor.Field(t => t.EngagementId).Type<NonNullType<IdType>>();
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
