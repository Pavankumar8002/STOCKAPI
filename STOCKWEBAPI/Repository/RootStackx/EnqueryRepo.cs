using System;
using Npgsql;
using STOCKWEBAPI.DataEntities.RootStackx;
using STOCKWEBAPI.DataEntities.Users;
using STOCKWEBAPI.RepositoryInterface.RootStackx;

namespace STOCKWEBAPI.Repository.RootStackx
{
	public class EnqueryRepo : IEnqueryRepo
	{
        private string _connectionString;

        public EnqueryRepo(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public async Task<dynamic> SaveEnquiry(EnquiryRequest request)
        {
            await using var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync();

            var query = "SELECT * FROM public.insert_enquiry(@p_name, @p_email, @p_message);";

            using var command = new NpgsqlCommand(query, connection);
            command.Parameters.AddWithValue("p_name", request.Name);
            command.Parameters.AddWithValue("p_email", request.Email);
            command.Parameters.AddWithValue("p_message", request.Message);

            using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(30));

            await using var reader = await command.ExecuteReaderAsync(cts.Token);

            if (await reader.ReadAsync(cts.Token))
            {
                int status = reader.GetInt32(0);
                string message = reader.GetString(1);

                return $"{{\"status\":{status},\"message\":\"{message}\"}}";
            }

            return "{\"status\":-1,\"message\":\"Unexpected error.\"}";
        }

    }
}

