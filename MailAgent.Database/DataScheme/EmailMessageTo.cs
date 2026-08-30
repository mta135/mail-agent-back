using System;
using System.Collections.Generic;
using System.Text;

namespace MailAgent.DataBaseAccess.DataScheme
{
    public class EmailMessageTo
    {
        public long Id { get; set; }

        public Guid? EmailMessageId { get; set; }

        public string To { get; set; } = string.Empty;

        public virtual EmailMessage? EmailMessage { get; set; }
    }
}
