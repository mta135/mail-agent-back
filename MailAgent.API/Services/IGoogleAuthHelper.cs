using Google.Apis.Auth.OAuth2;
using Google.Apis.Oauth2.v2;

namespace MailAgent.API.Services
{
    public interface IGoogleAuthHelper
    {
        string[] GetScopes();
        string ScopeToString();

        ClientSecrets GetClientSecrets();
    }





    public class GoogleAuthHelper : IGoogleAuthHelper
    {
        private readonly IConfiguration _configuration;
        public GoogleAuthHelper(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public string[] GetScopes()
        {
            var scopes = new[]
            { 
                Oauth2Service.Scope.Openid,
                Oauth2Service.Scope.UserinfoEmail,
                 
                Oauth2Service.Scope.UserinfoProfile,
            };

            return scopes;

        }

        public string ScopeToString()
        {
            return string.Join(" ", GetScopes());
        }

        public ClientSecrets GetClientSecrets()
        {
            var clientId = _configuration["Google:ClientId"];
            var clientSecret = _configuration["Google:ClientSecret"];

      
            return new ClientSecrets
            {
                ClientId = clientId,
                ClientSecret = clientSecret
            };
        }
    }

}
