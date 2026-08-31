using MailAgent.Model.EmailMessage;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using System.Text;

namespace MailAgent.Application.MessagingService
{
    public class MailKitEmailSender : IEmailSender
    {
        private readonly EmailSettings _settings;

        public MailKitEmailSender(IOptions<EmailSettings> options)
        {
            _settings = options.Value;
        }

        public async Task SendEmailAsync(EmaiMessagelRequestModel request, CancellationToken cancellationToken = default)
        {

            try
            {
                MimeMessage message = new();

                message.From.Add(new MailboxAddress(_settings.SenderName, _settings.SmtpUser));

                AddToRecipients(message, request.To.Select(x => x.EmailTo).ToList());

                AddCcRecipients(message, request.Copy.Select(x => x.EmailCopy ?? string.Empty).ToList());

                message.Subject = request.Subject;

                BodyBuilder bodyBuilder = new()
                {
                    HtmlBody = CreateHtmlContent(request)
                };
                
                AddAttachments(bodyBuilder, request.Attachments);

                message.Body = bodyBuilder.ToMessageBody();

                using SmtpClient client = new SmtpClient();

                await client.ConnectAsync(_settings.SmtpHost, _settings.SmtpPort, _settings.UseSsl ? SecureSocketOptions.StartTls : SecureSocketOptions.None, cancellationToken);

                await client.AuthenticateAsync(_settings.SmtpUser, _settings.SmtpPassword, cancellationToken);

                await client.SendAsync(message, cancellationToken);

                await client.DisconnectAsync(true, cancellationToken);

            }
            catch (Exception ex)
            {
                // Log the exception or handle it as needed
                throw new InvalidOperationException("Failed to send email.", ex);
            }

        }

        private void AddToRecipients(MimeMessage message, List<string> recipients)
        {
            foreach (var recipient in recipients)
            {
                message.To.Add(MailboxAddress.Parse(recipient));
            }
        }

        private void AddCcRecipients(MimeMessage message, List<string> ccRecipients)
        {
            foreach (var ccRecipient in ccRecipients)
            {
                message.Cc.Add(MailboxAddress.Parse(ccRecipient));
            }
        }


        private void AddAttachments(BodyBuilder bodyBuilder, List<EmailMessageRequestAttachmentModel> attachments)
        {
            foreach (var attachment in attachments)
            {
                bodyBuilder.Attachments.Add(attachment.FileName, attachment.Data, ContentType.Parse(attachment.ContentType ?? "application/octet-stream"));
            }
        }

        private string CreateHtmlContent(EmaiMessagelRequestModel request)
        {
            StringBuilder htmlBuilder = new StringBuilder();
            htmlBuilder.Append("<html><body>");
            htmlBuilder.Append(request.Header);

            htmlBuilder.Append(request.Body);
            htmlBuilder.Append(request.Footer);
            
            htmlBuilder.Append("</body></html>");
            return htmlBuilder.ToString();
        }
    }
}
