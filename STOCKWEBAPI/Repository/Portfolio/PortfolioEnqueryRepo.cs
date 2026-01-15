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
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _connectionString = configuration.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException("DefaultConnection string is missing in appsettings.json");
        }

        public async Task<dynamic> SavePortfolioEnquiry(PortfolioEnqueryRequest request)
        {
            if (request == null) throw new ArgumentNullException(nameof(request));

            await using var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync();

            var query = "SELECT * FROM public.set_portfolio_enqueries(@name, @email, @in_message);";

            await using var command = new NpgsqlCommand(query, connection);
            command.Parameters.AddWithValue("name", request.Name);
            command.Parameters.AddWithValue("email", request.Email);
            command.Parameters.AddWithValue("in_message", request.Message);

            using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(30));
            await using var reader = await command.ExecuteReaderAsync(cts.Token);

            if (await reader.ReadAsync(cts.Token))
            {
                int status = reader.GetInt32(0);
                string message = reader.GetString(1);

                // Only attempt email if DB insert succeeded
                if (status == 1)
                {
                    try
                    {
                        await SendMailAsync(request);
                    }
                    catch
                    {
                        // Do not throw; just modify the response
                        status = 0;
                        message = "Enquiry saved, but email sending failed.";
                    }
                }

                return new
                {
                    status,
                    message
                };
            }

            // Unexpected DB failure
            return new
            {
                status = -1,
                message = "Unexpected error."
            };
        }

        private async Task SendMailAsync(PortfolioEnqueryRequest request)
        {
            var host = _configuration["SmtpSettings:Host"] ?? throw new InvalidOperationException("SMTP Host missing");
            var portString = _configuration["SmtpSettings:Port"];
            var sslString = _configuration["SmtpSettings:EnableSsl"];
            var username = _configuration["SmtpSettings:Username"] ?? throw new InvalidOperationException("SMTP Username missing");
            var password = _configuration["SmtpSettings:Password"] ?? throw new InvalidOperationException("SMTP Password missing");
            var fromEmail = _configuration["SmtpSettings:FromEmail"] ?? throw new InvalidOperationException("SMTP FromEmail missing");
            var fromName = _configuration["SmtpSettings:FromName"] ?? "Rootstackx";

            if (!int.TryParse(portString, out var port))
                throw new InvalidOperationException("SMTP Port is invalid");

            if (!bool.TryParse(sslString, out var enableSsl))
                throw new InvalidOperationException("SMTP EnableSsl value is invalid");

            using var client = new SmtpClient(host, port)
            {
                Credentials = new NetworkCredential(username, password),
                EnableSsl = enableSsl
            };

            // ====== Admin Email ======
            using var adminMail = new MailMessage
            {
                From = new MailAddress(fromEmail, fromName),
                Subject = "New Portfolio Enquiry – Rootstackx",
                Body = $@"
                    <h3>New Portfolio Enquiry</h3>
                    <p><b>Name:</b> {request.Name}</p>
                    <p><b>Email:</b> {request.Email}</p>
                    <p><b>Message:</b><br/>{request.Message}</p>
                ",
                IsBodyHtml = true
            };
            adminMail.To.Add(fromEmail);
            await client.SendMailAsync(adminMail);

            // ====== Thank You Email ======
            using var thankYouMail = new MailMessage
            {
                From = new MailAddress(fromEmail, fromName),
                Subject = "Thank you for contacting Pavan's portfolio",
                Body = $@"
                    <div style='
                        max-width:600px;
                        margin:20px auto;
                        padding:30px 40px;
                        background:#fdf6e3;
                        border:2px solid #d4c4a8;
                        border-radius:8px;
                        font-family:Georgia, ""Times New Roman"", serif;
                        color:#3e2f1c;
                    '>
                        <p style='font-size:16px;'>Greetings <b>{request.Name}</b>,</p>
                        <p>Your message has been received. I will review it and respond shortly.</p>
                        <p>Thank you for your interest!</p>
                        <p>With warm regards,<br/><b>Pavan Kumar N</b></p>
                    </div>",
                IsBodyHtml = true
            };
            thankYouMail.To.Add(request.Email);
            await client.SendMailAsync(thankYouMail);
        }
    }
}
