using MailAgent.DataBaseAccess.Contex;
using MailAgent.DataBaseAccess.DataScheme;
using MailAgent.DataBaseAccess.Repositories.Abstract;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace MailAgent.DataBaseAccess.Repositories.Real
{
    public class EmailMessageRepository : IEmailMessageRepository
    {
        private readonly MailAgentDbContext _db;

        public EmailMessageRepository(MailAgentDbContext db)
        {
            _db = db;
        }

        public async Task SaveInitialMessageAsync(EmailMessage emailMessage)
        {
            try
            {
                await _db.EmailMessages.AddAsync(emailMessage);
                await _db.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw;
            }

        }

        public async Task SetEmailStatusAsync(Guid emailId, int status)
        {
            try
            {
                EmailMessage? emailMessage = await _db.EmailMessages.FirstOrDefaultAsync(em => em.Id == emailId);
                if (emailMessage != null)
                {
                    emailMessage.Status = status;
                    await _db.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
