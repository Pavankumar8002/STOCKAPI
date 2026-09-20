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
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Configuration;
using MimeKit;

namespace STOCKWEBAPI.Repository.Portfolio
{
    public class PortfolioEnqueryRepo : IPortfolioEnqueryRepo
    {
        private readonly string _connectionString;
        //private readonly string _resendApiKey;
        //private readonly string _fromEmail;
        //private readonly string _adminEmail;
        private readonly string _gmailEmail;
        private readonly string _gmailAppPassword;
        private readonly string _adminEmail;

        public PortfolioEnqueryRepo(IConfiguration configuration)
        {
            _gmailEmail = configuration["Gmail:Email"]
      ?? throw new InvalidOperationException("Gmail:Email is missing.");

            _gmailAppPassword = configuration["Gmail:AppPassword"]
                ?? throw new InvalidOperationException("Gmail:AppPassword is missing.");

            _adminEmail = configuration["Gmail:AdminEmail"]
                ?? throw new InvalidOperationException("Gmail:AdminEmail is missing.");
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
                        message = mailResult.ErrorMessage
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
        //private async Task<MailResult> SendMailAsync(string name, string email, string message)
        //{
        //    try
        //    {
        //        using var client = new HttpClient();
        //        client.DefaultRequestHeaders.Authorization =
        //            new AuthenticationHeaderValue("Bearer", _resendApiKey);

        //        // -------- ADMIN EMAIL --------
        //        var adminPayload = new
        //        {
        //            from = _fromEmail,
        //            to = new[] { _adminEmail },
        //            subject = "New Portfolio Enquiry – Rootstackx",
        //            html = $@"
        //                <h3>New Portfolio Enquiry</h3>
        //                <p><b>Name:</b> {name}</p>
        //                <p><b>Email:</b> {email}</p>
        //                <p><b>Message:</b><br/>{message}</p>"
        //        };

        //        await SendResendAsync(client, adminPayload);

        //       // --------THANK YOU EMAIL --------
        //       var thankYouPayload = new
        //       {
        //           from = _fromEmail,
        //           to = new[] { email },
        //           subject = "Thank you for contacting",
        //           html = $@"
        //            <div style='
        //                max-width:600px;
        //                margin:20px auto;
        //                padding:30px 40px;
        //                background: linear-gradient(#1b2a1b, #0f1a0f); /* deep forest gradient */
        //                border:2px solid #4a6b4a; /* subtle green border */
        //                border-radius:12px;
        //                box-shadow: 0 10px 25px rgba(0,0,0,0.5);
        //                font-family: ""Garamond"", ""Georgia"", serif;
        //                color:#c0c0a0;
        //            '>
        //                <!-- Inner container with accent line -->
        //                <div style='padding:20px; border-left:6px solid #a6c25f;'>
        //                    <p style='font-size:18px; color:#d0d0b0;'>Hi <b>{name}</b>,</p>

        //                    <p style='font-size:16px; line-height:1.7; color:#e0e0c0;'>
        //                        Thank you for reaching out.<br/>
        //                        I’ve received your message and will get back to you shortly.
        //                    </p>

        //                    <p style='font-size:16px; line-height:1.7; color:#d0d0b0;'>
        //                        With regards,<br/>

        //                    </p>
        //                </div>

        //                <hr style='border:none; border-top:1px dashed #4a6b4a; margin:25px 0;' />

        //                <p style='font-size:14px; color:#a0a080; text-align:center; letter-spacing:1px;'>
        //                    ✦ Pavan Kumar N✦
        //                </p>
        //            </div>"
        //       };

        //        await SendResendAsync(client, thankYouPayload);

        //        return MailResult.SuccessResult();
        //    }
        //    catch (Exception ex)
        //    {
        //        return MailResult.FailureResult(ex.Message);
        //    }
        //}

        //private static async Task SendResendAsync(HttpClient client, object payload)
        //{
        //    var content = new StringContent(
        //        JsonSerializer.Serialize(payload),
        //        Encoding.UTF8,
        //        "application/json"
        //    );

        //    var response = await client.PostAsync(
        //        "https://api.resend.com/emails",
        //        content
        //    );

        //    if (!response.IsSuccessStatusCode)
        //    {
        //        var error = await response.Content.ReadAsStringAsync();
        //        throw new Exception($"Resend API error: {error}");
        //    }
        //}

        //private async Task<MailResult> SendMailAsync(string name,string email,string message)
        //{
        //    try
        //    {
        //        // ============================================================
        //        // ADMIN EMAIL
        //        // ============================================================

        //        var adminEmail = new MimeMessage();

        //        adminEmail.From.Add(
        //            new MailboxAddress(
        //                "Pavan Kumar",
        //                _gmailEmail
        //            )
        //        );

        //        adminEmail.To.Add(
        //            MailboxAddress.Parse(_adminEmail)
        //        );

        //        // When you click Reply, Gmail will reply directly to
        //        // the person who submitted the enquiry.
        //        adminEmail.ReplyTo.Add(
        //            MailboxAddress.Parse(email)
        //        );

        //        adminEmail.Subject = "New Portfolio Enquiry – Rootstackx";

        //        var adminBody = new BodyBuilder
        //        {
        //            HtmlBody = $"""
        //        <h3>New Portfolio Enquiry</h3>

        //        <p>
        //            <b>Name:</b> {name}
        //        </p>

        //        <p>
        //            <b>Email:</b> {email}
        //        </p>

        //        <p>
        //            <b>Message:</b><br/>
        //            {message}
        //        </p>
        //        """
        //        };

        //        adminEmail.Body = adminBody.ToMessageBody();


        //        // ============================================================
        //        // THANK YOU EMAIL
        //        // ============================================================

        //        var thankYouEmail = new MimeMessage();

        //        thankYouEmail.From.Add(
        //            new MailboxAddress(
        //                "Pavan Kumar",
        //                _gmailEmail
        //            )
        //        );

        //        thankYouEmail.To.Add(
        //            MailboxAddress.Parse(email)
        //        );

        //        thankYouEmail.Subject = "Thank you for contacting Pavan Kumar";

        //        var thankYouBody = new BodyBuilder
        //        {
        //            HtmlBody = $"""
        //        <div style="
        //            max-width:600px;
        //            margin:20px auto;
        //            padding:30px 40px;
        //            background:linear-gradient(#1b2a1b,#0f1a0f);
        //            border:2px solid #4a6b4a;
        //            border-radius:12px;
        //            box-shadow:0 10px 25px rgba(0,0,0,0.5);
        //            font-family:Garamond, Georgia, serif;
        //            color:#c0c0a0;
        //        ">

        //            <div style="
        //                padding:20px;
        //                border-left:6px solid #a6c25f;
        //            ">

        //                <p style="
        //                    font-size:18px;
        //                    color:#d0d0b0;
        //                ">
        //                    Hi <b>{name}</b>,
        //                </p>

        //                <p style="
        //                    font-size:16px;
        //                    line-height:1.7;
        //                    color:#e0e0c0;
        //                ">
        //                    Thank you for reaching out.<br/>
        //                    I've received your message and will get back to you shortly.
        //                </p>

        //                <p style="
        //                    font-size:16px;
        //                    line-height:1.7;
        //                    color:#d0d0b0;
        //                ">
        //                    With regards,<br/>
        //                    Pavan Kumar N
        //                </p>

        //            </div>

        //            <hr style="
        //                border:none;
        //                border-top:1px dashed #4a6b4a;
        //                margin:25px 0;
        //            "/>

        //            <p style="
        //                font-size:14px;
        //                color:#a0a080;
        //                text-align:center;
        //                letter-spacing:1px;
        //            ">
        //                ✦ Pavan Kumar N ✦
        //            </p>

        //        </div>
        //        """
        //        };

        //        thankYouEmail.Body = thankYouBody.ToMessageBody();


        //        // ============================================================
        //        // GMAIL SMTP
        //        // ============================================================

        //        using var smtp = new SmtpClient();

        //        smtp.ServerCertificateValidationCallback =
        //            (sender, certificate, chain, sslPolicyErrors) =>
        //            {
        //                Console.WriteLine($"SSL Policy Errors: {sslPolicyErrors}");

        //                if (chain != null)
        //                {
        //                    foreach (var status in chain.ChainStatus)
        //                    {
        //                        Console.WriteLine(
        //                            $"Chain Status: {status.Status} - {status.StatusInformation}"
        //                        );
        //                    }
        //                }

        //                // Your Mac/.NET is failing only because of
        //                // certificate revocation checking.
        //                if (sslPolicyErrors == System.Net.Security.SslPolicyErrors.RemoteCertificateChainErrors)
        //                {
        //                    if (chain?.ChainStatus != null &&
        //                        chain.ChainStatus.All(x =>
        //                            x.Status == System.Security.Cryptography.X509Certificates.X509ChainStatusFlags.RevocationStatusUnknown ||
        //                            x.Status == System.Security.Cryptography.X509Certificates.X509ChainStatusFlags.OfflineRevocation))
        //                    {
        //                        return true;
        //                    }
        //                }

        //                // Accept a completely valid certificate.
        //                if (sslPolicyErrors == System.Net.Security.SslPolicyErrors.None)
        //                {
        //                    return true;
        //                }

        //                return false;
        //            };

        //        await smtp.ConnectAsync(
        //            "smtp.gmail.com",
        //            587,
        //            SecureSocketOptions.StartTls
        //        );

        //        await smtp.AuthenticateAsync(
        //            _gmailEmail,
        //            _gmailAppPassword
        //        );

        //        await smtp.SendAsync(adminEmail);
        //        await smtp.SendAsync(thankYouEmail);

        //        await smtp.DisconnectAsync(true);


        //        // Send enquiry to you
        //        await smtp.SendAsync(adminEmail);

        //        // Send acknowledgement to visitor
        //        await smtp.SendAsync(thankYouEmail);


        //        await smtp.DisconnectAsync(true);

        //        return MailResult.SuccessResult();
        //    }

        //    catch (Exception ex)
        //    {
        //        Console.WriteLine("EMAIL ERROR:");
        //        Console.WriteLine(ex.ToString());

        //        return MailResult.FailureResult(ex.ToString());
        //    }
        //}

        private async Task<MailResult> SendMailAsync(string name,string email,string message)
        {
            try
            {
               

                var adminEmail = new MimeMessage();

                adminEmail.From.Add(new MailboxAddress("Pavan Kumar",_gmailEmail));

                adminEmail.To.Add(MailboxAddress.Parse(_adminEmail));

                adminEmail.ReplyTo.Add(
                    MailboxAddress.Parse(email)
                );

                adminEmail.Subject = "New Portfolio Enquiry – Rootstackx";

                var adminBody = new BodyBuilder
                {
                    HtmlBody = $"""
                <h3>New Portfolio Enquiry</h3>

                <p>
                    <b>Name:</b> {name}
                </p>

                <p>
                    <b>Email:</b> {email}
                </p>

                <p>
                    <b>Message:</b><br/>
                    {message}
                </p>
                """
                };

                adminEmail.Body = adminBody.ToMessageBody();

                var thankYouEmail = new MimeMessage();

                thankYouEmail.From.Add(new MailboxAddress("Pavan Kumar",_gmailEmail));

                thankYouEmail.To.Add(MailboxAddress.Parse(email));

                thankYouEmail.Subject ="Thank you for contacting Pavan Kumar";

                var thankYouBody = new BodyBuilder
                {
                    HtmlBody = $"""
                <div style="
                    max-width:600px;
                    margin:20px auto;
                    padding:30px 40px;
                    background:linear-gradient(#1b2a1b,#0f1a0f);
                    border:2px solid #4a6b4a;
                    border-radius:12px;
                    box-shadow:0 10px 25px rgba(0,0,0,0.5);
                    font-family:Garamond, Georgia, serif;
                    color:#c0c0a0;
                ">

                    <div style="
                        padding:20px;
                        border-left:6px solid #a6c25f;
                    ">

                        <p style="
                            font-size:18px;
                            color:#d0d0b0;
                        ">
                            Hi <b>{name}</b>,
                        </p>

                        <p style="
                            font-size:16px;
                            line-height:1.7;
                            color:#e0e0c0;
                        ">
                            Thank you for reaching out.<br/>
                            I've received your message and will get back to you shortly.
                        </p>

                        <p style="
                            font-size:16px;
                            line-height:1.7;
                            color:#d0d0b0;
                        ">
                            With regards,<br/>
                            Pavan Kumar N
                        </p>

                    </div>

                    <hr style="
                        border:none;
                        border-top:1px dashed #4a6b4a;
                        margin:25px 0;
                    "/>

                    <p style="
                        font-size:14px;
                        color:#a0a080;
                        text-align:center;
                        letter-spacing:1px;
                    ">
                        ✦ Pavan Kumar N ✦
                    </p>

                </div>
                """
                };

                thankYouEmail.Body = thankYouBody.ToMessageBody();

                using var smtp = new SmtpClient();
                smtp.ServerCertificateValidationCallback =
                    (sender, certificate, chain, sslPolicyErrors) =>
                    {
                        Console.WriteLine(
                            $"SSL Policy Errors: {sslPolicyErrors}"
                        );

                        if (chain != null)
                        {
                            foreach (var status in chain.ChainStatus)
                            {
                                Console.WriteLine(
                                    $"Chain Status: {status.Status} - " +
                                    $"{status.StatusInformation}"
                                );
                            }
                        }

                       
                        if (sslPolicyErrors ==
                            System.Net.Security.SslPolicyErrors.None)
                        {
                            return true;
                        }

                
                        if (sslPolicyErrors ==
                            System.Net.Security.SslPolicyErrors.RemoteCertificateChainErrors &&
                            chain?.ChainStatus != null)
                        {
                            bool onlyRevocationErrors =
                                chain.ChainStatus.All(status =>
                                    status.Status ==
                                        System.Security.Cryptography.X509Certificates
                                            .X509ChainStatusFlags.RevocationStatusUnknown
                                    ||
                                    status.Status ==
                                        System.Security.Cryptography.X509Certificates
                                            .X509ChainStatusFlags.OfflineRevocation
                                );

                            if (onlyRevocationErrors)
                            {
                                return true;
                            }
                        }

                        return false;
                    };


                // ============================================================
                // CONNECT
                // ============================================================

                await smtp.ConnectAsync(
         "smtp.gmail.com",
         465,
         SecureSocketOptions.SslOnConnect
     );

                // ============================================================
                // AUTHENTICATE
                // ============================================================

                await smtp.AuthenticateAsync(
                    _gmailEmail,
                    _gmailAppPassword
                );


                // ============================================================
                // SEND ADMIN EMAIL
                // ============================================================

                await smtp.SendAsync(adminEmail);

                Console.WriteLine(
                    "Admin email sent successfully."
                );


                // ============================================================
                // SEND THANK YOU EMAIL
                // ============================================================

                await smtp.SendAsync(thankYouEmail);

                Console.WriteLine(
                    "Thank-you email sent successfully."
                );


                // ============================================================
                // DISCONNECT ONCE
                // ============================================================

                await smtp.DisconnectAsync(true);

                Console.WriteLine(
                    "Gmail SMTP disconnected successfully."
                );


                return MailResult.SuccessResult();
            }
            catch (Exception ex)
            {
                Console.WriteLine("========== EMAIL ERROR ==========");
                Console.WriteLine(ex.ToString());
                Console.WriteLine("================================");

                return MailResult.FailureResult(
                    ex.ToString()
                );
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
