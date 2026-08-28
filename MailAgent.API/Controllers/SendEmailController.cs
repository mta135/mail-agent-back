using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MailAgent.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SendEmailController : ControllerBase
    {

        [HttpPost]
        public IActionResult SendEmail()
        {
            return Ok("Email sent successfully!");
        }
    }
}
