using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lernwelt.SharePoint.Business.Utilities
{
    public class WikiParameter
    {
        public Guid WikiId { get; set; }

        public int FileID { get; set; }

        public Dictionary<FieldSettingsBL, object> parameters { get; set; }

        public int rating { get; set; }

        public string knowledgeTaxString { get; set; }

        public string keywordTaxString { get; set; }

        public string layoutName { get; set;}
    }
}
