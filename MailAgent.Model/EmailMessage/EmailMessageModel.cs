using System;
using System.Collections.Generic;
using System.Text;

namespace MailAgent.Model.EmailMessage
{
    public class EmailMessageModel
    {
        public string From { get; set; } = string.Empty;

        public string Header { get; set; } = string.Empty;

        public string Subject { get; set; } = string.Empty;

        public string Body { get; set; } = string.Empty;

        public string Footer { get; set; } = string.Empty;

        public List<EmailMessageToModel> EmailMessageTo { get; set; } = [];

        public List<EmailMessageAttachmentModel> Attachments { get; set; } = [];

        public List<EmailMessageCopyModel> Copy { get; set; } = [];
    }
}
