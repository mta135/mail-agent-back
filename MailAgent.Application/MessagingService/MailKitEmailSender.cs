using MailAgent.Model.EmailMessage;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
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

        public async Task<Tuple<bool, string>> SendEmailAsync(EmaiMessagelRequestModel request, CancellationToken cancellationToken = default)
        {
            MimeMessage message = SetMimeMessage(request);

            using SmtpClient smtpClient = await CreateSmtpClient(cancellationToken);
            return await SendAndDisconnectEmailMessageAsync(smtpClient, message, cancellationToken);

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

        private MimeMessage SetMimeMessage(EmaiMessagelRequestModel request)
        {
            MimeMessage message = new MimeMessage();

            message.From.Add(new MailboxAddress(_settings.SenderName, _settings.SmtpUser));

            List<string> toRecipients = request.To.Select(x => x.EmailTo).ToList();
            AddToRecipients(message, toRecipients);

            List<string> ccRecipients = request.Copy.Select(x => x.EmailCopy ?? string.Empty).ToList();
            AddCcRecipients(message, ccRecipients);

            message.Subject = request.Subject;
            
            message.Body = CreateBodyBuilder(request).ToMessageBody();
            return message;
        }

        BodyBuilder CreateBodyBuilder(EmaiMessagelRequestModel request)
        {
            BodyBuilder bodyBuilder = new BodyBuilder
            {
                HtmlBody = CreateHtmlContent(request)
            };

            AddAttachments(bodyBuilder, request.Attachments);
            return bodyBuilder;
        }

        private async Task<SmtpClient> CreateSmtpClient(CancellationToken cancellationToken)
        {
            SmtpClient client = new SmtpClient();

            await client.ConnectAsync(_settings.SmtpHost, _settings.SmtpPort, _settings.UseSsl ? SecureSocketOptions.StartTls : SecureSocketOptions.None, cancellationToken);

            await client.AuthenticateAsync(_settings.SmtpUser, _settings.SmtpPassword, cancellationToken);

            return client;
        }

        private async Task<Tuple<bool, string>> SendAndDisconnectEmailMessageAsync(SmtpClient smtpClient, MimeMessage mailMessage, CancellationToken cancellationToken)
        {
            try
            {
                var value = await smtpClient.SendAsync(mailMessage);
                await smtpClient.DisconnectAsync(true, cancellationToken);

                return new Tuple<bool, string>(true, string.Empty);
            }
            catch (Exception ex)
            {
                return new Tuple<bool, string>(false, ex.ToString());
            }
        }

    }
}
