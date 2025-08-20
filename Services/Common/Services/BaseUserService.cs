using Common.BusinessObjects;
using Common.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;

namespace Common.Services
{
    public class BaseUserService : BaseService
    {
        private bool _adjustNameColumn;
        public BaseUserService(IServiceProvider serviceProvider, bool bNameColumnAdjust = true) : base(serviceProvider)
        {
            _adjustNameColumn = bNameColumnAdjust;
        }
        virtual protected IFilter GetUsersDefaultFilter(string param, ref bool? isActiveFilter, ref bool? isLockedFilter)
        {
            IFilter filter = null;
            if (!string.IsNullOrWhiteSpace(param))
            {
                var expressions = Expression.StringToExpressions(param);
                if (_adjustNameColumn && expressions.Where(p => p.Property.ToLower() == "name").Any())
                    expressions.Where(p => p.Property.ToLower() == "name").FirstOrDefault().Property = "FirstName +' '+LastName";
                var filterlist = expressions.Where(p => p.Property.ToLower() != "isactive" && p.Property.ToLower() != "islocked")
                    .Select(p => new Filter(p.Property, p.Operation, p.Value)).ToList();
                filter = new ANDFilter(filterlist.Select(p => p as IFilter).ToList());
                if (expressions.Any(p => p.Property.ToLower() == "isactive"))
                    isActiveFilter = expressions.FirstOrDefault(p => p.Property.ToLower() == "isactive").Value.ToString().ToLower() == "true";
                if (expressions.Any(p => p.Property.ToLower() == "islocked"))
                    isLockedFilter = expressions.FirstOrDefault(p => p.Property.ToLower() == "islocked").Value.ToString().ToLower() == "true";
            }
            return filter;
        }
        virtual protected string getAdjustedOrderBy(string qOrderby, bool? isActiveFilter, bool? isLockedFilter)
        {
            if (string.IsNullOrWhiteSpace(qOrderby))
                qOrderby = "";
            var orderby = qOrderby;
            if (string.IsNullOrWhiteSpace(orderby) || orderby.ToLower().Contains("isactive") ||
                orderby.ToLower().Contains("islocked"))
                orderby = "CreatedOn desc";
            if (_adjustNameColumn && string.IsNullOrWhiteSpace(orderby) == false && orderby.ToLower().StartsWith("name"))
                orderby = orderby.Replace("name", "FirstName+' '+LastName");
            return orderby;
        }
        virtual protected int getAdjustedQueryLimit(string qOrderby, int qLimit, bool? isActiveFilter, bool? isLockedFilter)
        {
            if ((qOrderby != null && qOrderby.ToLower().Contains("isactive")) ||
                (qOrderby != null && qOrderby.ToLower().Contains("islocked")) ||
                (isActiveFilter != null) || (isLockedFilter != null))
                return 0;
            return qLimit;
        }
        protected void GetUMData<T>(string userManagementServiceBaseUrl, string roleCodes,
            PagedResults<T> pg, string qOrderby, int qLimit, int limit, int offset,
            bool? isActiveFilter, bool? isLockedFilter) where T : BaseUserBO
        {
            if (pg.TotalCount == 0)
                return;
            using (WebApiClient apiClient = new WebApiClient(_serviceProvider, userManagementServiceBaseUrl))
            {
                string res = string.Empty;
                List<BaseUserBO> resUsers = new List<BaseUserBO>();
                int dbTotalCount = pg.TotalCount;
                if (dbTotalCount <= qLimit || limit != 0)
                {
                    res = apiClient.InternalServiceGetAsync("api/v1/user/getbyloginnames/",
                       JsonSerializer.Serialize(pg.Data.Select(p => p.LoginName).ToList())).Result;
                    resUsers = JsonSerializer.Deserialize<List<BaseUserBO>>(res,
                    new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
                    resUsers = resUsers.Where(p => (limit != 0 || ((isActiveFilter == null || p.IsActive == isActiveFilter) && (isLockedFilter == null || p.IsLocked == isLockedFilter)))).ToList();
                    resUsers = resUsers.Where(p => roleCodes.Split(",").Contains(p.Role)).ToList();
                }
                else
                {
                    foreach (var roleCode in roleCodes.Split(","))
                    {
                        res = apiClient.InternalServiceGetAsync($"api/v1/user/getbyrole?role={roleCode}&isactive={(isActiveFilter != null ? isActiveFilter.ToString() : "")}&isLocked={(isLockedFilter != null ? isLockedFilter.ToString() : "")}").Result;
                        resUsers.AddRange(JsonSerializer.Deserialize<List<BaseUserBO>>(res,
                            new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase }));
                    }
                }
                pg.Data = pg.Data.Where(p => resUsers.Any(u => u.LoginName == p.LoginName));
                pg.Data.ToList().ForEach(p =>
                {
                    var user = resUsers.FirstOrDefault(u => u.LoginName == p.LoginName);
                    if (user != null)
                    {
                        p.LoginName = user.LoginName;
                        p.IsActive = user.IsActive;
                        p.IsLocked = user.IsLocked;
                        p.IsPasswordResetRequired = user.IsPasswordResetRequired;
                        p.Role = user.Role;
                    }
                });
                if (string.IsNullOrWhiteSpace(qOrderby) == false && qOrderby.ToLower().Contains("isactive"))
                {
                    if (qOrderby.ToLower().Contains(" asc"))
                        pg.Data = pg.Data.OrderBy(p => p.IsActive ? 0 : 1);
                    else
                        pg.Data = pg.Data.OrderByDescending(p => p.IsActive ? 0 : 1);
                }
                else if (string.IsNullOrWhiteSpace(qOrderby) == false && qOrderby.ToLower().Contains("islocked"))
                {
                    if (qOrderby.ToLower().Contains(" asc"))
                        pg.Data = pg.Data.OrderBy(p => p.IsLocked ? 0 : 1);
                    else
                        pg.Data = pg.Data.OrderByDescending(p => p.IsLocked ? 0 : 1);
                }
                if (limit == 0)
                {
                    pg.TotalCount = pg.Data.Count();
                    limit = Math.Min(qLimit==0 ? pg.TotalCount: qLimit, pg.TotalCount);
                    pg.PageSize = limit;
                    pg.Data = pg.Data.Skip(offset).Take(limit);
                }
            }
        }
    }
}
