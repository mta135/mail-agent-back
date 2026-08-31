using MailAgent.Model.EmailMessage;
using System;
using System.Collections.Generic;
using System.Text;

namespace MailAgent.Application.MessagingService
{
    public interface IEmailSender
    {
        Task SendAsync(EmaiMessagelRequestModel message, CancellationToken cancellationToken = default);
    }
}
