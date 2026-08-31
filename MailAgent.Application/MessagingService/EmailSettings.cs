using System;
using System.Collections.Generic;
using System.Text;

namespace MailAgent.Application.MessagingService
{
    public class EmailSettings
    {
        public string SmtpHost { get; set; } = string.Empty;

        public int SmtpPort { get; set; }

        public string SmtpUser { get; set; } = string.Empty;

        public string SmtpPassword { get; set; } = string.Empty;

        public bool UseSsl { get; set; } = true;

        public string SenderName { get; set; } = string.Empty;
    }
}
