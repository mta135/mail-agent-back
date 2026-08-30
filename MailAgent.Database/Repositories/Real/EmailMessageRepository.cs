using MailAgent.Database.DataBaseModel;
using MailAgent.DataBaseAccess.Contex;
using MailAgent.DataBaseAccess.Repositories.Abstract;
using System;
using System.Collections.Generic;
using System.Text;

namespace MailAgent.DataBaseAccess.Repositories.Real
{
    public class EmailMessageRepository : IEmailMessageRepository
    {
        private readonly MailAgentDbContext _db;

        public EmailMessageRepository()
        {
            _db = new MailAgentDbContext();
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
    }
}
