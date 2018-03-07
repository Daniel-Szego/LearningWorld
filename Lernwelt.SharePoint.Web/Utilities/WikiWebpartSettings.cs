using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lernwelt.SharePoint.Web.Utilities
{
    [Serializable]
    public class WikiWebpartSettings
    {
        public Guid ListId { get; set; }

        public FieldSettings Title { get; set; }

        public FieldSettings Description { get; set; }

        public FieldSettings AspxName { get; set; }

        public FieldSettings PageLayout { get; set; }

        public FieldSettings Language { get; set; }

        public FieldSettings Keywords { get; set; }

        public FieldSettings KnowledgeArea { get; set; }

        public FieldSettings Author { get; set; }

        public FieldSettings TargetGroup { get; set; }

        public FieldSettings ReferenceId { get; set; }

        public FieldSettings PageLink { get; set; }
    }
}
