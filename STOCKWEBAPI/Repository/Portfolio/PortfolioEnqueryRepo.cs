using System;
using System.Net;
using System.Net.Mail;
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

                if (status == 1)
                {
                    await SendMailAsync(
                        request.Name,
                        request.Email,
                        request.Message
                    );
                }

                return $"{{\"status\":{status},\"message\":\"{message}\"}}";
            }

            return "{\"status\":-1,\"message\":\"Unexpected error.\"}";
        }

        //private async Task SendMailAsync(string name, string email, string message)
        //{
        //    string host = _configuration["SmtpSettings:Host"];
        //    int port = int.Parse(_configuration["SmtpSettings:Port"]);
        //    bool enableSsl = bool.Parse(_configuration["SmtpSettings:EnableSsl"]);
        //    string username = _configuration["SmtpSettings:Username"];
        //    string password = _configuration["SmtpSettings:Password"];
        //    string fromEmail = _configuration["SmtpSettings:FromEmail"];
        //    string fromName = _configuration["SmtpSettings:FromName"];
        //    try
        //    {
        //        using var mail = new MailMessage
        //        {
        //            From = new MailAddress(fromEmail, fromName),
        //            Subject = "New Portfolio Enquiry – Rootstackx",
        //            Body = $@"
        //                <h3>New Portfolio Enquiry</h3>
        //                <p><b>Name:</b> {name}</p>
        //                <p><b>Email:</b> {email}</p>
        //                <p><b>Message:</b><br/>{message}</p>
        //            ",
        //            IsBodyHtml = true
        //        };

        //        // Send to admin inbox
        //        mail.To.Add(fromEmail);

        //        using var client = new SmtpClient(host, port)
        //        {
        //            Credentials = new NetworkCredential(username, password),
        //            EnableSsl = enableSsl
        //        };

        //        await client.SendMailAsync(mail);
        //    }
        //    catch(Exception ex)
        //    {
        //        return;
        //    }
        //}
        private async Task SendMailAsync(string name, string email, string message)
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

                /* =========================
                   ADMIN EMAIL
                ========================= */
                using var adminMail = new MailMessage
                {
                    From = new MailAddress(fromEmail, fromName),
                    Subject = "New Portfolio Enquiry – Rootstackx",
                    Body = $@"
                <h3>New Portfolio Enquiry</h3>
                <p><b>Name:</b> {name}</p>
                <p><b>Email:</b> {email}</p>
                <p><b>Message:</b><br/>{message}</p>
            ",
                    IsBodyHtml = true
                };

                adminMail.To.Add(fromEmail);
                await client.SendMailAsync(adminMail);

                /* =========================
                   THANK YOU EMAIL
                ========================= */
                using var thankYouMail = new MailMessage
                {
                    From = new MailAddress(fromEmail, fromName),
                    Subject = "Thank you for contacting pavan's portfolio",
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
                               
                            </p>

                        </div>
                        ",
                    IsBodyHtml = true
                };

                thankYouMail.To.Add(email);
                await client.SendMailAsync(thankYouMail);
            }
            catch (Exception ex)
            {
                // TODO: log ex (Serilog / ILogger)
            }
        }

    }
}
