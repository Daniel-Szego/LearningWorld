using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lernwelt.SharePoint.Business.Utilities
{
    public class PageLayoutWrapper
    {
        public PageLayoutWrapper(string _name, string _title)
        {
            Name = _name;
            Title = _title;
        }

        public string Name { get; set;}

        public string Title { get; set;}

    }
}
