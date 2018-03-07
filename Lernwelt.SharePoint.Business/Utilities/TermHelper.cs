using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lernwelt.SharePoint.Business.Utilities
{
    /// <summary>
    /// Helper class to store name and ID for a Term
    /// </summary>
    public class TermHelper
    {
        public string Name { get; set;}

        public Guid ID { get; set;}
        
        public TermHelper(string _name, Guid _id)
        {
            Name = _name;
            ID = _id;
        }

        public TermHelper(string _name, string _id)
        {
            Name = _name;
            ID = new Guid(_id);
        }



        public string getString()
        {
            return Name + "|" + ID.ToString();
        }

        public string getCAMLFragment(Guid ColumnID)
        {
            if (Name == null)
                return string.Empty;
            if (Name.Equals(string.Empty))
                return string.Empty;

            string ret = string.Format("<Eq><FieldRef ID='{0}'/><Value Type='Text'>{1}</Value></Eq>", ColumnID, Name);
            return ret;
        }

        #region Static_services

        public static string getString(List<TermHelper> input)
        {
            string result = string.Empty;
            for (int i = 0; i < input.Count; i++)
            {
                if (i < input.Count -1)
                    result += input.ElementAt(i).getString() + ";";
                else
                    result += input.ElementAt(i).getString();
            }
            return result;
        }

        public static string getNameString(string input)
        {
            if (!input.Contains("|"))
                return string.Empty;

            return input.Substring(0, input.IndexOf("|"));
        }

        public static List<TermHelper> getTermHelperList(string input)
        {
            List<TermHelper> result = new List<TermHelper>();

            if (!input.Equals(string.Empty))
            {
                foreach (string item in input.Split(';'))
                {
                    string name = item.Substring(0, item.IndexOf("|"));
                    string id = item.Substring(item.IndexOf("|") + 1, item.Length - (item.IndexOf("|") + 1));
                    result.Add(new TermHelper(name, id));
                }
            }
            return result;
        }

        public static bool Contains(Guid key, List<TermHelper> data)
        {
            if (data.Select(x => x.ID.Equals(key)).Count() > 0)
                return true;
            else return false;
        }

        public static bool Contains(string key, List<TermHelper> data)
        {
            bool ret = false;
            Guid keyGuid = new Guid(key);
            foreach(TermHelper elem in data)
            {
                if (elem.ID == keyGuid)
                    ret = true;
            }
            return ret;
        }

        public static string GetCAMLString(List<TermHelper> input, Guid columnID)
        {
            string ret = string.Empty;
            Stack<TermHelper> stack = new Stack<TermHelper>();
            foreach (TermHelper term in input)
            {
                stack.Push(term);
            }

            if(stack.Count == 0)
                return string.Empty;
            else if (stack.Count == 1)
            {
                TermHelper node = stack.Pop();
                ret = node.getCAMLFragment(columnID);
            }
            else
            {
                foreach (TermHelper node in input)
                {
                    if (ret.Equals(string.Empty))
                        ret = node.getCAMLFragment(columnID);
                    else
                        ret = string.Format("<And>{0}{1}</And>", node.getCAMLFragment(columnID), ret);
                }
            }
            return ret;
        }

        public static string GetCAMLString(KeyValuePair<Guid, string> input)
        {
            return TermHelper.GetCAMLString(TermHelper.getTermHelperList(input.Value), input.Key);
        }

        #endregion

    }
}
