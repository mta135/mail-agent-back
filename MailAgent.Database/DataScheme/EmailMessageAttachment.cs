using System;
using System.Collections.Generic;
using System.Text;

namespace MailAgent.DataBaseAccess.DataScheme
{
    public class EmailMessageAttachment
    {
        public long Id { get; set; }

        public Guid? EmailMessageId { get; set; }

        public string FileName { get; set; } = string.Empty;

        public string? ContentType { get; set; }

        public long? FileSizeBytes { get; set; }

        public byte[] Data { get; set; } = [];

        public DateTime? CreateDate { get; set; }


        // Navigare circulară către părinte
        public virtual EmailMessage? EmailMessage { get; set; }
    }
}
