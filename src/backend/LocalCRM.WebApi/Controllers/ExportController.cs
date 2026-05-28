using LocalCRM.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace LocalCRM.WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ExportController : ControllerBase
    {
        private readonly IExportService _exportService;
        private readonly ICompanyService _companyService;
        private readonly IContactService _contactService;

        public ExportController(IExportService exportService, ICompanyService companyService, IContactService contactService)
        {
            _exportService = exportService;
            _companyService = companyService;
            _contactService = contactService;
        }

        [HttpPost("companies")]
        public async Task<IActionResult> ExportCompanies()
        {
            var data = await _companyService.GetAllAsync();
            var csv = await _exportService.ExportToCsvAsync(data);
            return File(csv, "text/csv", "companies.csv");
        }

        [HttpPost("contacts")]
        public async Task<IActionResult> ExportContacts()
        {
            var data = await _contactService.GetAllAsync();
            var csv = await _exportService.ExportToCsvAsync(data);
            return File(csv, "text/csv", "contacts.csv");
        }
    }
}
