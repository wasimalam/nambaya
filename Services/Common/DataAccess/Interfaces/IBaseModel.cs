using System;

namespace Common.DataAccess.Interfaces
{
    public interface IBaseModel
    {
        [DapperKey]
        long ID { get; set; }

        DateTime? CreatedOn { get; set; }
        DateTime? UpdatedOn { get; set; }
        string CreatedBy { get; set; }
        string UpdatedBy { get; set; }

        T Clone<T>(T obj);
    }
}
