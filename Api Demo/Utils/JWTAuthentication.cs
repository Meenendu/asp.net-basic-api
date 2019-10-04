using ApiDemo.Controllers;
using ApiDemo.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Filters;

namespace ApiDemo.Auth
{
    public class JwtAuthentication : Attribute, IAuthenticationFilter
    {
        public string Realm { get; set; }
        public bool AllowMultiple => false;

        public async Task AuthenticateAsync(HttpAuthenticationContext context, CancellationToken cancellationToken)
        {
            var request = context.Request;
            var authorization = request.Headers.Authorization;
            // checking request header value having required scheme "Bearer" or not.
            if (authorization == null || authorization.Scheme != "Bearer" || string.IsNullOrEmpty(authorization.Parameter))
            {
                //check if atleast header has "username" passed
                context.ErrorResult = new AuthFailureResult("JWT Token is Missing", request);

                //if (request.Headers.Any(x => x.Key == "username"))
                //{
                //    var username = request.Headers.Where(x => x.Key == "username").Select(x => x.Value.FirstOrDefault()).FirstOrDefault();
                //    var claims = new List<Claim>
                //    {
                //        new Claim(ClaimTypes.Name, username)
                //    };
                //    var identity = new ClaimsIdentity(claims, "Jwt");
                //    IPrincipal user = new ClaimsPrincipal(identity);
                //    context.Principal = await Task.FromResult(user);
                //}
                return;
            }
            // Getting Token value from header values.
            var token = authorization.Parameter;
            var principal = await AuthJwtToken(token);

            if (principal == null)
            {
                context.ErrorResult = new AuthFailureResult("Invalid JWT Token", request);
            }
            else
            {
                context.Principal = principal;
            }
        }
        private static bool ValidateToken(string token, out string username)
        {
            username = null;

            var simplePrinciple = JwtAuthManager.GetPrincipal(token);
            if (simplePrinciple == null)
                return false;
            var identity = simplePrinciple.Identity as ClaimsIdentity;

            if (identity == null)
                return false;

            if (!identity.IsAuthenticated)
                return false;

            var usernameClaim = identity.FindFirst(ClaimTypes.Name);
            username = usernameClaim?.Value;

            if (string.IsNullOrEmpty(username))
                return false;

            var theUsername = username;
            using (ApiDbContext ctx = new ApiDbContext())
            {
                if (ctx.UserTokens.Any(x => x.UserName == theUsername && x.Token == token &&
                ((x.TokenType == "LIMITED" && x.ExpireTime > DateTime.UtcNow) || x.TokenType == "UNLIMITED"))) return true;
            }
            return false;
        }
        protected Task<IPrincipal> AuthJwtToken(string token)
        {
            string username;

            if (ValidateToken(token, out username))
            {
                //to get more information from DB in order to build local identity based on username 
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, username)
                    // you can add more claims if needed like Roles ( Admin or normal user or something else)
                };

                var identity = new ClaimsIdentity(claims, "Jwt");
                IPrincipal user = new ClaimsPrincipal(identity);

                return Task.FromResult(user);
            }

            return Task.FromResult<IPrincipal>(null);
        }
        public Task ChallengeAsync(HttpAuthenticationChallengeContext context, CancellationToken cancellationToken)
        {
            Challenge(context);
            return Task.FromResult(0);
        }
        private void Challenge(HttpAuthenticationChallengeContext context)
        {
            string parameter = null;

            if (!string.IsNullOrEmpty(Realm))
                parameter = "realm=\"" + Realm + "\"";

            context.ChallengeWith("Bearer", parameter);
        }
    }
}