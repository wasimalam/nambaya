using System;
using System.Collections.Generic;
using System.Text;
using Common.Interfaces.Interfaces;

namespace Common.Services
{
    public class CronConfig<T> : ICronConfig<T>
    {
        public string CronExpression { get; set; }
        public TimeZoneInfo TimeZoneInfo { get; set; }
    }
}
