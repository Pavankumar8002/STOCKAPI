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
            string host = "smtp.gmail.com";
            int port = 587;
            bool enableSsl = true;
            string username = "pavankumarpk8002@gmail.com";
            string password = "dyfy hilf gwhh gzhf";
            string fromEmail = "pavankumarpk8002@gmail.com";
            string fromName = "Pavan Kumar N";

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
                        <div style='
                            max-width:600px;
                            margin:20px auto;
                            padding:30px 40px;
                            background:#fdf6e3;
                            border:2px solid #d4c4a8;
                            border-radius:8px;
                            box-shadow:0 8px 20px rgba(0,0,0,0.15);
                            font-family:Georgia, ""Times New Roman"", serif;
                            color:#3e2f1c;
                        '>

                            <div style='
                                border-left:6px solid #c2a76d;
                                padding-left:20px;
                            '>
                                <p style='font-size:16px;'>Greetings <b>{name}</b>,</p>

                                <p style='font-size:15px; line-height:1.7;'>
                                    I sincerely appreciate you taking the time to visit my portfolio and
                                    share your thoughts with me.
                                </p>

                                <p style='font-size:15px; line-height:1.7;'>
                                    Your message has been safely received, and I shall review it with care.
                                    You may expect a response from me shortly.
                                </p>

                                <p style='font-size:15px; line-height:1.7;'>
                                    Until then, thank you once again for your interest and trust.
                                </p>
                            </div>

                            <hr style='
                                border:none;
                                border-top:1px dashed #c2a76d;
                                margin:25px 0;
                            ' />

                            <p style='font-size:14px;'>
                                With warm regards,<br/>
                                <b style='font-size:16px;'>Pavan Kumar N</b><br/>
                                <span style='font-size:13px;'>Software Developer</span>
                            </p>

                        </div>
                        ",

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
