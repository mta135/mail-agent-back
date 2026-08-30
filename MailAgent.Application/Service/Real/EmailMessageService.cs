using MailAgent.Application.Service.Abstract;
using MailAgent.DataBaseAccess.DataScheme;
using MailAgent.DataBaseAccess.Repositories.Abstract;
using MailAgent.Model.EmailMessage;
using MailAgent.Model.Enums;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Net.Mail;
using System.Text;

namespace MailAgent.Application.Service
{
    public class EmailMessageService : IEmailMessageService
    {
        private readonly IEmailMessageRepository _emailRepository;

        public EmailMessageService(IEmailMessageRepository emailRepository)
        {
            _emailRepository = emailRepository;
        }

        public async Task<Guid> SaveInitialMessageAsync(EmailMessageModel email)
        {
            Guid emailId = Guid.Empty;

            try
            {
                emailId = Guid.NewGuid();

                EmailMessage emailMessage = new EmailMessage();

                emailMessage.Id = emailId;

                emailMessage.From = email.From;
                emailMessage.Header = email.Header;

                emailMessage.Subject = email.Subject;
                emailMessage.Body = email.Body;
                emailMessage.Footer = email.Footer;

                emailMessage.CreatedAt = DateTime.UtcNow;
                emailMessage.Status = (int)EmailSendStatusEnum.Pending;

                emailMessage.EmailMessageTos = email.EmailMessageTo.ConvertAll(to => new EmailMessageTo()
                {
                    To = to.To
                });

                if (email.Copy.Count > 0)
                {
                    emailMessage.Copies = email.Copy.ConvertAll(copy => new EmailMessageCopy
                    {
                        Copy = copy.Copy
                    });
                }

                if(email.Attachments.Count > 0)
                {
                    for(int i = 0; i < email.Attachments.Count; i++)
                    {
                        EmailMessageAttachmentModel attachments = email.Attachments[i];
                        EmailMessageAttachment emailMessageAttachment = await ProcessFileAsync(attachments.File);

                        emailMessage.Attachments.Add(emailMessageAttachment);
                    }
                }

                await _emailRepository.SaveInitialMessageAsync(emailMessage);

            } catch(Exception ex)
            {
                string errorMessage = $"Error saving initial message: {ex.Message}";
            }

            return emailId;
        }

        public async Task<EmailMessageAttachment> ProcessFileAsync(IFormFile file)
        {

            string contentType = file.ContentType;

            long fileSizeBytes = file.Length;

            byte[] data;
            using (var memoryStream = new MemoryStream())
            {
                await file.CopyToAsync(memoryStream);
                data = memoryStream.ToArray();
            }

            // Numele fișierului dacă ai nevoie de file_name:
            string fileName = Path.GetFileName(file.FileName);

            // Mapezi direct în entitatea ta de EF Core:
            var attachmentEntity = new EmailMessageAttachment
            {
                FileName = fileName,
                ContentType = contentType,

                FileSizeBytes = fileSizeBytes,
                Data = data,
                CreateDate = DateTime.UtcNow
            };

            return attachmentEntity;
        }
    }
}
