using UserManagement.Contracts.Models;

namespace UserManagement.Contracts.Interfaces
{
    public interface ICredentialService
    {
        CredentialBO UpdateUserPassword(CredentialBO cred);
    }
}
