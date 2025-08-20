using Common.Infrastructure;
using Identity.Contracts.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text.Json;

namespace Identity.API.Authorities
{
    public class AccountAuthority : IAuthority
    {
        private readonly IServiceProvider _serviceProvider;
        public AccountAuthority(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public string[] Payload => new string[] { "username", "password", "role" };

        public Claim[] OnForward(Claim[] claims)
        {
            throw new NotImplementedException();
        }

        public Claim[] OnVerify(Claim[] claims, JsonElement payload, string identifier, out bool valid)
        {
            IIdentityService identityService = _serviceProvider.GetRequiredService<IIdentityService>();
            valid = false;
            var roles =  payload.GetProperty("roles").ToString().Split(',');
            var user = identityService.IsValidUser(payload.GetProperty("username").ToString(), payload.GetProperty("password").ToString(),roles.ToList());
            if (user == null || roles.Any(p=> p.Trim().ToLower() == user.RoleCode.ToLower())==false)
                throw new Common.Infrastructure.Exceptions.ServiceException(ClientSideErrors.INVALID_USER_ID_PASSWORD);
            identityService.GetUser(user);
            valid = true;
            return new Claim[]
            {
              new Claim(identifier, user.UserID.ToString()),
              new Claim("name", user.FullName),
              new Claim("email", user.LoginName),
              new Claim("phone", user.Phone??"")
            };
        }
    }
}
