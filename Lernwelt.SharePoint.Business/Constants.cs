using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lernwelt.SharePoint.Business
{
    public static class Constants
    {
        public static class Lists 
        {
            public static class MtLanguage
            {
                public static string ListName = "MTLanguage";

                public static string EnglishValue = "English";
            }

            public static class MtDocumentType
            {
                public static string ListName = "MTDocumenttype";
            }                                   

            public static class MtTargetGroup
            {
                public static string ListName = "MTTargetgroup";                                                
            }                                    
        }

        public static class Resource
        {
            public static string ResourceFile = "Lernwelt.SharePoint";
        }

        public static class General
        { 
            public static string AspxExtention = ".aspx";
        }

        public static class NonSupportedPageLayout
        {
            public static List<string> layouts = new List<string> { "versuch", "summary links", "redirect", "splash" };
        }
    }
}
