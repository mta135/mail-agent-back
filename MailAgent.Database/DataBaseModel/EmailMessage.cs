using System;
using System.Collections.Generic;
using System.Text;

namespace MailAgent.Database.DataBaseModel
{
    public class EmailMessage
    {
        public Guid Id { get; set; }

        public string From { get; set; } = string.Empty;

        public string Subject { get; set; } = string.Empty;
    }
}
