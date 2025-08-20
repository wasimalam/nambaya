using Common.DataAccess.Interfaces;
using Common.DataAccess.Models;
using Common.Infrastructure;
using Dapper;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;

namespace Common.DataAccess
{
    public class DapperRepositoryBase<T> : IDapperRepositoryBase<T> where T : BaseModel
    {
        private readonly string _tableName;

        #region Constructor
        public DapperRepositoryBase(IDatabaseSession databaseSession)
        {
            DatabaseSession = databaseSession;
            _tableName = typeof(T).Name;
        }

        #endregion

        public IEnumerable<R> GetCustomItems<R>(CommandType commandType, string sql, object parameters = null)
        {
            var conn = DatabaseSession.Session; ;
            return conn.Query<R>(sql, parameters, commandType: commandType);
        }

        #region Standard Dapper functionality
        public IEnumerable<T> GetItems(CommandType commandType, string sql, object parameters = null)
        {
            var conn = DatabaseSession.Session; ;
            return conn.Query<T>(sql, parameters, commandType: commandType);
        }

        public R ExecuteScalar<R>(CommandType commandType, string sql, object parameters = null)
        {
            var connection = DatabaseSession.Session;
            return connection.ExecuteScalar<R>(sql, parameters, commandType: commandType);
        }

        public int Execute(CommandType commandType, string sql, object parameters = null)
        {
            var connection = DatabaseSession.Session;
            return connection.Execute(sql, parameters, commandType: commandType);
        }

        protected string TableName
        {
            get
            {
                var validKeywordNames = new[] { "User", "Role" }; // Some keywords 
                if (validKeywordNames.Contains(_tableName))
                    return $"[{_tableName}]";
                return _tableName;
            }
        }

        public IDatabaseSession DatabaseSession { get; }

        #endregion

        #region Automated methods for: Insert, Update, Delete
        public T GetByID(long id)
        {
            var props = GetFilteredProperties(typeof(T));
            var propName = string.Join(",", props.Select(o => "[" + o.Name + "]"));// props.Aggregate<string>((i,j)=> '['+i.Name+"], "+'['+j.Name+"] ");
            var sql = $"SELECT {propName}  FROM {TableName} Where ID = @Id";
            var param = new DynamicParameters();
            param.Add("@Id", id);
            var f = DatabaseSession.Session.Query<T>(sql, param).FirstOrDefault();
            if (f != null)
                f.DataState = DBState.None;
            return f;
        }

        public IEnumerable<T> SelectAll()
        {
            var sql = $"SELECT *  FROM {TableName}";
            var f = DatabaseSession.Session.Query<T>(sql);
            f.ToList().ForEach(p => p.DataState = DBState.None);
            return f;
        }
        public PagedResults<T> GetAllPages(int limit, int offset, string orderBy = null, IFilter parameters = null)
        {
            var whereClause = GenerateWhereClause(parameters);
            var limitClause = limit != 0 ? $"Limit @Limit Offset @Offset" : "";
            var results = new PagedResults<T>();
            DynamicParameters par = new DynamicParameters();
            var sql = $"SELECT *  FROM [{_tableName} {GenerateOrderByClause(orderBy)} {limitClause}; SELECT COUNT(*)  FROM {_tableName}";
            var connection = DatabaseSession.Session;
            if (limit != 0)
            {
                par.Add("Limit", limit);
                par.Add("Offset", offset);
            }
            if (connection is SqlConnection)
            {
                limitClause = limit != 0 ? $" OFFSET  @Offset ROWS FETCH NEXT @Limit ROWS ONLY " : "";
                sql = $"SELECT *  FROM {TableName} {whereClause} {GenerateOrderByClause(orderBy)} {limitClause}; SELECT COUNT(*) FROM {TableName} {whereClause}";
            }
            if (parameters != null)
            {
                foreach (var p in parameters.Params)
                    par.Add(p.Key, p.Value);
            }
            var f = connection.QueryMultiple(sql, par);
            results.Data = f.Read<T>().ToList();
            results.TotalCount = f.Read<int>().FirstOrDefault();
            results.Data.ToList().ForEach(p => p.DataState = DBState.None);
            results.PageSize = limit == 0 ? results.TotalCount : limit;
            return results;
        }

