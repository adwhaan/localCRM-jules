using LocalCRM.Application.Interfaces;
using LocalCRM.Infrastructure.Persistence;
using LocalCRM.Infrastructure.Services;
using LocalCRM.Infrastructure.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace LocalCRM.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlite(configuration.GetConnectionString("DefaultConnection") ?? "Data Source=localcrm.db"));

        services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
        services.AddScoped<IAuditService, AuditService>();
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IDashboardService, DashboardService>();
        services.AddScoped<ICurrentUserService, CurrentUserService>();

        return services;
    }
}
