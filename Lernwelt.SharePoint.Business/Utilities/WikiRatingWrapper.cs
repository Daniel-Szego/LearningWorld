using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lernwelt.SharePoint.Business.Utilities
{
    /// <summary>
    /// Rating wrapper for wiki
    /// </summary>
    public class WikiRatingWrapper
    {
        public WikiRatingWrapper(Guid _id, string _name, int _intId, int _rating, string _link)
        {
            Id = _id;
            Name = _name;
            intId = _intId;
            Rating = _rating;
            Link = _link;
        }

        public Guid Id { get; set; }

        public string Name { get; set; }

        public int intId { get; set; }

        public int Rating { get; set;}

        public string Link { get; set;}
    }
}
