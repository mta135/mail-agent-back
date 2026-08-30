using System;
using System.Collections.Generic;
using System.Text;

namespace MailAgent.Model.EmailMessage
{
    public class EmailMessageModel
    {
        public string From { get; set; } = string.Empty;

        //public List<string> To { get; set; } = [];

        public string Subject { get; set; } = string.Empty;

        //public string Body { get; set; } = string.Empty;
    }
}
