using IdentityServer4.Models;
using System;
using System.Security.Claims;

namespace Identity.API.Authorities
{
    public class AuthenticationAuthority : IAuthenticator
    {
        private static string authSecret = AuthorityKeys.GetAuthorityKeys().authenticationkey.Sha256();

        public Claim[] GetAuthenticationClaims(string identifier)
        {
            if (!long.TryParse(identifier, out long guid))
                throw new FormatException();
            var hash = string.Format("{0}:{1}", identifier, authSecret).Sha256();
            return new Claim[]
            {
            new Claim("auth_key", identifier),
            new Claim("auth_hash", hash)
            };
        }
    }
}
