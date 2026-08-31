using MailAgent.Model.EmailMessage;
using System;
using System.Collections.Generic;
using System.Text;

namespace MailAgent.Application.MessagingService
{
    public interface IEmailSender
    {
        Task SendEmailAsync(EmaiMessagelRequestModel message, CancellationToken cancellationToken = default);
    }
}
