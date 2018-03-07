using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Web;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Utilities;

namespace Lernwelt.SharePoint.Business.Utilities
{
    public static class LocalizeHelper
    {
        private static string GetLocalizedString(string resourceKey, int lcid)
        {
            if (string.IsNullOrEmpty(resourceKey))
                return string.Empty;
            var globalResourceObject = HttpContext.GetGlobalResourceObject(Constants.Resource.ResourceFile, resourceKey, CultureInfo.CurrentUICulture);
            return globalResourceObject != null ? globalResourceObject.ToString() : string.Empty;
        }

        public static string GetLocalizedString(string resourceKey)
        {
            return GetLocalizedString(resourceKey, CultureInfo.CurrentUICulture.LCID);
        }
    }
}
