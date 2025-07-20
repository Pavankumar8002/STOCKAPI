using System;
using System.Text.Json;
using Npgsql;
using STOCKWEBAPI.DataEntities.RootStackx;
using STOCKWEBAPI.RepositoryInterface.RootStackx;

namespace STOCKWEBAPI.Repository.RootStackx
{
	public class EnquiryDetailsRepo : IEnquiryDetailsRepo
	{
        private string _connectionString;

        public EnquiryDetailsRepo(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public async Task<string> ViewEnquiry()
        {
            await using var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync();

            var query = "SELECT * FROM public.get_enquiries();";
            await using var command = new NpgsqlCommand(query, connection);
            await using var reader = await command.ExecuteReaderAsync();

            var results = new List<object>();

            while (await reader.ReadAsync())
            {
                results.Add(new
                {
                    name = reader["name"]?.ToString(),
                    email = reader["email"]?.ToString(),
                    message = reader["message"]?.ToString(),
                    created_at = reader["created_at"] != DBNull.Value
                        ? Convert.ToDateTime(reader["created_at"]).ToString("yyyy-MM-dd HH:mm:ss")
                        : null
                });
            }

            return JsonSerializer.Serialize(results); 
        }
    }
}

