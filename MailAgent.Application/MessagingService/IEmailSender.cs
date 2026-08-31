using MailAgent.Model.EmailMessage;
using System;
using System.Collections.Generic;
using System.Text;

namespace MailAgent.Application.MessagingService
{
    public interface IEmailSender
    {
        Task<Tuple<bool, string>> SendEmailAsync(EmaiMessagelRequestModel message, CancellationToken cancellationToken = default);
    }
}
