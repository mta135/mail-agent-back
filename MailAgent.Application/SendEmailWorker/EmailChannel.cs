using MailAgent.Model.EmailMessage;
using System;
using System.Collections.Generic;
using System.Net.Mail;
using System.Text;
using System.Threading.Channels;

namespace MailAgent.Application.SendEmailWorker
{
    public class EmailChannel : IEmailChannel
    {

        private readonly Channel<EmailMessageModel> _channel = Channel.CreateBounded<EmailMessageModel>(new BoundedChannelOptions(1000)
        {
            FullMode = BoundedChannelFullMode.Wait, 
            SingleReader = true                    
        });

        public ChannelWriter<EmailMessageModel> Writer => _channel.Writer;

        public ChannelReader<EmailMessageModel> Reader => _channel.Reader;

    }
}
