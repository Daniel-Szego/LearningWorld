using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.SharePoint.Utilities;

namespace Lernwelt.SharePoint.Web.Utilities
{
    /// <summary>
    /// Localisation helper
    /// </summary>
    public static class LocHelper
    {
        private const string RESOURCES = "$Resources:";
        private const string RESOURCESFILENAME = "Lernwelt.SharePoint";
                                                  

        public static string GetLocalizedString(string source)
        {

            if (string.IsNullOrEmpty(source))
            {
                return source;
            }

            string value = source;
            try
            {
                string resources = RESOURCES + source;
                int lcid = System.Threading.Thread.CurrentThread.CurrentUICulture.LCID;
                value = SPUtility.GetLocalizedString(resources, RESOURCESFILENAME, (uint)lcid);

                if (!string.IsNullOrEmpty(value))
                {
                    if (value.StartsWith(resources))
                    {
                        value = SPUtility.GetLocalizedString(resources, RESOURCESFILENAME, (uint)1033);
                    }
                }
                else
                {
                    return source;
                }
            }
            catch (Exception ex)
            {
                // log resource file errors

            }
            return value;
        }
    }
}
