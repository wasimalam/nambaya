using Common.Infrastructure;
using Identity.Contracts.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text.Json;

namespace Identity.API.Authorities
{
    public class RPOTPAuthority : IAuthority
    {
        private readonly IServiceProvider _serviceProvider;

        public RPOTPAuthority(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public string[] Payload => new string[] { "loginname", "otptype", "applicationcode" };

        public Claim[] OnForward(Claim[] claims)
        {
            throw new NotImplementedException();
        }

        public Claim[] OnVerify(Claim[] claims, JsonElement payload, string identifier, out bool valid)
        {
            IIdentityService identityService = _serviceProvider.GetRequiredService<IIdentityService>();
            valid = false;
            var user = identityService.GetUMUser(payload.GetProperty("loginname").ToString());
            if (user == null || payload.GetProperty("applicationcode").ToString().ToLower() != user.ApplicationCode.ToLower())
                throw new KeyNotFoundException();
            if (user.IsActive == false)
                throw new Common.Infrastructure.Exceptions.ServiceException(ClientSideErrors.USER_ID_INACTIVE);
            else if (user.IsLocked)
                throw new Common.Infrastructure.Exceptions.ServiceException(ClientSideErrors.USER_ID_LOCKED);
            else if (user.IsDeleted)
                throw new Common.Infrastructure.Exceptions.ServiceException(ClientSideErrors.USER_ID_DELETED);
            identityService.GetUser(user);
            valid = true;
            return new Claim[]
            {
              new Claim(identifier, user.UserID.ToString()),
              new Claim("email", user.LoginName),
              new Claim("phone", user.Phone??""),
              new Claim("name", user.FullName??""),
              new Claim("otptype", payload.GetProperty("otptype").ToString()),
              new Claim("otppurpose", user.IsPasswordResetRequired?"firstreset":"resetpassword")
            };
        }
    }
}
