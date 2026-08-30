using System;
using System.Collections.Generic;
using System.Text;

namespace MailAgent.Model.EmailMessage
{
    public class EmailMessageToModel
    {
        public Guid EmailMessageId { get; set; }

        public string To { get; set; } = string.Empty;
    }
}
