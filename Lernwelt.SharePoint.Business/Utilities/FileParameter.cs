using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lernwelt.SharePoint.Business.Utilities
{
    /// <summary>
    /// Help class to summarize the file parameters
    /// </summary>
    public class FileParameter
    {
        public Guid DocumentLibraryId {get; set;}

        public int FileID {get; set;}

        public Dictionary<FieldSettingsBL, object> parameters { get; set; }

        public int rating { get; set;}

        public string knowledgeTaxString {get; set;}

        public string keywordTaxString {get; set;}

    }
}
