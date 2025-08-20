using System;
using System.Collections.Generic;
using System.Text;

namespace Common.Interfaces.Interfaces
{
    public interface ICronConfig<T>
    {
        string CronExpression { get; set; }
        TimeZoneInfo TimeZoneInfo { get; set; }
    }
}
