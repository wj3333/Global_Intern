﻿using Global_Intern.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.CodeAnalysis.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Org.BouncyCastle.Asn1.Ocsp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Security.Principal;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace Global_Intern.Util
{
    public class BasicAuthOptions : AuthenticationSchemeOptions
    {

    }

    public class CustomAuthHandler : AuthenticationHandler<BasicAuthOptions>
    {

        private string _logedInUserToken;
        private readonly ICustomAuthManager cAuthManger;
        private  string _tokenSession;
        public CustomAuthHandler(
            IOptionsMonitor<BasicAuthOptions> options, ILoggerFactory logger, UrlEncoder encode, ISystemClock clock,
            ICustomAuthManager customAuthManager,
            IHttpContextAccessor httpContextAccessor
            ) : base (options, logger, encode, clock)
        {
            
            cAuthManger = customAuthManager;
            
        }

        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            //if (!Request.Headers.ContainsKey("Authorization"))
            //    return AuthenticateResult.Fail("UnAuthorized");

            //string authHandler = Request.Headers["Authorization"];
            //if(string.IsNullOrEmpty(authHandler))
            //    return AuthenticateResult.Fail("UnAuthorize");

            //if(authHandler.StartsWith("bearer", StringComparison.OrdinalIgnoreCase))
            //    return AuthenticateResult.Fail("UnAuthorize");

            //string token = authHandler.Substring("bearer".Length).Trim();
            //if(string.IsNullOrEmpty(token))
            //    return AuthenticateResult.Fail("UnAuthorize");
            try
            {
                return validateToken();
            }
            catch (Exception ex)
            {

                return AuthenticateResult.Fail(ex.Message);
            }
        }


        private AuthenticateResult validateToken() {
            var validatedToken = cAuthManger.Tokens.FirstOrDefault();
            if(validatedToken.Key == null)
            {
                return AuthenticateResult.Fail("UnAuthorize");
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, validatedToken.Value.Item1),
                new Claim(ClaimTypes.Role, validatedToken.Value.Item2)
            };

            var identity = new ClaimsIdentity(claims, Scheme.Name);

            var principle = new GenericPrincipal(identity, new[] { validatedToken.Value.Item2 });
            var ticket = new AuthenticationTicket(principle, Scheme.Name);

            return AuthenticateResult.Success(ticket);
        }
    }
}