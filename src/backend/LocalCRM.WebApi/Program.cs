using LocalCRM.Application.Interfaces;
using LocalCRM.Application.Services;
using LocalCRM.Domain.Entities;
using LocalCRM.Domain.Interfaces;
using LocalCRM.Infrastructure.Persistence;
using LocalCRM.Infrastructure.Persistence.Repositories;
using LocalCRM.Infrastructure.Services;
using LocalCRM.WebApi.Filters;
using LocalCRM.WebApi.GraphQL.Mutations;
using LocalCRM.WebApi.GraphQL.Queries;
using LocalCRM.WebApi.GraphQL.Types;
using LocalCRM.WebApi.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// CLI Check
var isInit = args.Contains("--init");
var adminPassword = args.SkipWhile(a => a != "--password").Skip(1).FirstOrDefault();

if (isInit && string.IsNullOrEmpty(adminPassword))
{
    Console.WriteLine("Error: --password is required with --init");
    return;
}

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? "Data Source=LocalCRM.db";

builder.Services.AddPooledDbContextFactory<LocalCRMContext>(options =>
    options.UseSqlite(connectionString));

builder.Services.AddDbContext<LocalCRMContext>(options =>
    options.UseSqlite(connectionString));

builder.Services.AddIdentity<User, Role>(options => {
    options.Password.RequireDigit = false;
    options.Password.RequiredLength = 6;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireLowercase = false;
})
    .AddEntityFrameworkStores<LocalCRMContext>()
    .AddDefaultTokenProviders();

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"] ?? "LocalCRM",
        ValidAudience = builder.Configuration["Jwt:Audience"] ?? "LocalCRM",
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"] ?? throw new InvalidOperationException("JWT Key is missing from configuration.")))
    };
});

builder.Services.AddHttpContextAccessor();
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

// Repositories
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));

// Services
builder.Services.AddScoped<IAuditService, AuditService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<ICompanyService, CompanyService>();
builder.Services.AddScoped<IContactService, ContactService>();
builder.Services.AddScoped<IInteractionService, InteractionService>();
builder.Services.AddScoped<IEngagementService, EngagementService>();
builder.Services.AddScoped<INoteService, NoteService>();
builder.Services.AddScoped<IDocumentService, DocumentService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IDashboardService, DashboardService>();
builder.Services.AddSingleton<ISystemMetricsService, SystemMetricsService>();
builder.Services.AddScoped<IExportService, ExportService>();
builder.Services.AddScoped<ITagService, TagService>();
builder.Services.AddScoped<ISettingService, SettingService>();
builder.Services.AddScoped<IAuditLogService, AuditLogService>();
builder.Services.AddScoped<IRoleService, RoleService>();

builder.Services
    .AddGraphQLServer()
    .AddQueryType<Query>()
    .AddMutationType<Mutation>()
    .AddType<CompanyType>()
    .AddType<ContactType>()
    .AddType<InteractionType>()
    .AddType<EngagementType>()
    .AddType<NoteType>()
    .AddType<DocumentType>()
    .AddType<UserType>()
    .AddType<AuditLogType>()
    .AddFiltering()
    .AddSorting();

builder.Services.AddControllers(options =>
{
    options.Filters.Add<ApiExceptionFilter>();
});
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (isInit)
{
    using var scope = app.Services.CreateScope();
    var context = scope.ServiceProvider.GetRequiredService<LocalCRMContext>();
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<Role>>();
    await DbInitializer.Initialize(context, userManager, roleManager, adminPassword!);
    Console.WriteLine("Database initialized successfully.");
    if (args.Contains("--init")) return;
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapGraphQL();

app.Run();
