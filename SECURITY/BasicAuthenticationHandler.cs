using HYDSWMAPI.SERVICES;
using COMMON.GENERIC;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace HYDSWMAPI.SECURITY
{
    public class BasicAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {

        public BasicAuthenticationHandler(
            IOptionsMonitor<AuthenticationSchemeOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder,
            ISystemClock clock)
            : base(options, logger, encoder, clock)
        {
        }

        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            if (!Request.Headers.ContainsKey("Authorization"))
                return AuthenticateResult.Fail("Missing Authorization Header");

            BUserInfo user = null;
            bool IsAuthorized = false;
            try
            {
                var authHeader = AuthenticationHeaderValue.Parse(Request.Headers["Authorization"]);
                var credentialBytes = Convert.FromBase64String(authHeader.Parameter);
                var credentials = Encoding.UTF8.GetString(credentialBytes).Split(new[] { ':' }, 2);
                var username = credentials[0];
                var password = credentials[1];
                user = new BUserInfo();
                user.UserName = username;
                IsAuthorized =await IsAuthorizedUser(username, password);
            }
            catch
            {
                return AuthenticateResult.Fail("Invalid Authorization Header");
            }

            if (IsAuthorized == false)
                return AuthenticateResult.Fail("Invalid Username or Password");

            var claims = new[] {
                new Claim(ClaimTypes.NameIdentifier, user.UserName.ToString()),
                new Claim(ClaimTypes.Name, user.UserName),
            };
            var identity = new ClaimsIdentity(claims, Scheme.Name);
            var principal = new ClaimsPrincipal(identity);
            var ticket = new AuthenticationTicket(principal, Scheme.Name);

             return  AuthenticateResult.Success(ticket);
         // return await Task.FromResult(AuthenticateResult.Success(ticket));
        }
        public  async Task<bool>  IsAuthorizedUser(string Username, string Password)
        {
            List<BUserInfo> _lst = new List<BUserInfo>();

            _lst.Add(new BUserInfo { UserName = "patna", Password = "patna#2020" });
            _lst.Add(new BUserInfo { UserName = "ramky_hiswm_hyd", Password = "ramky_hiswm_hyd#2021_$#^&*9846@!" });
            // In this method we can handle our database logic here...
            bool IsAuthorize= await Task.Run(() => _lst.Any(i => i.UserName == Username && i.Password == Password));
            return IsAuthorize;
        }
    }

}
