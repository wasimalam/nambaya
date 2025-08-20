using Common.Infrastructure;
using Identity.Contracts.Interfaces;
using IdentityServer4.Models;
using IdentityServer4.Services;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Identity.API
{

    public class ProfileService : IProfileService
    {
        private IIdentityService _identityService;
        public ProfileService(IIdentityService identityService)
        {
            _identityService = identityService;
        }
        public Task GetProfileDataAsync(ProfileDataRequestContext context)
        {
            try
            {
                context.IssuedClaims.Add(new Claim(ClaimNames.ApplicationCode, context.Subject.Claims.FirstOrDefault(p => p.Type == ClaimNames.ApplicationCode)?.Value));
                context.IssuedClaims.Add(new Claim(ClaimNames.RoleCode, context.Subject.Claims.FirstOrDefault(p => p.Type == ClaimNames.RoleCode)?.Value));
                context.IssuedClaims.Add(new Claim(ClaimNames.AppUserID, context.Subject.Claims.FirstOrDefault(p => p.Type == ClaimNames.AppUserID)?.Value));
                context.IssuedClaims.Add(new Claim(ClaimNames.FirstName, context.Subject.Claims.FirstOrDefault(p => p.Type == ClaimNames.FirstName)?.Value));
                context.IssuedClaims.Add(new Claim(ClaimNames.LastName, context.Subject.Claims.FirstOrDefault(p => p.Type == ClaimNames.LastName)?.Value));
                context.IssuedClaims.Add(new Claim(ClaimNames.Email, context.Subject.Claims.FirstOrDefault(p => p.Type == ClaimNames.Email)?.Value));
                context.IssuedClaims.Add(new Claim(ClaimNames.Name, context.Subject.Claims.FirstOrDefault(p => p.Type == ClaimNames.Name)?.Value));
                context.IssuedClaims.Add(new Claim(ClaimNames.Language, context.Subject.Claims.FirstOrDefault(p => p.Type == ClaimNames.Language)?.Value));
                context.IssuedClaims.Add(new Claim(ClaimNames.PharmacyID, context.Subject.Claims.FirstOrDefault(p => p.Type == ClaimNames.PharmacyID)?.Value));
                context.IssuedClaims.Add(new Claim(ClaimNames.CardiologistID, context.Subject.Claims.FirstOrDefault(p => p.Type == ClaimNames.CardiologistID)?.Value));
            }
            catch//(Exception exception)
            {
                //ignored
            }
            return Task.FromResult(0);
        }
        public Task IsActiveAsync(IsActiveContext context)
        {
            context.IsActive = true;

            return Task.FromResult(0);
        }
    }
}
