using MailAgent.Model.EmailMessage;
using System;
using System.Collections.Generic;
using System.Text;

namespace MailAgent.Application.Service.Abstract
{
    public interface IEmailMessageService
    {
        Task<Guid> SaveInitialMessageAsync(EmailMessageModel email);
    }
}
