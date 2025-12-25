using System;
using Npgsql;
using STOCKWEBAPI.DataEntities.Portfolio;
using STOCKWEBAPI.DataEntities.RootStackx;
using STOCKWEBAPI.RepositoryInterface.Portfolio;
using STOCKWEBAPI.RepositoryInterface.RootStackx;

namespace STOCKWEBAPI.Repository.Portfolio
{
        public class PortfolioEnqueryRepo : IPortfolioEnqueryRepo
        {
            private string _connectionString;

            public PortfolioEnqueryRepo(IConfiguration configuration)
            {
                _connectionString = configuration.GetConnectionString("DefaultConnection");
            }

            public async Task<dynamic> SavePortfolioEnquiry(PortfolioEnqueryRequest request)
            {
                await using var connection = new NpgsqlConnection(_connectionString);
                await connection.OpenAsync();

                var query = "SELECT * FROM public.set_portfolio_enqueries(@name, @email, @in_message);";

                using var command = new NpgsqlCommand(query, connection);
                command.Parameters.AddWithValue("name", request.Name);
                command.Parameters.AddWithValue("email", request.Email);
                command.Parameters.AddWithValue("in_message", request.Message);

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

