using Microsoft.Extensions.Configuration;
using Npgsql;
using System;
using System.Dynamic;
using System.Collections.Generic;
using STOCKWEBAPI.RepositoryInterface.Users;
using STOCKWEBAPI.DataEntities.Users;

namespace STOCKWEBAPI.Repository.Users
{
    public class UsersRepo : IUsersRepo
    {
        private readonly string _connectionString;

        public UsersRepo(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        //public async Task<dynamic> SaveUsers(UsersRequestClass request)
        //{
        //    await using var connection = new NpgsqlConnection(_connectionString);
        //    await connection.OpenAsync();

        //    await using var command = new NpgsqlCommand("SELECT * FROM insert_user(@p_username, @p_password);", connection);
        //    command.CommandTimeout = 30; // in seconds
        //    command.Parameters.AddWithValue("p_username", request.Username);
        //    command.Parameters.AddWithValue("p_password", request.Password);

        //    await using var reader = await command.ExecuteReaderAsync();

        //    if (await reader.ReadAsync())
        //    {
        //        return new
        //        {
        //            status = reader.GetInt32(0),
        //            message = reader.GetString(1)
        //        };
        //    }

        //    return new { status = -1, message = "Unexpected error." };
        //}

        //public async Task<dynamic> SaveUsers(UsersRequestClass request)
        //{
        //    await using var connection = new NpgsqlConnection(_connectionString);
        //    await connection.OpenAsync(); // ensure connection is ready

        //    var query = "SELECT * FROM insert_user(@p_username, @p_password);";

        //    using var command = new NpgsqlCommand(query, connection); // no await needed
        //    command.Parameters.AddWithValue("p_username", request.Username);
        //    command.Parameters.AddWithValue("p_password", request.Password);
        //    command.CommandTimeout = 30; // optional

        //    await using var reader = await command.ExecuteReaderAsync();

        //    if (await reader.ReadAsync())
        //    {
        //        return new
        //        {
        //            status = reader.GetInt32(0),
        //            message = reader.GetString(1)
        //        };
        //    }

        //    return new { status = -1, message = "Unexpected error." };
        //}
        public async Task<dynamic> SaveUsers(UsersRequestClass request)
        {
            await using var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync(); // ensure connection is ready

            var query = "SELECT * FROM insert_user(@p_username, @p_password);";

            using var command = new NpgsqlCommand(query, connection); // no await needed
            command.Parameters.AddWithValue("p_username", request.Username);
            command.Parameters.AddWithValue("p_password", request.Password);
            command.CommandTimeout = 30; // optional

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
