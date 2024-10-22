using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ResuRead.Engine
{
    public static class Strings
    {
        public static string CONFIGFILENAME = "AgentPrompts.json";

        public static string LOGGINGELEMENT = "Logging";
        public static string LOGGING_FILEPATH = "FilePath";
        public static string LOGGING_RETENTIONDAYS = "RollingIntervalDays";
        public static string LOGGING_LEVEL = "LogLevel";

        public static string AGENTCONFIGELEMENT = "Agent";
        public static string AGENTCONFIG_LIBRARYFILENAME = "LibraryFileName";
        public static string AGENTCONFIG_CLASSNAME = "ProviderClassName";
        public static string AGENTCONFIG_PARAMETERS = "Parameters";

        public static string DESTINATIONCONFIGELEMENT = "Destination";
        public static string DESTINATIONCONFIG_LIBRARYFILENAME = "LibraryFileName";
        public static string DESTINATIONCONFIG_CLASSNAME = "ProviderClassName";
    }
}
