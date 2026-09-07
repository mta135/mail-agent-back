using Google.Apis.Auth.OAuth2;
using Google.Apis.Auth.OAuth2.Flows;

namespace MailAgent.API.Services
{
    public interface IGoogleAthorization
    {

        string GetAuthorizationUrl();

        Task<UserCredential> ExchangeCodeForToken(string code);

        Task<UserCredential> ValidateToken(string accessToken);

    }




    public class GoogleAuthorizationService : IGoogleAthorization
    {
        private readonly IConfiguration _configuration;
        private readonly IGoogleAuthHelper _googleAuthHelper;


        public GoogleAuthorizationService(IConfiguration configuration, IGoogleAuthHelper googleAuthHelper)
        {    
            _configuration = configuration;
            _googleAuthHelper = googleAuthHelper;

        }



        public async Task<UserCredential> ExchangeCodeForToken(string code)
        {
            var flow = new GoogleAuthorizationCodeFlow(new GoogleAuthorizationCodeFlow.Initializer
            {
                ClientSecrets = _googleAuthHelper.GetClientSecrets(),
                Scopes = _googleAuthHelper.GetScopes()
            });

            var token = await flow.ExchangeCodeForTokenAsync(userId: "user", code: code, redirectUri: _configuration["Google:RedirectUri"], CancellationToken.None);

            return new UserCredential(flow, "user", token);
        }



        public string GetAuthorizationUrl()
        {
            var clientSecrets = _googleAuthHelper.GetClientSecrets();
            var scopes = _googleAuthHelper.GetScopes();
            
            var flow = new GoogleAuthorizationCodeFlow(new GoogleAuthorizationCodeFlow.Initializer
            {
                ClientSecrets = clientSecrets,
                Scopes = scopes
            });

            var redirectUri = _configuration["Google:RedirectUri"];

            var authorizationUrl = flow.CreateAuthorizationCodeRequest(redirectUri).Build().AbsoluteUri;
            return authorizationUrl;
        }




        //public string GetAuthorizationUrl()
        //{



        //    var clientSecrets = _googleAuthHelper.GetClientSecrets();

        //    //var authUrl = new GoogleAuthorizationCodeFlow(new GoogleAuthorizationCodeFlow.Initializer
        //    //{
        //    //    ClientSecrets = new ClientSecrets
        //    //    {
        //    //        ClientSecret = _googleAuthHelper.GetClientSecrets().ClientSecret,
        //    //        Scopes = _googleAuthHelper.GetScopes().Sc,

        //    //        //ClientId = _configuration["Google:ClientId"],
        //    //        //ClientSecret = _configuration["Google:ClientSecret"]
        //    //    },

        //    //    Scopes = new[] { "openid", "email", "profile" }


        //    //}).CreateAuthorizationCodeRequest(RedirectUri).Build().AbsoluteUri;




        //    throw new NotImplementedException();
        //}

        public Task<UserCredential> ValidateToken(string accessToken)
        {
            throw new NotImplementedException();
        }
    }
}
