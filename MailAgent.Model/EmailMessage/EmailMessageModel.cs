using Microsoft.AspNetCore.Http;
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

        //public List<EmailMessageToModel> EmailMessageTo { get; set; } = [];

        public List<string> To { get; set; } = [];

        //public List<EmailMessageAttachmentModel> Attachments { get; set; } = [];

        public List<string> Copy { get; set; } = [];

        public List<IFormFile> Attachments { get; set; } = new();

        //public List<EmailMessageCopyModel> Copy { get; set; } = [];
    }
}
