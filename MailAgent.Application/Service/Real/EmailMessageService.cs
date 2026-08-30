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

                EmailMessage emailMessage = new EmailMessage
                {
                    Id = emailId,

                    From = email.From,
                    Header = email.Header,

                    Subject = email.Subject,
                    Body = email.Body,
                    Footer = email.Footer,

                    CreatedAt = DateTime.UtcNow,
                    Status = (int)EmailSendStatusEnum.Pending,

                    EmailMessageTos = email.To?.ConvertAll(to => new EmailMessageTo
                    {
                        To = to.ToString(),
                        EmailMessageId = emailId
                    }) ?? [],

                    Copies = email.Copy?.ConvertAll(copy => new EmailMessageCopy
                    {
                        Copy = copy.ToString(),
                        EmailMessageId = emailId
                    }) ?? [],



                };
            

                if (email.Attachments.Count > 0)
                {
                    for (int i = 0; i < email.Attachments.Count; i++)
                    {
                        EmailMessageAttachment emailMessageAttachment = await ProcessFileAsync(email.Attachments[i], emailId);
                        emailMessage.Attachments.Add(emailMessageAttachment);
                    }
                }

             
                await _emailRepository.SaveInitialMessageAsync(emailMessage);
            }
            catch (Exception)
            {
                // propagate so caller/upper layers can observe the underlying DB error (e.g. missing FK column)
                throw;
            }

            return emailId;
        }

        public async Task<EmailMessageAttachment> ProcessFileAsync(IFormFile file, Guid emailId)
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
                EmailMessageId = emailId,
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
