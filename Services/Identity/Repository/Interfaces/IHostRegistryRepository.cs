using System;
using System.Collections.Generic;
using System.Text;

namespace Identity.Repository.Interfaces
{
    public interface IHostRegistryRepository
    {
        string GetErrorUrl();
        string GetLogInUrl();
        string GetLogOutUrl();
        string GetIdentityServerUrl();
        string[] GetCorsUrls();
    }
}
