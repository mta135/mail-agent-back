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
        private readonly Channel<Guid> _channel = Channel.CreateBounded<Guid>(new BoundedChannelOptions(1000)
        {
            FullMode = BoundedChannelFullMode.Wait,
            SingleReader = true
        });

        //public ChannelWriter<EmailMessageModel> Writer
        //{
        //    get { return _channel.Writer; }
        //}


        //public ChannelReader<EmailMessageModel> Reader
        //{
        //    get { return _channel.Reader; }
        //}




        public ValueTask WriteAsync(Guid message, CancellationToken ct = default)
        {
            return _channel.Writer.WriteAsync(message, ct);
        }


        public IAsyncEnumerable<Guid> ReadAllAsync(CancellationToken ct = default)
        {
            return _channel.Reader.ReadAllAsync(ct);
        }

   
    }
}
