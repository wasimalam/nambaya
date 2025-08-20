using System.Collections.Generic;
using System.Threading.Tasks;

namespace Common.BusinessObjects.Interfaces
{
    public interface ICommonUserServce
    {
        List<UserSettingBO> GetSettings(string loginname);
        void UpdateSettings(string loginname, List<UserSettingBO> userSettings);
        Task ChangeCredentials(string loginname, ChangeCredentialBO userCredential);
    }
}