        public void Insert(T obj)
        {
            var propertyContainer = ParseProperties(obj, true);
            var sql = string.Format("INSERT INTO {0} ({1}) VALUES(@{2}) SELECT CAST(scope_identity() AS int)",
                TableName,
                string.Join(", ", propertyContainer.ValueNames),
                string.Join(", @", propertyContainer.ValueNames));

            var id = DatabaseSession.Session.Query<int>(sql, propertyContainer.ValuePairs, commandType: CommandType.Text).First();
            SetId(obj, id, propertyContainer.IdPairs);
            obj.DataState = DBState.None;
        }

        public void Insert(T obj, IPropertyContainer propertyContainer)
        {
            var sql = string.Format("INSERT INTO {0} ({1}) VALUES(@{2}) SELECT CAST(scope_identity() AS int)",
                TableName,
                string.Join(", ", propertyContainer.ValueNames),
                string.Join(", @", propertyContainer.ValueNames));

            var id = DatabaseSession.Session.Query<int>(sql, propertyContainer.ValuePairs, commandType: CommandType.Text).First();
            SetId(obj, id, propertyContainer.IdPairs);
            obj.DataState = DBState.None;
        }

        public void Update(T obj)
        {
            var propertyContainer = ParseProperties(obj, false);
            var sqlIdPairs = GetSqlPairs(propertyContainer.IdNames);
            var sqlValuePairs = GetSqlPairs(propertyContainer.ValueNames);
            var sql = string.Format("UPDATE {0} SET {1} WHERE {2}", TableName, sqlValuePairs, sqlIdPairs);
            Execute(CommandType.Text, sql, propertyContainer.AllPairs);
        }

        public void Update(T obj, IPropertyContainer propertyContainer)
        {
            var sqlIdPairs = GetSqlPairs(propertyContainer.IdNames);
            var sqlValuePairs = GetSqlPairs(propertyContainer.ValueNames);
            var sql = string.Format("UPDATE {0} SET {1} WHERE {2}", TableName, sqlValuePairs, sqlIdPairs);
            Execute(CommandType.Text, sql, propertyContainer.AllPairs);
        }

        public void Delete(T obj)
        {
            var propertyContainer = ParseProperties(obj, false);
            var sqlIdPairs = GetSqlPairs(propertyContainer.IdNames);
            var sql = string.Format("DELETE FROM [{0}] WHERE {1} ", TableName, sqlIdPairs);
            Execute(CommandType.Text, sql, propertyContainer.IdPairs);
        }

        public IEnumerable<T> ExecuteStoredProcedure(String SpName, DynamicParameters objectParams)
        {
            return GetItems(CommandType.StoredProcedure, SpName, objectParams);
        }
        #endregion
        protected string GenerateWhereClause(IFilter parameters = null)
        {
            return (parameters != null && parameters.Params.Any()) ? "Where " + parameters.FilterString : "";
        }
        protected string GenerateOrderByClause(string orderBy)
        {
            var keyProperties = GetKeyProperties(typeof(T));
            var keyColumn = keyProperties.FirstOrDefault()?.Name;
            keyColumn = orderBy ?? keyColumn ?? "ID";
            if (!keyColumn.ToLower().Contains(" asc") && !keyColumn.ToLower().Contains(" desc"))
                keyColumn = $"{keyColumn} DESC ";
            return $" ORDER BY { keyColumn} ";
        }
        #region Reflection support

        public static PropertyInfo[] GetFilteredProperties(Type type)
        {
            return type.GetProperties().Where(pi => pi.GetCustomAttributes(typeof(DapperIgnore), true).Length == 0).ToArray();
        }
        public static PropertyInfo[] GetSecuredProperties(Type type)
        {
            return type.GetProperties().Where(pi => pi.GetCustomAttributes(typeof(DapperSecured), true).Length != 0).ToArray();
        }
        /// <summary>
        /// Filter properties by providing type and filter attribute as parameters
        /// </summary>
        /// <param name="type"></param>
        /// <param name="attribute">Attribute on the base of which properties will be filtered</param>
        /// <returns>Array of properties</returns>
        public static PropertyInfo[] GetFilteredProperties(Type type, Type attribute)
        {
            return type.GetProperties().Where(pi => pi.GetCustomAttributes(attribute, true).Length == 0).ToArray();
        }

