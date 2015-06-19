using Microsoft.Owin.Security.OAuth;
using System.Security.Claims;
using System.Threading.Tasks;
using WebApiTokenAuth.DataAccess;
using WebApiTokenAuth.Model;
using WebApiTokenAuth.Utility;

namespace WebApiTokenAuth.Providers
{
    public class SimpleAuthorizationServerProvider : OAuthAuthorizationServerProvider
    {

        public override async Task ValidateClientAuthentication(OAuthValidateClientAuthenticationContext context)
        {
            context.Validated();
        }

        public override async Task GrantResourceOwnerCredentials(OAuthGrantResourceOwnerCredentialsContext context)
        {

            context.OwinContext.Response.Headers.Add("Access-Control-Allow-Origin", new[] { "*" });

            DataAccessLayer _repo = new DataAccessLayer();
            
                AppResultModel user = _repo.FindUser(context.UserName, context.Password);

                if (user.ResultStatus == (int)AppResultStatus.UNAUTHORIZED)
                {
                    context.SetError("invalid_grant", "The mobile number or password is incorrect.");
                    return;
                }
            
            var identity = new ClaimsIdentity(context.Options.AuthenticationType);
            identity.AddClaim(new Claim("sub", context.UserName));
            identity.AddClaim(new Claim("role", "user"));

            context.Validated(identity);

        }
    }

}