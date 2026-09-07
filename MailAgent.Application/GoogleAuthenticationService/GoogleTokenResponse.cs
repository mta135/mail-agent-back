using MailAgent.API.Services;

namespace MailAgent.API.Services
{

    public class GoogleTokenResponse
    {
        public string AccessToken { get; }

        public int ExpiresIn { get; }

        public string? RefreshToken { get; }

        public string Scope { get; }

        public string TokenType { get; }

        public GoogleTokenResponse(string accessToken, int expiresIn, string? refreshToken, string scope, string tokenType)
        {
            AccessToken = accessToken;
            ExpiresIn = expiresIn;
            RefreshToken = refreshToken;
            Scope = scope;
            TokenType = tokenType;
        }
    }
}


