using LocalCRM.Application.Interfaces;
using LocalCRM.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace LocalCRM.Infrastructure.Services;

public class DashboardService : IDashboardService
{
    private readonly ApplicationDbContext _context;

    public DashboardService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<object> GetDashboardMetricsAsync()
    {
        return new
        {
            TotalCompanies = await _context.Companies.CountAsync(c => !c.IsDeleted),
            TotalContacts = await _context.Contacts.CountAsync(c => !c.IsDeleted),
            UpcomingTasks = await _context.Interactions.CountAsync(i => !i.IsDeleted && i.IsTask && i.State == "open")
        };
    }

    public async Task<object> GetSystemMetricsAsync()
    {
        return new
        {
            TotalUsers = await _context.Users.CountAsync(),
            ActiveSessions = await _context.RefreshTokens.CountAsync(t => t.ExpiresAt > DateTime.UtcNow && t.RevokedAt == null)
        };
    }
}
