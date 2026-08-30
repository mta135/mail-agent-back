 using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using Microsoft.AspNetCore.Http;

namespace MailAgent.Model.EmailMessage
{
    public class EmailMessageAttachmentModel
    {
        public string FileName { get; set; } = string.Empty;

        public IFormFile File { get; set; } = null!;

    }
}
