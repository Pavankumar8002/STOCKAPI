using System;
using System.Threading.Tasks;
using Npgsql;
using Microsoft.Extensions.Configuration;
using STOCKWEBAPI.RepositoryInterface.Login;

namespace STOCKWEBAPI.Repository.Login
{
    public class LoginRepo : ILoginRepo
    {
        private readonly string _connectionString;

        public LoginRepo(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public async Task<dynamic> ValidateUser(string username, string password)
        {
            try
            {
                await using var connection = new NpgsqlConnection(_connectionString);
                await connection.OpenAsync();

                var query = "SELECT row_to_json(t) FROM get_users(@id_input, @passcode_input) AS t;";
                await using var command = new NpgsqlCommand(query, connection);
                command.Parameters.AddWithValue("id_input", username);
                command.Parameters.AddWithValue("passcode_input", password);
               

                await using var reader = await command.ExecuteReaderAsync();

                if (await reader.ReadAsync())
                {
                    string jsonResult = reader.GetString(0); 
                    return jsonResult;
                }

                return "{\"status\": -1, \"message\": \"No data returned.\"}";
            }
            catch (Exception ex)
            {
                return $"{{\"status\": -1, \"message\": \"Error: {ex.Message}\"}}";
            }
        }

    }
}