        public static PropertyInfo[] GetKeyProperties(Type type)
        {
            return type.GetProperties().Where(pi => pi.GetCustomAttributes(typeof(DapperIgnore), true).Length == 0
            && pi.GetCustomAttributes(typeof(DapperKey), false).Length == 0).ToArray();
        }

        /// <summary>
        /// Retrieves a Dictionary with name and value 
        /// for all object properties matching the given criteria.
        /// </summary>
        protected static IPropertyContainer ParseProperties(T obj, bool isInsert)
        {
            var propertyContainer = new PropertyContainer();

            var typeName = typeof(T).Name;
            var validKeyNames = new[] { "ID",
            string.Format("ID", "{0}ID", typeof(T).Name), string.Format("{0}Id", typeof(T).Name), string.Format("{0}id", typeof(T).Name) };

            var properties = GetFilteredProperties(typeof(T)); // typeof(T).GetProperties().Where(p => !p.IsDefined(typeof(DapperIgnore), false));

            foreach (var property in properties)
            {
                // Skip reference types (but still include string!)
                if (property.PropertyType.IsClass && property.PropertyType != typeof(string))
                    continue;

                // Skip methods without a public setter
                if (property.GetSetMethod() == null)
                    continue;

                // Skip methods specifically ignored
                if (property.IsDefined(typeof(DapperIgnore), false))
                    continue;
                var secureAttr = property.GetCustomAttributes(typeof(DapperSecured), true).FirstOrDefault() as DapperSecured;
                var name = property.Name;
                var value = typeof(T).GetProperty(property.Name).GetValue(obj, null);
                if (property.IsDefined(typeof(DapperKey), false) || validKeyNames.Contains(name))
                {
                    propertyContainer.AddId(name, (secureAttr == null) ? value : new DbString() { Value = value?.ToString(), Length = secureAttr.Length });
                }
                else
                {
                    if ((isInsert && (name == "UpdatedBy" || name == "UpdatedOn") || (!isInsert && (name == "CreatedBy" || name == "CreatedOn"))))
                        continue;
                    else
                    {
                        value = SetDefaultValues(isInsert, name, value);
                        propertyContainer.AddValue(name, (secureAttr == null) ? value : new DbString() { Value = value?.ToString(), Length = secureAttr.Length });
                    }
                }
            }
            return propertyContainer;
        }

        private static object SetDefaultValues(bool isInsert, string name, object value)
        {
            if (isInsert && name == "CreatedOn" /*&& value == null*/)
                value = DateTime.UtcNow;
            else if (!isInsert && name == "UpdatedOn" /*&& value == null*/)
                value = DateTime.UtcNow;
            return value;
        }

        /// <summary>
        /// Create a commaseparated list of value pairs on 
        /// the form: "key1=@value1, key2=@value2, ..."
        /// </summary>
        protected static string GetSqlPairs
        (IEnumerable<string> keys, string separator = ", ")
        {
            var pairs = keys.Select(key => string.Format("{0}=@{0}", key)).ToList();
            return string.Join(separator, pairs);
        }

        public void SetId(T obj, int id, IDictionary<string, object> propertyPairs)
        {
            if (propertyPairs.Count == 1)
            {
                var propertyName = propertyPairs.Keys.First();
                var propertyInfo = obj.GetType().GetProperty(propertyName);
                if (propertyInfo.PropertyType == typeof(int) || propertyInfo.PropertyType == typeof(Int64))
                {
                    propertyInfo.SetValue(obj, id, null);
                }
            }
        }

        public IEnumerable<T> ExecuteTheStoredProcedure(string SpName, DynamicParameters objectParams)
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}