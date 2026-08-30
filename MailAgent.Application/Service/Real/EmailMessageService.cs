using MailAgent.Application.Service.Abstract;
using MailAgent.Database.DataBaseModel;
using MailAgent.DataBaseAccess.Repositories.Abstract;
using MailAgent.Model.EmailMessage;
using System;
using System.Collections.Generic;
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



        public async Task SaveInitialMessageAsync(EmailMessageModel email)
        {

            try
            {
                Guid emailId = Guid.NewGuid();

                EmailMessage emailMessage = new()
                {
                    Id = emailId,

                    From = email.From,
                    Subject = email.Subject
                };

                await _emailRepository.SaveInitialMessageAsync(emailMessage);

            } catch(Exception ex)
            {
              
            }

           


            //email.Status = EmailStatus.Pending;
            //email.CreatedAt = DateTime.UtcNow;

            //_logger.LogInformation("Salvare mesaj inițial în baza de date pentru Id: {Id}", email.Id);
            //await _emailRepository.InsertAsync(email);
        }
    }
}
