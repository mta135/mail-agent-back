using System;
using System.Collections.Generic;
using System.Text;

namespace MailAgent.DataBaseAccess.DataScheme
{
    public class EmailMessageCopy
    {
        public long Id { get; set; }
        public Guid EmailMessageId { get; set; }
        public string? Copy { get; set; }

        // Navigare circulară către părinte
        public virtual EmailMessage? EmailMessage { get; set; }
    }
}
