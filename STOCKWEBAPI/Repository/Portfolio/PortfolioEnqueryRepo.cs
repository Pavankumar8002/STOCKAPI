using System;
using System.Net;
using System.Net.Mail;
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
        private readonly IConfiguration _configuration;

        public PortfolioEnqueryRepo(IConfiguration configuration)
        {
            _configuration = configuration;
            _connectionString = configuration.GetConnectionString("DefaultConnection");
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

            // Send email only if DB insert was successful
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

        private async Task<MailResult> SendMailAsync(string name, string email, string message)
        {
            string host = _configuration["SmtpSettings:Host"];
            int port = int.Parse(_configuration["SmtpSettings:Port"]);
            bool enableSsl = bool.Parse(_configuration["SmtpSettings:EnableSsl"]);
            string username = _configuration["SmtpSettings:Username"];
            string password = _configuration["SmtpSettings:Password"];
            string fromEmail = _configuration["SmtpSettings:FromEmail"];
            string fromName = _configuration["SmtpSettings:FromName"];

            try
            {
                using var client = new SmtpClient(host, port)
                {
                    Credentials = new NetworkCredential(username, password),
                    EnableSsl = enableSsl
                };

                // ================= ADMIN EMAIL =================
                using var adminMail = new MailMessage
                {
                    From = new MailAddress(fromEmail, fromName),
                    Subject = "New Portfolio Enquiry – Rootstackx",
                    Body = $@"
                        <h3>New Portfolio Enquiry</h3>
                        <p><b>Name:</b> {name}</p>
                        <p><b>Email:</b> {email}</p>
                        <p><b>Message:</b><br/>{message}</p>",
                    IsBodyHtml = true
                };

                adminMail.To.Add(fromEmail);
                await client.SendMailAsync(adminMail);

                // ================= THANK YOU EMAIL =================
                using var thankYouMail = new MailMessage
                {
                    From = new MailAddress(fromEmail, fromName),
                    Subject = "Thank you for contacting Pavan's portfolio",
                    Body = $@"
                        <p>Dear <b>{name}</b>,</p>
                        <p>Thank you for reaching out. Your message has been received.</p>
                        <p>I will get back to you shortly.</p>
                        <br/>
                        <p>Regards,<br/><b>Pavan Kumar N</b></p>",
                    IsBodyHtml = true
                };

                thankYouMail.To.Add(email);
                await client.SendMailAsync(thankYouMail);

                return MailResult.SuccessResult();
            }
            catch (Exception ex)
            {
                // You can log ex here using ILogger or Serilog
                return MailResult.FailureResult(ex.Message);
            }
        }
    }

    // ================= HELPER CLASS =================
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
