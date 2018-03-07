using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.SharePoint.Administration;

namespace Lernwelt.SharePoint.Business
{
    /// <summary>
    /// Class for logging errors to the Sharepoint side appeared on the Business Logic
    /// </summary>
    public class Logger
    {
        public static void LogError(Exception ex)
        {

            LogError(ex.ToString() + (ex.InnerException == null ? string.Empty : ex.InnerException.ToString()));
        }

        public static void LogError(string error)
        {
            SPDiagnosticsService diagSvc = SPDiagnosticsService.Local;
            diagSvc.WriteEvent(0, new SPDiagnosticsCategory("Lernwelt Error", TraceSeverity.Unexpected, EventSeverity.Error), EventSeverity.Error, error, null);

        }

        public static void LogTrace(string message)
        {
            SPDiagnosticsService diagSvc = SPDiagnosticsService.Local;
            diagSvc.WriteEvent(0, new SPDiagnosticsCategory("Lernwelt Message", TraceSeverity.Monitorable, EventSeverity.Information), EventSeverity.Information, message, null);            
        }

    }
}
