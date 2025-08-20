using Common.DataAccess.Interfaces;
using Dapper;
using NambayaUser.Repository.Models;
using System;
using System.Collections.Generic;
using System.Data;

namespace NambayaUser.Repository.Interfaces
{
    public interface IUserRepository : IDapperRepositoryBase<User>
    {
        IEnumerable<Models.User> GetUsers();
        Models.User GetByEmail(string email);
        new IEnumerable<User> GetItems(CommandType commandType, string sql, object parameters = null);
        int Execute(CommandType commandType, string sql, object parameters = null);
        IEnumerable<User> ExecuteStoredProcedure(String SpName, DynamicParameters objectParams);
        long GetCount();
    }
}