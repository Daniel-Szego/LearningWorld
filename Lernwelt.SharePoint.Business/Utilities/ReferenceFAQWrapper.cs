using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.SharePoint;

namespace Lernwelt.SharePoint.Business.Utilities
{
   public class ReferenceFAQWrapper
    {
        public Guid? fileID { get; set;}

        public string fileName { get; set;}

        public int? intId {get; set;}

        public Guid columnID {get; set;}

        public string columnName {get; set;}

        public ReferenceFAQWrapper(string _fileName, Guid _columnID, string _columnName, int _intId)
        {
            //fileID = _fileID;
            fileName = _fileName;
            columnID = _columnID;
            columnName = _columnName;
            intId = _intId;
        }

        public ReferenceFAQWrapper(string _fileID, string _fileName, Guid _columnID, string _columnName)
        {
            if ((_fileID == null) || (_fileID.Equals(string.Empty)))
            {
                intId = null;
            }
            //fileID = new Guid(_fileID);
            fileName = _fileName;
            columnID = _columnID;
            columnName = _columnName;
            intId = int.Parse(_fileID);
        }

        public SPFieldLookupValue Lookup
        {
            get {
                if (intId.HasValue)
                    return new SPFieldLookupValue(intId.Value, fileName);
                else
                    return null;
            }
        }

    }
}
