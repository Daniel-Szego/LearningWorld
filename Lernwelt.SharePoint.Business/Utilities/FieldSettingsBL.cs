using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lernwelt.SharePoint.Business.Utilities
{
    [Serializable]
    public class FieldSettingsBL
    {
            public FieldSettingsBL()
            { }

            public FieldSettingsBL(Guid _id, string _name)
            {
                Id = _id;
                Name = _name;
            }

            public Guid Id { get; set; }

            public string Name { get; set; }

    }
}
