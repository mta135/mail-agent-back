using MailAgent.API.Services;

namespace MailAgent.API.Services
{
    public record GoogleTokenResponse(
      string AccessToken,
      int ExpiresIn,
      string? RefreshToken,
      string Scope,
      string TokenType);
}


public interface IGoogleAuthService
{
    string BuildAuthorizationUrl(long userId);
    Task<GoogleTokenResponse> ExchangeCodeForTokensAsync(string code, CancellationToken ct = default);

    Task<string> GetUserEmailAsync(string accessToken, CancellationToken ct = default);
}