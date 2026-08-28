using MailAgent.Model.EmailMessage;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Channels;

namespace MailAgent.Application.SendEmailWorker
{
    public interface IEmailChannel
    {
        ChannelWriter<EmailMessageModel> Writer { get; }
        ChannelReader<EmailMessageModel> Reader { get; }
    }
}
