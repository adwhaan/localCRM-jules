using LocalCRM.Application.DTOs;
using LocalCRM.Application.Interfaces;
using LocalCRM.Domain.Entities;
using LocalCRM.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LocalCRM.Application.Services
{
    public class DashboardService : IDashboardService
    {
        private readonly IRepository<Company> _companyRepo;
        public DashboardService(IRepository<Company> companyRepo) { _companyRepo = companyRepo; }
        public Task<DashboardDto> GetDashboardDataAsync() => Task.FromResult(new DashboardDto());
    }

    public class SystemMetricsService : ISystemMetricsService
    {
        public Task<SystemMetricsDto> GetSystemMetricsAsync() => Task.FromResult(new SystemMetricsDto());
        public void IncrementApiCall() {}
        public void IncrementApiFailure() {}
        public void UpdateLastAction() {}
    }
}
