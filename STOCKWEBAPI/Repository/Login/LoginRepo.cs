using System;
using System.Threading.Tasks;
using Npgsql;
using Microsoft.Extensions.Configuration;
using STOCKWEBAPI.RepositoryInterface.Login;
using System.Dynamic;

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

                    var query = "SELECT * FROM public.validate_user(@id_input, @passcode_input);";
                    await using var command = new NpgsqlCommand(query, connection);
                    command.Parameters.AddWithValue("id_input", username);
                    command.Parameters.AddWithValue("passcode_input", password);

                    await using var reader = await command.ExecuteReaderAsync();

                    if (await reader.ReadAsync())
                    {
                        dynamic result = new ExpandoObject();
                        result.status = reader.GetInt32(0);
                        result.message = reader.GetString(1);
                        return result;
                    }

                    dynamic noData = new ExpandoObject();
                    noData.status = -1;
                    noData.message = "No data returned.";
                    return noData;
                }
                catch (Exception ex)
                {
                    dynamic error = new ExpandoObject();
                    error.status = -1;
                    error.message = $"Error: {ex.Message}";
                    return error;
                }
        }

    }
}
