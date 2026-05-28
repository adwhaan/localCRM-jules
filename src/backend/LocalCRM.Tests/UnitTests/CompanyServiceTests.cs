using AutoMapper;
using LocalCRM.Application.DTOs;
using LocalCRM.Application.Services;
using LocalCRM.Domain.Entities;
using LocalCRM.Domain.Interfaces;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace LocalCRM.Tests.UnitTests
{
    public class CompanyServiceTests
    {
        private readonly Mock<IRepository<Company>> _repoMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<IAuditService> _auditMock;
        private readonly CompanyService _service;

        public CompanyServiceTests()
        {
            _repoMock = new Mock<IRepository<Company>>();
            _mapperMock = new Mock<IMapper>();
            _auditMock = new Mock<IAuditService>();
            _service = new CompanyService(_repoMock.Object, _mapperMock.Object, _auditMock.Object);
        }

        [Fact]
        public async Task GetAllAsync_ShouldReturnMappedDtos()
        {
            var companies = new List<Company> { new Company { CompanyId = 1, Name = "Test" } };
            _repoMock.Setup(r => r.GetAllAsync()).ReturnsAsync(companies);
            _mapperMock.Setup(m => m.Map<IEnumerable<CompanyDto>>(companies)).Returns(new List<CompanyDto> { new CompanyDto { Name = "Test" } });

            var result = await _service.GetAllAsync();

            Assert.Single(result);
        }
    }
}
