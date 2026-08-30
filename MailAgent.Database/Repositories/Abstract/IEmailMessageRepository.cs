using MailAgent.DataBaseAccess.DataScheme;
using System;
using System.Collections.Generic;
using System.Text;

namespace MailAgent.DataBaseAccess.Repositories.Abstract
{
    public interface IEmailMessageRepository
    {
        Task SaveInitialMessageAsync(EmailMessage emailMessage);

        Task SetEmailStatusAsync(Guid emailId, int status);
    }
}
