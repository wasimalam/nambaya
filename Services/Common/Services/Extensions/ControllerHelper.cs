using Common.BusinessObjects;
using Common.Infrastructure;
using Microsoft.AspNetCore.Http;
using System.Linq;
using System.Text.Json;
using System.Threading;

namespace Common.Services.Extensions
{
    public static class ControllerHelper
    {
        public static SessionContext GetSessionContext(this HttpContext context)
        {
            long userid = 0, appUserID = 0, pharmacyID = 0, cardiologistID;
            var claims = context?.User?.Claims;// FindFirst("scope");
            if (claims != null && claims.Where(p => p.Type == "sub").Any())
            {
                long.TryParse(claims.Where(p => p.Type == "sub").FirstOrDefault().Value, out userid);
                long.TryParse(claims.Where(p => p.Type == ClaimNames.AppUserID).FirstOrDefault().Value, out appUserID);
                long.TryParse(claims.Where(p => p.Type == ClaimNames.PharmacyID).FirstOrDefault().Value, out pharmacyID);
                long.TryParse(claims.Where(p => p.Type == ClaimNames.CardiologistID).FirstOrDefault().Value, out cardiologistID);
                return new SessionContext
                {
                    ApplicationCode = claims.Where(p => p.Type == ClaimNames.ApplicationCode).FirstOrDefault()?.Value,
                    RoleCode = claims.Where(p => p.Type == ClaimNames.RoleCode).FirstOrDefault()?.Value,
                    UserID = userid,
                    AppUserID = appUserID,
                    LoginName = claims.Where(p => p.Type == ClaimNames.Email).FirstOrDefault().Value,
                    FirstName = claims.Where(p => p.Type == ClaimNames.FirstName).FirstOrDefault().Value,
                    LastName = claims.Where(p => p.Type == ClaimNames.LastName).FirstOrDefault().Value,
                    PharmacyID = pharmacyID,
                    CardiologistID = cardiologistID
                };
            }
            else
            {
                if (context != null && context.Request != null && context.Request.Headers["NambayaSession"].Count > 0)
                {
                    var res = context.Request.Headers["NambayaSession"];
                    if (string.IsNullOrWhiteSpace(res) == false)
                        return JsonSerializer.Deserialize<SessionContext>(res);
                }
                else if(Thread.GetNamedDataSlot("NambayaSession") != null)
                {
                    return (Common.BusinessObjects.SessionContext)Thread.GetData(Thread.GetNamedDataSlot("NambayaSession"));
                }
            }
            return null;
        }
    }
}
