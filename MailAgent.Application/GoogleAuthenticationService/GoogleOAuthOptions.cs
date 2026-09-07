namespace MailAgent.API.Services
{
    public class GoogleOAuthOptions
    {
        public const string SectionName = "GoogleOAuth";

        public string ClientId { get; set; } = string.Empty;
        public string ClientSecret { get; set; } = string.Empty;
        public string RedirectUri { get; set; } = string.Empty;
        public string Scope { get; set; } = string.Empty;
    }
}
