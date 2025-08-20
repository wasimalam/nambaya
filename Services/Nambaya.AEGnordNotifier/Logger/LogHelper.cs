using System;
using Serilog;


namespace Nambaya.AEGnordNotifier.Logger
{
    public static class LogHelper
    {
        public static Serilog.Core.Logger Log;
        public static void ConfigureSeriLog()
        {
             Log = new LoggerConfiguration()
                .MinimumLevel.Information()
                .WriteTo.File(AppDomain.CurrentDomain.BaseDirectory + "\\logs\\log_.log", rollingInterval: RollingInterval.Day)
                .CreateLogger();
        }
    }
}
