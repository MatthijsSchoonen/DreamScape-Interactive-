using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using MailKit.Net.Smtp;
using MimeKit;

namespace DreamScape.Helpers
{
    class EmailHelper
    {
        private const string SenderEmail = "interactivedreamscape@gmail.com";
        private const string SenderName = "DreamScape Interactive";
        private const string SmtpServer = "smtp.gmail.com";
        private const int SmtpPort = 587;
        private const string SmtpPassword = "scvf niad qiea tnwj"; // Consider storing this securely

        public async Task ConfirmAccountCreation(string recipientEmail)
        {
            MimeMessage message = new MimeMessage();
            message.From.Add(new MailboxAddress(SenderName, SenderEmail));
            message.To.Add(new MailboxAddress("", recipientEmail));
            message.Subject = "Account Creation";

            // Build email body
            BodyBuilder bodyBuilder = new BodyBuilder
            {
                HtmlBody = @"
                    <html>
                        <body>
                            <h1>Welcome to DreamScape Interactive!</h1>
                            <p>Thank you for creating an account on DreamScape Interactive.</p>
                            <img src=""cid:logo"" alt=""DreamScape Logo"" />
                        </body>
                    </html>",
                TextBody = "Thank you for creating an account on DreamScape Interactive."
            };

            // Add logo image
            string logoPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets", "Logo.png");
            var logo = bodyBuilder.LinkedResources.Add(logoPath);
            logo.ContentId = "logo";

            message.Body = bodyBuilder.ToMessageBody();

            await SendEmailAsync(message);
        }

        public async Task SendPasswordReset(string recipientEmail, string Code)
        {
            MimeMessage message = new MimeMessage();
            message.From.Add(new MailboxAddress(SenderName, SenderEmail));
            message.To.Add(new MailboxAddress("", recipientEmail));
            message.Subject = "Password Reset";

            // Build email body
            BodyBuilder bodyBuilder = new BodyBuilder
            {
                HtmlBody = $@"
                    <html>
                        <body>
                            <h1>Password Reset Request</h1>
                            <p>Use this Code to Reset Your Password</p>
                            <p>{Code}</p>
                        </body>
                    </html>",
                TextBody = $"Use this Code to Reset Your Password {Code}"
            };

            message.Body = bodyBuilder.ToMessageBody();

            await SendEmailAsync(message);
        }

        public async Task SendWarningEmail(string recipientEmail)
        {
            MimeMessage message = new MimeMessage();
            message.From.Add(new MailboxAddress(SenderName, SenderEmail));
            message.To.Add(new MailboxAddress("", recipientEmail));
            message.Subject = "Security Warning";

            // Build email body
            BodyBuilder bodyBuilder = new BodyBuilder
            {
                HtmlBody = @"
                    <html>
                        <body>
                            <h1>Security Warning</h1>
                            <p>Someone tried to log in to your account and failed. If this wasn't you, please click on 'Forgot Password' in the app to reset your password.</p>
                        </body>
                    </html>",
                TextBody = "Someone tried to log in to your account and failed. If this wasn't you, please click on 'Forgot Password' in the app to reset your password."
            };

            message.Body = bodyBuilder.ToMessageBody();

            await SendEmailAsync(message);
        }

        private async Task SendEmailAsync(MimeMessage message)
        {
            try
            {
                using (var client = new MailKit.Net.Smtp.SmtpClient())
                {
                    await client.ConnectAsync(SmtpServer, SmtpPort, MailKit.Security.SecureSocketOptions.StartTls);
                    await client.AuthenticateAsync(SenderEmail, SmtpPassword);
                    await client.SendAsync(message);
                    await client.DisconnectAsync(true);
                }

                Console.WriteLine("Email sent successfully!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error sending email: {ex.Message}");
                throw;
            }
        }

        public bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }
    }
}