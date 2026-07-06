using MimeKit;
using System;
using System.Collections.Generic;
using System.Text;
using System;
using MailKit.Net.Smtp;
using MailKit.Security;

namespace Smart_Water_Consumption_Service.Services.MailService
{
    public class EmailSender
    {
        public static async Task SendCode(string recipientEmail, string recipientName, string securityCode)
        {
            var message = new MimeMessage();

            message.From.Add(new MailboxAddress("SmartWater Admin", "lindanilinda2007@gmail.com"));
            message.To.Add(new MailboxAddress(recipientName, recipientEmail));
            message.Subject = "Smart Water Consumption Security Code";

            var bodyBuilder = new BodyBuilder();
            bodyBuilder.HtmlBody = $@"
            <div style='font-family: Arial, sans-serif; padding: 20px; border: 1px solid #ddd; max-width: 500px; border-radius: 8px;'>
                <h2 style='color: #2c3e50;'>Hello {recipientName},</h2>
                <p>Hi, Your requested security code code has been sent to this email please do not share it with anyone..</p>
                
                <div style='background-color: #f8f9fa; padding: 15px; border-left: 4px solid #3498db; margin: 20px 0; text-align: center;'>
                    <span style='font-size: 14px; color: #7f8c8d; display: block; margin-bottom: 5px;'>YOUR SECURITY CODE</span>
                    <strong style='font-size: 28px; letter-spacing: 5px; color: #2980b9;'>{securityCode}</strong>
                </div>

                <p style='font-size: 12px; color: #7f8c8d;'>This is an automated notification sent by Smart water Consumption System.</p>
                <hr style='border: none; border-top: 1px solid #eee;'/>
                <small style='color: #95a5a6;'>Please do not reply directly to this email account.</small>
            </div>";

            message.Body = bodyBuilder.ToMessageBody();

            using (var client = new SmtpClient())
            {
                try
                {
                    await client.ConnectAsync("smtp.gmail.com", 587, SecureSocketOptions.StartTls);
                    await client.AuthenticateAsync("lindanilinda2007@gmail.com", "cxxhuiowszmwopsl");
                    await client.SendAsync(message);

                    Console.WriteLine("Email sent successfully");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Email failed to send. Error: {ex.Message}");
                }
                finally
                {
                    await client.DisconnectAsync(true);
                }
            }
        }
    }
}
