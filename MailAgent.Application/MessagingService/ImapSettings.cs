using System;
using System.Collections.Generic;
using System.Text;

namespace MailAgent.Application.MessagingService
{
    public class ImapSettings
    {
        public string Host { get; set; } = string.Empty;

        public int Port { get; set; }

        public string Username { get; set; } = string.Empty;

        public string Password { get; set; } = string.Empty;

        public bool UseSsl { get; set; } = true;

    }
}
