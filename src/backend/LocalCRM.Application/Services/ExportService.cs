using LocalCRM.Application.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalCRM.Application.Services
{
    public class ExportService : IExportService
    {
        public Task<byte[]> ExportToCsvAsync<T>(IEnumerable<T> data)
        {
            var properties = typeof(T).GetProperties();
            var sb = new StringBuilder();

            // Header
            sb.AppendLine(string.Join(",", properties.Select(p => $"\"{p.Name}\"")));

            // Rows
            foreach (var item in data)
            {
                var values = properties.Select(p => {
                    var val = p.GetValue(item);
                    return val != null ? $"\"{val.ToString()?.Replace("\"", "\"\"")}\"" : "";
                });
                sb.AppendLine(string.Join(",", values));
            }

            return Task.FromResult(Encoding.UTF8.GetBytes(sb.ToString()));
        }
    }
}
