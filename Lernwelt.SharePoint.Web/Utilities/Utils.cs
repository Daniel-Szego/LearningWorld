using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lernwelt.SharePoint.Web.Utilities
{
    class Utils
    {
        public static string removeHash(string i)
        {
            if (i.IndexOf('#') > -1)
                return i.Substring(i.IndexOf('#') + 1 , i.Length - (i.IndexOf('#') + 1));
            else
                return i;
        }

        public static string removeHashLeft(string i)
        {
            if (i.IndexOf('#') > -1)
                return i.Substring(0, i.IndexOf(';'));
            else
                return i;
        }



        public static bool EqWithoutHash(string a, string b)
        {
            if (Utils.removeHash(a).ToLower().Equals(Utils.removeHash(b).ToLower()))
                return true;
            else
                return false;
        }

    }
}
