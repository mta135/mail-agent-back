using MailAgent.Model.EmailMessage;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Channels;

namespace MailAgent.Application.EmailProcessingBackgroundWorker.SendWorker
{
    public interface IEmailChannel
    {

        //ChannelWriter<EmailMessageModel> Writer { get; }

        //ChannelReader<EmailMessageModel> Reader { get; }


        ValueTask WriteAsync(Guid emailId, CancellationToken ct = default);

        IAsyncEnumerable<Guid> ReadAllAsync(CancellationToken ct = default);
    }
}
