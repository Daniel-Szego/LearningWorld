using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.SharePoint.Administration;

namespace Lernwelt.SharePoint.Business.Utilities
{
    public class LoggingService : SPDiagnosticsServiceBase
    {
        private const string AreaName = "Lernwelt";
        private const string CategoryName = "Webparts";

        private static LoggingService _instance;

        public static LoggingService Instance
        {
            get { return _instance ?? (_instance = new LoggingService()); }
        }

        private LoggingService()
            : base("MBLernwelt", SPFarm.Local)
        {
            
        }

        protected override IEnumerable<SPDiagnosticsArea> ProvideAreas()
        {
            yield return new SPDiagnosticsArea(AreaName, ProvideCategories());
        }

        private IEnumerable<SPDiagnosticsCategory> ProvideCategories()
        {
            yield return new SPDiagnosticsCategory(CategoryName, TraceSeverity.Medium, EventSeverity.Warning);
        }

        public void LogError(Exception exception)
        {
            var category = Instance.Areas[AreaName].Categories[CategoryName];
            Instance.WriteTrace(0, category, TraceSeverity.High, exception.ToString(), null);
        }

        public void LogInformation(string text, params object[] data)
        {
            var category = Instance.Areas[AreaName].Categories[CategoryName];
            Instance.WriteTrace(0, category, TraceSeverity.Monitorable, text, data);
        }
    }
}
