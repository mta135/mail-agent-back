 using System;
using System.Collections.Generic;
using System.Text;

namespace MailAgent.Model.EmailMessage
{
    public class EmailMessageAttachmentModel
    {
        public string FileName { get; set; } = string.Empty;

        public string? ContentType { get; set; }

        public long? FileSizeBytes { get; set; }

        public byte[] Data { get; set; } = [];

        public DateTime? CreateDate { get; set; }

    }
}
