using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
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

            _resendApiKey = configuration["RESEND_API_KEY"]
           ?? throw new Exception("Resend API key missing");

            _fromEmail = configuration["Resend:FromEmail"]
                ?? throw new Exception("FromEmail missing");

            _adminEmail = configuration["Resend:AdminEmail"]
                ?? throw new Exception("AdminEmail missing");
        }

        //public async Task<dynamic> SavePortfolioEnquiry(PortfolioEnqueryRequest request)
        //{
        //    await using var connection = new NpgsqlConnection(_connectionString);
        //    await connection.OpenAsync();

        //    const string query =
        //        "SELECT * FROM public.set_portfolio_enqueries(@name, @email, @in_message);";

        //    await using var command = new NpgsqlCommand(query, connection);
        //    command.Parameters.AddWithValue("name", request.Name);
        //    command.Parameters.AddWithValue("email", request.Email);
        //    command.Parameters.AddWithValue("in_message", request.Message);

        //    using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(30));
        //    await using var reader = await command.ExecuteReaderAsync(cts.Token);

        //    if (!await reader.ReadAsync(cts.Token))
        //    {
        //        return new
        //        {
        //            status = -1,
        //            message = "Unexpected error while saving enquiry."
        //        };
        //    }

        //    int status = reader.GetInt32(0);
        //    string message = reader.GetString(1);

        //    if (true)
        //    {
        //        var mailResult = await SendMailAsync(
        //            request.Name,
        //            request.Email,
        //            request.Message
        //        );

        //        if (!mailResult.Success)
        //        {
        //            return new
        //            {
        //                status = 0,
        //                message = "Enquiry saved, but email sending failed."
        //            };
        //        }
        //    }

        //    return new
        //    {
        //        status,
        //        message
        //    };
        //}

        // ================= EMAIL SENDER =================


        public async Task<dynamic> SavePortfolioEnquiry(PortfolioEnqueryRequest request)
        {
            try
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
                        message = "Sorry, we were unable to send your message. Please try again later."
                    };
                }

                return new
                {
                    status = 1,
                    message = "Thank you for contacting us! Your message has been sent successfully. We will get back to you soon."
                };
            }
            catch (Exception)
            {
                return new
                {
                    status = 0,
                    message = "Sorry, something went wrong while sending your message. Please try again later."
                };
            }
        }
        private async Task<MailResult> SendMailAsync(string name, string email, string message)
        {
            try
            {
                using var client = new HttpClient();
                client.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", _resendApiKey);

                var adminPayload = new
                {
                    from = _fromEmail,
                    to = new[] { _adminEmail },
                    subject = "New Portfolio Enquiry – Rootstackx",
                    html = $@"
<!DOCTYPE html>
<html>
<head>
    <meta charset='UTF-8'>
</head>

<body style='
    margin:0;
    padding:20px;
    background-color:#0b120d;
    font-family:Georgia, ""Times New Roman"", serif;
'>

    <div style='
        max-width:600px;
        margin:20px auto;
        padding:0;
        background:linear-gradient(145deg, #1b2a1b, #0f1a0f);
        border:1px solid #4a6b4a;
        border-radius:12px;
        box-shadow:0 10px 30px rgba(0,0,0,0.5);
        overflow:hidden;
    '>

        <!-- Header -->
        <div style='
            padding:25px 30px;
            background:#142114;
            border-bottom:1px solid #4a6b4a;
        '>
            <h2 style='
                margin:0;
                color:#a6c25f;
                font-size:24px;
                font-weight:600;
                letter-spacing:0.5px;
            '>
                New Portfolio Enquiry
            </h2>

            <p style='
                margin:8px 0 0 0;
                color:#8fa58f;
                font-size:13px;
            '>
                A new message has been submitted through your portfolio.
            </p>
        </div>

        <!-- Content -->
        <div style='
            padding:30px;
            border-left:5px solid #a6c25f;
        '>

            <p style='
                margin:0 0 18px 0;
                font-size:16px;
                color:#d0d0b0;
            '>
                <strong style='color:#a6c25f;'>Name</strong><br/>
                {System.Net.WebUtility.HtmlEncode(name)}
            </p>

            <p style='
                margin:0 0 18px 0;
                font-size:16px;
                color:#d0d0b0;
            '>
                <strong style='color:#a6c25f;'>Email</strong><br/>
                {System.Net.WebUtility.HtmlEncode(email)}
            </p>

            <div style='
                margin-top:25px;
                padding:20px;
                background:#111d11;
                border:1px solid #344d34;
                border-radius:8px;
            '>

                <p style='
                    margin:0 0 10px 0;
                    font-size:15px;
                    color:#a6c25f;
                    font-weight:bold;
                '>
                    Message
                </p>

                <p style='
                    margin:0;
                    font-size:15px;
                    line-height:1.7;
                    color:#d8d8c0;
                    white-space:pre-line;
                '>
                    {System.Net.WebUtility.HtmlEncode(message)}
                </p>

            </div>

        </div>

        <!-- Footer -->
        <div style='
            padding:18px 30px;
            background:#0d170d;
            border-top:1px dashed #4a6b4a;
            text-align:center;
        '>

            <p style='
                margin:0;
                color:#a6c25f;
                font-size:14px;
                letter-spacing:1px;
            '>
                ✦ Pavan Kumar N ✦
            </p>

            <p style='
                margin:6px 0 0 0;
                color:#718071;
                font-size:12px;
            '>
                Portfolio Enquiry
            </p>

        </div>

    </div>

</body>
</html>"
            };

                await SendResendAsync(client, adminPayload);

               // --------THANK YOU EMAIL --------
               //var thankYouPayload = new
               //{
               //    from = _fromEmail,
               //    to = new[] { email },
               //    subject = "Thank you for contacting",
               //    html = $@"
               //     <div style='
               //         max-width:600px;
               //         margin:20px auto;
               //         padding:30px 40px;
               //         background: linear-gradient(#1b2a1b, #0f1a0f); /* deep forest gradient */
               //         border:2px solid #4a6b4a; /* subtle green border */
               //         border-radius:12px;
               //         box-shadow: 0 10px 25px rgba(0,0,0,0.5);
               //         font-family: ""Garamond"", ""Georgia"", serif;
               //         color:#c0c0a0;
               //     '>
               //         <!-- Inner container with accent line -->
               //         <div style='padding:20px; border-left:6px solid #a6c25f;'>
               //             <p style='font-size:18px; color:#d0d0b0;'>Hi <b>{name}</b>,</p>

               //             <p style='font-size:16px; line-height:1.7; color:#e0e0c0;'>
               //                 Thank you for reaching out.<br/>
               //                 I’ve received your message and will get back to you shortly.
               //             </p>

               //             <p style='font-size:16px; line-height:1.7; color:#d0d0b0;'>
               //                 With regards,<br/>
            
               //             </p>
               //         </div>

               //         <hr style='border:none; border-top:1px dashed #4a6b4a; margin:25px 0;' />

               //         <p style='font-size:14px; color:#a0a080; text-align:center; letter-spacing:1px;'>
               //             ✦ Pavan Kumar N✦
               //         </p>
               //     </div>"
               //};

               // await SendResendAsync(client, thankYouPayload);

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
