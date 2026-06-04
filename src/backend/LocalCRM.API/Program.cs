using LocalCRM.API.GraphQL.Queries;
using LocalCRM.Application.Interfaces;
using LocalCRM.Domain.Entities;
using LocalCRM.Infrastructure.Identity;
using LocalCRM.Infrastructure.Persistence;
using LocalCRM.Infrastructure.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace LocalCRM.API;

public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        builder.Services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection") ?? "Data Source=localcrm.db"));

        builder.Services.AddIdentity<ApplicationUser, ApplicationRole>()
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders();

        builder.Services.AddScoped<IAuditService, AuditService>();
        builder.Services.AddScoped<IAuthService, AuthService>();

        builder.Services.AddControllers();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        builder.Services
            .AddGraphQLServer()
            .AddQueryType<CompanyQueries>()
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

        // Configure the HTTP request pipeline.
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

        // Seed roles
        if (!await roleManager.RoleExistsAsync("Administrator"))
        {
            await roleManager.CreateAsync(new ApplicationRole { Name = "Administrator" });
        }

        // Seed admin user
        var adminEmail = "admin@localcrm.local";
        var adminUser = await userManager.FindByEmailAsync(adminEmail);
        if (adminUser == null)
        {
            var password = args.FirstOrDefault(a => a.StartsWith("--password="))?.Split('=')[1] ?? "DefaultAdminPassword123!";
            adminUser = new ApplicationUser
            {
                UserName = "admin",
                Email = adminEmail,
                CreatedBy = "system"
            };
            await userManager.CreateAsync(adminUser, password);
            await userManager.AddToRoleAsync(adminUser, "Administrator");
        }

        Console.WriteLine("Database initialized successfully.");
    }
}
