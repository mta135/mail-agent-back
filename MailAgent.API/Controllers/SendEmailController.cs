using MailAgent.Application.SendEmailWorker;
using MailAgent.Application.Service;
using MailAgent.Application.Service.Abstract;
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
        private readonly IEmailMessageService _emailMessageService;

        public SendEmailController(IEmailChannel emailChannel, IEmailMessageService emailMessageService)
        {
            _emailChannel = emailChannel;
            _emailMessageService = emailMessageService;
        }

        [HttpPost]
        public async Task<IActionResult> SendEmail([FromForm] EmailMessageModel model)
        {

            await _emailMessageService.SaveInitialMessageAsync(model);


            await _emailChannel.WriteAsync(model);

            //await _emailChannel.Writer.WriteAsync(model);

            return Accepted(new
            {
                Message = "Mesajul a fost pus în coadă.",
                
            });
        }
    }
}
