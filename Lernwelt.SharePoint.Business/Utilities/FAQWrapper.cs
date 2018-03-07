using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lernwelt.SharePoint.Business.Utilities
{
    public class FAQWrapper
    {

        public FAQWrapper()
        {

        }

        public FAQWrapper(Guid _id, string _name, int _intID)
        {
            Id = _id;
            Name = _name;
            intId = _intID;
        }

        public Guid Id {get; set;}

        public string Name { get; set; }

        public int intId { get; set;}

    }
}
