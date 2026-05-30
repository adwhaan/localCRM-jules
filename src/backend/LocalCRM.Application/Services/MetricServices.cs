using LocalCRM.Application.DTOs;
using LocalCRM.Application.Interfaces;
using LocalCRM.Domain.Entities;
using LocalCRM.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace LocalCRM.Application.Services
{
    public class DashboardService : IDashboardService
    {
        private readonly IRepository<Company> _companyRepo;
        private readonly IRepository<Contact> _contactRepo;
        private readonly IRepository<Interaction> _interactionRepo;
        private readonly IRepository<Engagement> _engagementRepo;

        public DashboardService(
            IRepository<Company> companyRepo,
            IRepository<Contact> contactRepo,
            IRepository<Interaction> interactionRepo,
            IRepository<Engagement> engagementRepo)
        {
            _companyRepo = companyRepo;
            _contactRepo = contactRepo;
            _interactionRepo = interactionRepo;
            _engagementRepo = engagementRepo;
        }

        public async Task<DashboardDto> GetDashboardDataAsync()
        {
            var now = DateTime.UtcNow;
            return new DashboardDto
            {
                TotalCompanies = await _companyRepo.Query().CountAsync(),
                TotalContacts = await _contactRepo.Query().CountAsync(),
                TotalInteractions = await _interactionRepo.Query().CountAsync(),
                ActiveTasks = await _interactionRepo.Query()
                    .CountAsync(i => i.IsTask
                        && i.InteractionDate >= now.Date
                        && i.State != "clsd") // closed
            };
        }
    }

    public class SystemMetricsService : ISystemMetricsService
    {
        private static DateTime _startTime = DateTime.UtcNow;
        private static int _apiCalls = 0;
        private static int _apiFailures = 0;
        private static DateTime? _lastActionAt;
        private readonly string _dbPath;

        public SystemMetricsService()
        {
            _dbPath = "LocalCRM.db";
        }

        public Task<SystemMetricsDto> GetSystemMetricsAsync()
        {
            long dbSize = 0;
            try { if (File.Exists(_dbPath)) dbSize = new FileInfo(_dbPath).Length; } catch {}

            return Task.FromResult(new SystemMetricsDto
            {
                UptimeSeconds = (int)(DateTime.UtcNow - _startTime).TotalSeconds,
                TotalApiCalls = _apiCalls,
                ApiFailures = _apiFailures,
                LastActionAt = _lastActionAt ?? _startTime,
                DatabaseSize = dbSize
            });
        }

        public void IncrementApiCall() { _apiCalls++; _lastActionAt = DateTime.UtcNow; }
        public void IncrementApiFailure() { _apiFailures++; }
        public void UpdateLastAction() { _lastActionAt = DateTime.UtcNow; }
    }
}
