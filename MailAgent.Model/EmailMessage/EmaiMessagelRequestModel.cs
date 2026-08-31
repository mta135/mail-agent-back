using System;
using System.Collections.Generic;
using System.Text;

namespace MailAgent.Model.EmailMessage
{
    public class EmaiMessagelRequestModel
    {
        public string From { get; set; } = string.Empty;

        public string Header { get; set; } = string.Empty;

        public string Subject { get; set; } = string.Empty;

        public string Body { get; set; } = string.Empty;

        public string Footer { get; set; } = string.Empty;

        public List<EmailMessageRequestToModel> To { get; set; } = new();

        public List<EmailMessageRequestCopyModel> Copy { get; set; } = new();

        public List<EmailMessageRequestAttachmentModel> Attachments { get; set; } = new();

    }


    public class EmailMessageRequestToModel
    {
        public string EmailTo { get; set; } = string.Empty;
    }


    public class EmailMessageRequestAttachmentModel
    {
        public string FileName { get; set; } = string.Empty;

        public string? ContentType { get; set; }

        public long? FileSizeBytes { get; set; }

        public byte[] Data { get; set; } = [];



    }

    public class EmailMessageRequestCopyModel
    {

        public string? EmailCopy { get; set; }

    }
}
