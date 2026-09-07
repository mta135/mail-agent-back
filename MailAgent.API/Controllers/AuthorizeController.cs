using MailAgent.Application.GoogleAuthenticationService;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MailAgent.API.Controllers
{
    [ApiController]
    [Route("authorize")]
    public class AuthorizeController : ControllerBase
    {
        private readonly IGoogleAuthService _googleAuthService;
        private readonly ILogger<AuthorizeController> _logger;

        public AuthorizeController(IGoogleAuthService googleAuthService, ILogger<AuthorizeController> logger)
        {
            _googleAuthService = googleAuthService;
            _logger = logger;
        }

        /// <summary>
        /// Pasul 1: Generează link-ul Google și redirecționează utilizatorul.
        /// Deschide această adresă direct în browser pentru a testa ecranul Google.
        /// </summary>
        [HttpGet("connect-google")]
        public IActionResult ConnectGoogle([FromQuery] long userId = 1)
        {
            var authUrl = _googleAuthService.BuildAuthorizationUrl(userId);
            return Redirect(authUrl);
        }

        /// <summary>
        /// Pasul 2: Endpoint-ul înregistrat în consola Google (Redirect URI).
        /// Google trimite aici codul temporar prin query params.
        /// </summary>
        [HttpGet("callback")]
        public async Task<IActionResult> Callback([FromQuery] string code, [FromQuery] string state)
        {
            if (string.IsNullOrEmpty(code))
            {
                return BadRequest("Codul de autorizare lipsește din răspunsul Google.");
            }


            try
            {
                // 1. Schimbă codul pe tokens
                var tokens = await _googleAuthService.ExchangeCodeForTokensAsync(code);

                // 2. Extrage adresa de Gmail conectată
                var userEmail = await _googleAuthService.GetUserEmailAsync(tokens.AccessToken);

                long userId = long.Parse(state);

                // 3. Aici salvezi în baza de date (ex: via DbContext sau Repository):
                // var account = new UserEmailAccount {
                //     UserId = userId,
                //     EmailAddress = userEmail,
                //     Provider = "Google",
                //     RefreshToken = tokens.RefreshToken, // Criptat
                //     AccessToken = tokens.AccessToken,
                //     ExpiresAtUtc = DateTime.UtcNow.AddSeconds(tokens.ExpiresIn)
                // };
                // await _accountRepo.SaveAsync(account, ct);

                return Ok(new
                {
                    Message = "Cont Google conectat cu succes!",
                    UserId = userId,
                    ConnectedEmail = userEmail,
                    HasRefreshToken = !string.IsNullOrEmpty(tokens.RefreshToken),
                    ExpiresInSeconds = tokens.ExpiresIn
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Eroare la autorizare.");
                return StatusCode(500, new { Error = ex.Message });
            }
        }

    }
}



