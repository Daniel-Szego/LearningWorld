using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lernwelt.SharePoint.Web.Utilities
{
    [Serializable]
    public class WikiRatingWebpartSettings
    {
        public Guid ListId { get; set; }

        public FieldSettings LinkField { get; set; }

        public FieldSettings KeywordsField { get; set; }

        public string Keywords { get; set; }

        public OpenStyle OpenLink { get; set;}

        public string LinkTitle { get; set;}

        public int EntryLimit { get; set;}
    }

}
