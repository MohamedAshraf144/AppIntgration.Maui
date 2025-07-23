using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppIntgration.Shard.Constants
{
    public static class ApiConstants
    {
        public static class ServiceStatuses
        {
            public const string Active = "Active";
            public const string Inactive = "Inactive";
            public const string Pending = "Pending";
            public const string Maintenance = "Maintenance";
            public const string Error = "Error";
        }
        public static class LogLevels
        {
            public const string Info = "Info";
            public const string Warning = "Warning";
            public const string Error = "Error";
            public const string Debug = "Debug";
            public const string Success = "Success";
        }
        public static class Endpoints
        {
            public const string GetLogs = "/api/logs";
            public const string GetServices = "/api/services";
            public const string GetAuth = "/api/auth";
            public const string GetApiKey = "/api/auth/apikey";
        }
    }
}
