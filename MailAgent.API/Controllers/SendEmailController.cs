using MailAgent.Application.SendEmailWorker;
using MailAgent.Model.EmailMessage;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MailAgent.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SendEmailController : ControllerBase
    {
        private readonly IEmailChannel _emailChannel;

        public SendEmailController(IEmailChannel emailChannel)
        {
            _emailChannel = emailChannel;
        }

        [HttpPost]
        public async Task<IActionResult> SendEmail(EmailMessageModel model)
        {
            await _emailChannel.WriteAsync(model);

            //await _emailChannel.Writer.WriteAsync(model);

            return Accepted(new
            {
                Message = "Mesajul a fost pus în coadă.",
                
            });
        }
    }
}
