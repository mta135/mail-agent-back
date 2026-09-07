using MailAgent.API.Services;
using System;
using System.Collections.Generic;
using System.Text;

namespace MailAgent.Application.GoogleAuthenticationService
{
    public interface IGoogleAuthService
    {
        string BuildAuthorizationUrl(long userId);
        Task<GoogleTokenResponse> ExchangeCodeForTokensAsync(string code);

        Task<string> GetUserEmailAsync(string accessToken);
    }
}
