using MailAgent.Model.EmailMessage;
using MailKit;
using MailKit.Net.Imap;
using MailKit.Net.Smtp;
using MailKit.Search;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;

using System.Text;

namespace MailAgent.Application.MessagingService
{
    public class MailKitEmailSender : IEmailSender
    {
        private readonly EmailSettings _settings;
        private readonly ImapSettings _imapSettings;

        public MailKitEmailSender(IOptions<EmailSettings> emailOptions, IOptions<ImapSettings> imapOptions)
        {
            _settings = emailOptions.Value;
            _imapSettings = imapOptions.Value;
        }

        #region SendEmailAsync

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


        #endregion

        #region ReceiveEmail

        public async Task ReceiveEmailAsync(CancellationToken cancellationToken = default)
        {
            using var client = new ImapClient();

            try
            {
                // 1. Connect to the secure IMAP server
                await client.ConnectAsync(_imapSettings.Host, _imapSettings.Port, useSsl: _imapSettings.UseSsl, cancellationToken: cancellationToken);

                // 2. Authenticate using credentials
                await client.AuthenticateAsync(_imapSettings.Username, _imapSettings.Password);

                // 3. Open the target mailbox folder (ReadWrite allows altering read/unread flags)
                var inbox = client.Inbox;
                await inbox.OpenAsync(FolderAccess.ReadWrite);

                Console.WriteLine($"Total messages: {inbox.Count}");
                Console.WriteLine($"Recent messages: {inbox.Recent}");

                // 4. Search for specific messages (e.g., all unread emails)
                var query = SearchQuery.NotSeen;
                var uniqueIds = await inbox.SearchAsync(query);

                foreach (var uid in uniqueIds)
                {
                    MimeMessage message = await inbox.GetMessageAsync(uid);

                    // Access email fields safely
                    Console.WriteLine($"-------------------------------------------------");
                    Console.WriteLine($"Subject: {message.Subject}");
                    Console.WriteLine($"From:    {message.From}");
                    Console.WriteLine($"Date:    {message.Date}");
                    Console.WriteLine($"Date:    {message.To}");
                    Console.WriteLine($"Date:    {message.Cc}");
                    Console.WriteLine($"Date:    {message.Attachments}");

                    Console.WriteLine($"Body:    {message.TextBody}");
                    Console.WriteLine($"Body:    {message.HtmlBody}"); 


                    //await inbox.AddFlagsAsync(uid, MessageFlags.Seen, silent: true);
                }

                // 6. Disconnect cleanly from the server
                await client.DisconnectAsync(quit: true);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }
        }

        #endregion


    }

}





