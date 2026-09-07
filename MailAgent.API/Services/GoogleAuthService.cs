using Microsoft.Extensions.Options;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace MailAgent.API.Services
{
    public class GoogleAuthService : IGoogleAuthService
    {
        private readonly GoogleOAuthOptions _options;
        private readonly IHttpClientFactory _httpClientFactory;

        public GoogleAuthService(IOptions<GoogleOAuthOptions> options, IHttpClientFactory httpClientFactory)
        {
            _options = options.Value;
            _httpClientFactory = httpClientFactory;
        }

        public string BuildAuthorizationUrl(long userId)
        {
            // access_type=offline -> Google generează Refresh Token
            // prompt=consent      -> Forțează afișarea ecranului de aprobare (pentru a primi sigur Refresh Token)
            // state               -> Trimitem userId-ul criptat sau ca identificator pentru a ști al cui cont este la callback
            var parameters = new Dictionary<string, string>
            {
                ["client_id"] = _options.ClientId,
                ["redirect_uri"] = _options.RedirectUri,
                ["response_type"] = "code",
                ["scope"] = _options.Scope,
                ["access_type"] = "offline",
                ["prompt"] = "consent",
                ["state"] = userId.ToString()
            };

            var queryString = string.Join("&", parameters.Select(p => $"{p.Key}={Uri.EscapeDataString(p.Value)}"));
            return $"https://accounts.google.com/o/oauth2/v2/auth?{queryString}";
        }

        public async Task<GoogleTokenResponse> ExchangeCodeForTokensAsync(string code, CancellationToken ct = default)
        {
            var client = _httpClientFactory.CreateClient();

            var body = new Dictionary<string, string>
            {
                ["code"] = code,
                ["client_id"] = _options.ClientId,
                ["client_secret"] = _options.ClientSecret,
                ["redirect_uri"] = _options.RedirectUri,
                ["grant_type"] = "authorization_code"
            };

            var response = await client.PostAsync("https://oauth2.googleapis.com/token", new FormUrlEncodedContent(body), ct);

            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync(ct);
            var tokenResult = JsonSerializer.Deserialize<GoogleTokenPayload>(json);

            if (tokenResult is null || string.IsNullOrEmpty(tokenResult.AccessToken))
            {
                throw new InvalidOperationException("Răspunsul de la Google nu conține un token valid.");
            }

            return new GoogleTokenResponse(tokenResult.AccessToken, tokenResult.ExpiresIn, tokenResult.RefreshToken, tokenResult.Scope, tokenResult.TokenType);
        }



        public async Task<string> GetUserEmailAsync(string accessToken, CancellationToken ct = default)
        {
            var client = _httpClientFactory.CreateClient();
            using var request = new HttpRequestMessage(HttpMethod.Get, "https://www.googleapis.com/oauth2/v2/userinfo");
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);

            var response = await client.SendAsync(request, ct);
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync(ct);
            using var doc = System.Text.Json.JsonDocument.Parse(json);

            return doc.RootElement.GetProperty("email").GetString() ?? throw new InvalidOperationException("Emailul nu a putut fi extras din profilul Google.");
        }
    }



}


public sealed class GoogleTokenPayload
{
    [JsonPropertyName("access_token")]
    public string AccessToken { get; set; } = string.Empty;

    [JsonPropertyName("expires_in")]
    public int ExpiresIn { get; set; }

    [JsonPropertyName("refresh_token")]
    public string? RefreshToken { get; set; }

    [JsonPropertyName("scope")]
    public string Scope { get; set; } = string.Empty;

    [JsonPropertyName("token_type")]
    public string TokenType { get; set; } = string.Empty;
}
