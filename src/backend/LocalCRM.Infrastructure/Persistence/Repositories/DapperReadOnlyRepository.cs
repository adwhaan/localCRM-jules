using Dapper;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace LocalCRM.Infrastructure.Persistence.Repositories
{
    public class DapperReadOnlyRepository
    {
        private readonly string _connectionString;

        public DapperReadOnlyRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection") ?? "Data Source=LocalCRM.db";
        }

        public IDbConnection CreateConnection() => new SqliteConnection(_connectionString);

        public async Task<IEnumerable<T>> QueryAsync<T>(string sql, object? param = null)
        {
            using var connection = CreateConnection();
            return await connection.QueryAsync<T>(sql, param);
        }
    }
}
