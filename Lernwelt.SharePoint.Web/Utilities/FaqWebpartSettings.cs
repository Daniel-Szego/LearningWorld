using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lernwelt.SharePoint.Web.Utilities
{
    [Serializable]
    public class FaqWebpartSettings
    {
        public Guid ListId { get; set; }

        public FieldSettings Question { get; set; }

        public FieldSettings Answer { get; set; }

        public FieldSettings Language { get; set; }

        public FieldSettings Keywords { get; set; }

        public FieldSettings KnowledgeArea { get; set; }

        public FieldSettings Author { get; set; }

        public FieldSettings TargetGroup { get; set; }

        public FieldSettings ReferenceId { get; set; }

     //   public FieldSettings NoOfRatings { get; set;}
    }
}
