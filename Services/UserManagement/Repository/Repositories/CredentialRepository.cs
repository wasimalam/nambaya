using Common.DataAccess;
using Common.DataAccess.Interfaces;
using Common.Infrastructure.Extensions;
using Dapper;
using System;
using System.Data;
using System.Linq;
using UserManagement.Repository.Interfaces;
using UserManagement.Repository.Models;

namespace UserManagement.Repository.Repositories
{
    public class CredentialRepository : DapperRepositoryBase<Credential>, ICredentialRepository
    {
        private readonly ICredentialHistoryRepository _credentialHistoryRepository;

        public CredentialRepository(IDatabaseSession session, ICredentialHistoryRepository credentialHistoryRepository) : base(session)
        {
            _credentialHistoryRepository = credentialHistoryRepository;
        }

        public void InsertPassword(Credential cred)
        {
            var propertyContainer = ParseProperties(cred, true);
            var sql = string.Format("INSERT INTO {0} ({1}) VALUES(@{2})",
                TableName,
                string.Join(", ", propertyContainer.ValueNames),
                string.Join(", @", propertyContainer.ValueNames));

            DatabaseSession.Session.Query(sql, propertyContainer.ValuePairs, commandType: CommandType.Text);
            cred.DataState = DBState.None;
        }

        public bool IsValid(long userId, string password)
        {
            var credential = GetByID(userId);
            if (credential.Password == password.Base64())
                return true;
            return false;
        }
        public new Credential GetByID(long userId)
        {
            var sql = $"SELECT *  FROM {TableName} Where UserID = @Id";
            var f = DatabaseSession.Session.Query<Credential>(sql, new { Id = userId }).FirstOrDefault();
            if (f != null)
                f.DataState = DBState.None;
            return f;
        }

        public new void Update(Credential obj)
        {
            obj.UpdatedOn = DateTime.UtcNow;
            var propertyContainer = ParseProperties(obj, false);
            var sqlValuePairs = GetSqlPairs(propertyContainer.ValueNames);
            var sql = string.Format("UPDATE [{0}] SET {1} WHERE UserID=@UserId", TableName, sqlValuePairs);
            Execute(CommandType.Text, sql, propertyContainer.AllPairs);
        }
    }
}