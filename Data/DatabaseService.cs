using System.Data;
using Microsoft.Data.SqlClient;
using Dapper;

namespace Quiniela.Data
{
    public class DatabaseService : IDatabaseService
    {
        private readonly string _connectionString;

        public DatabaseService(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection")
                ?? throw new ArgumentNullException("DefaultConnection", "Connection string is not configured");
        }

        public async Task<T?> ExecuteStoredProcedureSingle<T>(string storedProcedure, object? parameters = null)
        {
            using var connection = new SqlConnection(_connectionString);
            return await connection.QueryFirstOrDefaultAsync<T?>(
                storedProcedure,
                parameters,
                commandType: CommandType.StoredProcedure);
        }

        public async Task<IEnumerable<T>> ExecuteStoredProcedure<T>(string storedProcedure, object? parameters = null)
        {
            using var connection = new SqlConnection(_connectionString);
            return await connection.QueryAsync<T>(
                storedProcedure,
                parameters ?? new { }, // Provide empty object if null
                commandType: CommandType.StoredProcedure);
        }

        public async Task<int> ExecuteStoredProcedure(string storedProcedure, object? parameters = null)
        {
            using var connection = new SqlConnection(_connectionString);
            return await connection.ExecuteAsync(
                storedProcedure,
                parameters ?? new { }, // Provide empty object if null
                commandType: CommandType.StoredProcedure);
        }
    }
}