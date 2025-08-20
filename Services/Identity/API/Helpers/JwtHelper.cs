using Common.Infrastructure.Helpers;
using IdentityServer4.Models;
using System;
using System.Security.Claims;

namespace Identity.API.Helpers
{
    public class JwtHelper
    {
        private static string Secret = AuthorityKeys.GetAuthorityKeys().jwtkey.Sha256();
        public static string GenerateToken(Claim[] claims, int timeout)
        {
            return JwtWrapper.GenerateToken(Secret, claims, timeout);
        }
        public static ClaimsPrincipal GetClaimsPrincipal(string token)
        {
            try
            {
                return JwtWrapper.GetClaimsPrincipal(Secret, token);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"GetClaimsPrincipal Exception : {ex.Message}, \n StackTrace : {ex.StackTrace}");
                return null;
            }
        }
    }
}
