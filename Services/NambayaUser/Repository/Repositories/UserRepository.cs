using Common.DataAccess;
using Common.DataAccess.Interfaces;
using Common.Infrastructure;
using NambayaUser.Repository.Interfaces;
using System.Collections.Generic;
using System.Linq;

namespace NambayaUser.Repository.Repositories
{
    public class UserRepository : DapperRepositoryBase<Repository.Models.User>, IUserRepository
    {
        private ApiRequestConfiguration _apiSecretConf;
        private WebServiceConfiguration _webApiConf;
        private IDatabaseSession _databaseSession;

        public UserRepository(IDatabaseSession databaseSession, WebServiceConfiguration webApiConf, ApiRequestConfiguration apiSecretConf) : base(databaseSession)
        {
            _apiSecretConf = apiSecretConf;
            _webApiConf = webApiConf;
            _databaseSession = databaseSession;
        }

        public IEnumerable<Models.User> GetUsers()
        {
            string sql = $"Select * from [User]";
            return GetItems(System.Data.CommandType.Text, sql);
        }

        public Models.User GetByEmail(string email)
        {
            string sql = $"Select * from {TableName} where email=@email";
            return GetItems(System.Data.CommandType.Text, sql, new { email = email }).FirstOrDefault();
        }
        public long GetCount()
        {
            return ExecuteScalar<long>(System.Data.CommandType.Text, $"select count(id) from {TableName}");
        }
    }
}