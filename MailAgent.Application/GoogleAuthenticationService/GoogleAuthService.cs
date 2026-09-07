using MailAgent.Application.GoogleAuthenticationService;
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
            Dictionary<string, string> parameters = new Dictionary<string, string>
            {
                ["client_id"] = _options.ClientId,
                ["redirect_uri"] = _options.RedirectUri,
                ["response_type"] = "code",
                ["scope"] = _options.Scope,
                ["access_type"] = "offline",
                ["prompt"] = "consent",
                ["state"] = userId.ToString()
            };

            string queryString = string.Join("&", parameters.Select(p => $"{p.Key}={Uri.EscapeDataString(p.Value)}"));
            return $"https://accounts.google.com/o/oauth2/v2/auth?{queryString}";
        }

        public async Task<GoogleTokenResponse> ExchangeCodeForTokensAsync(string code)
        {
            var client = _httpClientFactory.CreateClient();

            Dictionary<string, string> body = new()
            {
                ["code"] = code,
                ["client_id"] = _options.ClientId,
                ["client_secret"] = _options.ClientSecret,
                ["redirect_uri"] = _options.RedirectUri,
                ["grant_type"] = "authorization_code"
            };

            HttpResponseMessage response = await client.PostAsync("https://oauth2.googleapis.com/token", new FormUrlEncodedContent(body));

            response.EnsureSuccessStatusCode();

            string json = await response.Content.ReadAsStringAsync();
            GoogleTokenPayload? tokenResult = JsonSerializer.Deserialize<GoogleTokenPayload>(json);

            if (tokenResult is null || string.IsNullOrEmpty(tokenResult.AccessToken))
            {
                throw new InvalidOperationException("Răspunsul de la Google nu conține un token valid.");
            }

            return new GoogleTokenResponse(tokenResult.AccessToken, tokenResult.ExpiresIn, tokenResult.RefreshToken, tokenResult.Scope, tokenResult.TokenType);
        }



        public async Task<string> GetUserEmailAsync(string accessToken)
        {
            HttpClient? client = _httpClientFactory.CreateClient();
            using HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, "https://www.googleapis.com/oauth2/v2/userinfo");

            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);

            HttpResponseMessage response = await client.SendAsync(request);
            response.EnsureSuccessStatusCode();

            string json = await response.Content.ReadAsStringAsync();
            using JsonDocument doc = System.Text.Json.JsonDocument.Parse(json);

            return doc.RootElement.GetProperty("email").GetString() ?? throw new InvalidOperationException("Emailul nu a putut fi extras din profilul Google.");
        }
    }
}


