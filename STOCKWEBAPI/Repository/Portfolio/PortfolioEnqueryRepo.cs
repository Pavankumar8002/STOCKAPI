using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Npgsql;
using STOCKWEBAPI.DataEntities.Portfolio;
using STOCKWEBAPI.RepositoryInterface.Portfolio;

namespace STOCKWEBAPI.Repository.Portfolio
{
    public class PortfolioEnqueryRepo : IPortfolioEnqueryRepo
    {
        private readonly string _connectionString;
        private readonly string _resendApiKey;
        private readonly string _fromEmail;
        private readonly string _adminEmail;

        public PortfolioEnqueryRepo(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection")
                ?? throw new Exception("DefaultConnection missing");

            _resendApiKey = configuration["Resend:ApiKey"]
                ?? throw new Exception("Resend ApiKey missing");

            _fromEmail = configuration["Resend:FromEmail"]
                ?? throw new Exception("FromEmail missing");

            _adminEmail = configuration["Resend:AdminEmail"]
                ?? throw new Exception("AdminEmail missing");
        }

        public async Task<dynamic> SavePortfolioEnquiry(PortfolioEnqueryRequest request)
        {
            await using var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync();

            const string query =
                "SELECT * FROM public.set_portfolio_enqueries(@name, @email, @in_message);";

            await using var command = new NpgsqlCommand(query, connection);
            command.Parameters.AddWithValue("name", request.Name);
            command.Parameters.AddWithValue("email", request.Email);
            command.Parameters.AddWithValue("in_message", request.Message);

            using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(30));
            await using var reader = await command.ExecuteReaderAsync(cts.Token);

            if (!await reader.ReadAsync(cts.Token))
            {
                return new
                {
                    status = -1,
                    message = "Unexpected error while saving enquiry."
                };
            }

            int status = reader.GetInt32(0);
            string message = reader.GetString(1);

            if (status == 1)
            {
                var mailResult = await SendMailAsync(
                    request.Name,
                    request.Email,
                    request.Message
                );

                if (!mailResult.Success)
                {
                    return new
                    {
                        status = 0,
                        message = "Enquiry saved, but email sending failed."
                    };
                }
            }

            return new
            {
                status,
                message
            };
        }

        // ================= EMAIL SENDER =================
        private async Task<MailResult> SendMailAsync(string name, string email, string message)
        {
            try
            {
                using var client = new HttpClient();
                client.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", _resendApiKey);

                // -------- ADMIN EMAIL --------
                var adminPayload = new
                {
                    from = _fromEmail,
                    to = new[] { _adminEmail },
                    subject = "New Portfolio Enquiry – Rootstackx",
                    html = $@"
                        <h3>New Portfolio Enquiry</h3>
                        <p><b>Name:</b> {name}</p>
                        <p><b>Email:</b> {email}</p>
                        <p><b>Message:</b><br/>{message}</p>"
                };

                await SendResendAsync(client, adminPayload);

               // --------THANK YOU EMAIL --------
               var thankYouPayload = new
               {
                   from = _fromEmail,
                   to = new[] { email },
                   subject = "Thank you for contacting Pavan's Portfolio",
                   html = $@"
                        <p>Hi <b>{name}</b>,</p>
                        <p>
                            Thank you for reaching out.
                            I’ve received your message and will get back to you shortly.
                        </p>
                        <p>
                            Regards,<br/>
                            <b>Pavan Kumar N</b>
                        </p>"
               };

                await SendResendAsync(client, thankYouPayload);

                return MailResult.SuccessResult();
            }
            catch (Exception ex)
            {
                return MailResult.FailureResult(ex.Message);
            }
        }

        private static async Task SendResendAsync(HttpClient client, object payload)
        {
            var content = new StringContent(
                JsonSerializer.Serialize(payload),
                Encoding.UTF8,
                "application/json"
            );

            var response = await client.PostAsync(
                "https://api.resend.com/emails",
                content
            );

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                throw new Exception($"Resend API error: {error}");
            }
        }
    }

    // ================= HELPER =================
    public class MailResult
    {
        public bool Success { get; private set; }
        public string? ErrorMessage { get; private set; }

        public static MailResult SuccessResult()
            => new MailResult { Success = true };

        public static MailResult FailureResult(string error)
            => new MailResult { Success = false, ErrorMessage = error };
    }
}
