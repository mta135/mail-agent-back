using MailAgent.Database.DataBaseModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace MailAgent.DataBaseAccess.Repositories.Abstract
{
    public interface IEmailMessageRepository
    {
        Task SaveInitialMessageAsync(EmailMessage emailMessage);
    }
}
