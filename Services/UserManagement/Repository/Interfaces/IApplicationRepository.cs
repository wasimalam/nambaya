using System.Data;
using Common.DataAccess.Interfaces;
using UserManagement.Repository.Models;

namespace UserManagement.Repository.Interfaces
{
    public interface IApplicationRepository : IDapperRepositoryBase<Application>
    {
        long GetID(string applicationCode, IDbTransaction trans = null);
    }
}