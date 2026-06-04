using LocalCRM.API.GraphQL.Queries;
using LocalCRM.API.GraphQL.Mutations;
using LocalCRM.Application;
using LocalCRM.Infrastructure;
using LocalCRM.Infrastructure.Persistence;
using LocalCRM.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace LocalCRM.API;

public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Configuration
        var jwtKey = builder.Configuration["Jwt:Key"];
        if (string.IsNullOrEmpty(jwtKey) && !args.Contains("--init"))
        {
             jwtKey = "placeholder_key_at_least_32_characters_long_for_development";
        }

        // Add services
        builder.Services.AddInfrastructure(builder.Configuration);
        builder.Services.AddApplication();
        builder.Services.AddHttpContextAccessor();

        builder.Services.AddIdentity<ApplicationUser, ApplicationRole>()
            .AddEntityFrameworkStores<ApplicationDbContext>()
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
                ValidIssuer = builder.Configuration["Jwt:Issuer"],
                ValidAudience = builder.Configuration["Jwt:Audience"],
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey ?? "placeholder_key_at_least_32_characters_long_for_development"))
            };
        });

        builder.Services.AddControllers();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        builder.Services
            .AddGraphQLServer()
            .AddQueryType(q => q.Name("Query"))
                .AddType<CompanyQueries>()
                .AddType<ContactQueries>()
                .AddType<InteractionQueries>()
                .AddType<EngagementQueries>()
                .AddType<NoteQueries>()
                .AddType<DocumentQueries>()
            .AddMutationType(m => m.Name("Mutation"))
                .AddType<CompanyMutations>()
                .AddType<ContactMutations>()
                .AddType<EngagementMutations>()
                .AddType<NoteMutations>()
                .AddType<DocumentMutations>()
            .AddFiltering()
            .AddSorting()
            .AddProjections();

        var app = builder.Build();

        // CLI Handling
        if (args.Contains("--init"))
        {
            await InitializeDatabase(app, args);
            return;
        }

        // Pipeline
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
    }

    private static async Task InitializeDatabase(WebApplication app, string[] args)
    {
        using var scope = app.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<ApplicationRole>>();

        await context.Database.EnsureCreatedAsync();

        if (!await roleManager.RoleExistsAsync("Administrator"))
            await roleManager.CreateAsync(new ApplicationRole { Name = "Administrator" });

        if (!await roleManager.RoleExistsAsync("User"))
            await roleManager.CreateAsync(new ApplicationRole { Name = "User" });

        var adminEmail = "admin@localcrm.local";
        var adminUser = await userManager.FindByEmailAsync(adminEmail);
        if (adminUser == null)
        {
            var password = args.FirstOrDefault(a => a.StartsWith("--password="))?.Split('=')[1] ?? "DefaultAdminPassword123!";
            adminUser = new ApplicationUser { UserName = "admin", Email = adminEmail, CreatedBy = "system" };
            var result = await userManager.CreateAsync(adminUser, password);
            if (result.Succeeded) await userManager.AddToRoleAsync(adminUser, "Administrator");
            else Console.WriteLine("Admin user creation failed: " + string.Join(", ", result.Errors.Select(e => e.Description)));
        }

        Console.WriteLine("Database initialized successfully.");
    }
}
