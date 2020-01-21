using System;
using System.IO;
using Serilog;

namespace DatabaseTool
{
    public static class LogHelper
    {
        static LogHelper()
        {
            Log.Logger = new LoggerConfiguration()
                .WriteTo.RollingFile(Path.Combine(Environment.CurrentDirectory, @"Logs\log-{Date}.txt"))
                .CreateLogger();
        }
       
        public static void Info(string info)
        {
          Log.Logger.Information(info);
        }

        public static void Fatal(string info)
        {
           Log.Logger.Fatal(info);
        }

        public static void Error(Exception exception)
        {
            Log.Logger.Error(exception,exception.Message);
        }

        public static void Fatal(Exception ex)
        {
            Log.Logger.Fatal(ex,ex.Message);
        }

        public static void Fatal(string info, Exception ex)
        {
            Log.Logger.Fatal(info);
        }
    }
}
