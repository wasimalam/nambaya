using Common.Infrastructure;
using Dapper;
using System;
using System.Collections.Generic;
using System.Data;

namespace Common.DataAccess.Interfaces
{
    public interface IDapperRepositoryBase
    {

    }

    public interface IDapperRepositoryBase<T> : IDapperRepositoryBase where T : class
    {
        public IEnumerable<T> GetItems(CommandType commandType, string sql, object parameters = null);
        T GetByID(long Id);
        IEnumerable<T> SelectAll();
        PagedResults<T> GetAllPages(int limit, int offset, string orderby = null, IFilter parameters = null);
        void Insert(T obj);
        void Insert(T obj, IPropertyContainer propertyContainer);
        void Update(T obj);
        void Update(T obj, IPropertyContainer propertyContainer);
        void Delete(T obj);
        IEnumerable<T> ExecuteTheStoredProcedure(String SpName, DynamicParameters objectParams);
        R ExecuteScalar<R>(CommandType commandType, string sql, object parameters = null);
        IEnumerable<R> GetCustomItems<R>(CommandType commandType, string sql, object parameters = null);
    }
}
