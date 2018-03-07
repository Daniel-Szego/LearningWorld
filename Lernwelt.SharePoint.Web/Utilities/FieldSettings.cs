using System;
using Microsoft.SharePoint;

namespace Lernwelt.SharePoint.Web.Utilities
{
    [Serializable]
    public class FieldSettings
    {

        public Guid Id { get; set; }

        public string Name { get; set; }

        public SPFieldType Type { get; set; }
    }
}