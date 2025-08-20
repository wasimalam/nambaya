using System.Security.Claims;
using System.Text.Json;

namespace Identity.API.Authorities
{

    public interface IAuthority
    {
        string[] Payload { get; }
        Claim[] OnVerify(Claim[] claims, JsonElement payload, string identifier, out bool valid);
        Claim[] OnForward(Claim[] claims);
    }


    public interface IAuthenticator
    {
        Claim[] GetAuthenticationClaims(string identifier);
    }
}
