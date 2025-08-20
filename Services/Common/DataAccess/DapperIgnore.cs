using System;

namespace Common.DataAccess
{
    [AttributeUsage(AttributeTargets.Property)]
    public class DapperIgnore : Attribute
    {
    }

    [AttributeUsage(AttributeTargets.Property)]
    public class DapperKey : Attribute
    {
    }
    [AttributeUsage(AttributeTargets.Property)]
    public class DapperSecured : Attribute
    {
        public int Length { get; set; }
        public System.Data.DbType DbType { get; set; }
    }
}
