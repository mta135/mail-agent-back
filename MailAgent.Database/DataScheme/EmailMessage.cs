using System;
using System.Collections.Generic;
using System.Text;

namespace MailAgent.DataBaseAccess.DataScheme
{
    public class EmailMessage
    {
        public Guid Id { get; set; }

        public string From { get; set; } = string.Empty;

        public string Header { get; set; } = string.Empty;

        public string Subject { get; set; } = string.Empty;

        public string Body { get; set; } = string.Empty;

        public string Footer { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public int Status { get; set; } = 0; // 0 - Pending, 1 - Sent, 2 - Failed


        public virtual ICollection<EmailMessageTo> EmailMessageTos { get; set; } = new List<EmailMessageTo>();

        public virtual ICollection<EmailMessageAttachment> Attachments { get; set; } = new List<EmailMessageAttachment>();

    }
}
