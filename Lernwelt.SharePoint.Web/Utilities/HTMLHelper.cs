using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lernwelt.SharePoint.Business.Utilities;

namespace Lernwelt.SharePoint.Web.Utilities
{
    /// <summary>
    /// Static class for HTML generation
    /// </summary>
    public static class HTMLHelper
    {
        public static string genetrateHTML(List<WikiRatingWrapper> input, OpenStyle _openStyle)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("<table>");
            sb.Append("<tr>");
            sb.Append("<th>");
            string linkTitle = LocalizeHelper.GetLocalizedString("Link_Title");
            sb.Append(linkTitle);
            sb.Append("</th>");
            sb.Append("<th>");
            string ratingTitle = LocalizeHelper.GetLocalizedString("Rating_Title");
            sb.Append(ratingTitle);
            sb.Append("</th>");
            sb.Append("</tr>");

            foreach(WikiRatingWrapper elem in input)
            {
                sb.Append("<tr>");
                  sb.Append("<td>");
                    sb.Append(HTMLHelper.GenerateJavascriptForWindow(elem.Name, elem.Link, _openStyle));
                  sb.Append("</td>");
                  sb.Append("<td>");
                int rateNum = elem.Rating >= 0 ? elem.Rating : 0;
                        
                      for (int i = 1; i <= 5; i++)
                      {
                        if (i <= rateNum)
                            sb.Append("<img id=\"star\" tabIndex=\"0\" alt=\"star selected..\" src=\"/_layouts/Lernwelt.SharePoint.Web/RatingsNew.png\"> </img>");
                        else
                            sb.Append("<img id=\"star\" tabIndex=\"0\" alt=\"star selected..\" src=\"/_layouts/Lernwelt.SharePoint.Web/RatingsEmpty.png\"> </img>");
                      }
                sb.Append("<td>");
                sb.Append("</tr>");
            }

            sb.Append("</table>");
            return sb.ToString();
        }

        public static string GenerateJavascriptForWindow(string name, string link, OpenStyle openStyle) 
        {
            StringBuilder sb = new StringBuilder();

            if (openStyle == OpenStyle.SameWindow)
            { 
                sb.Append(string.Format("<a href=\"{0}\">{1}</a>", link, name));
            }
            else if (openStyle == OpenStyle.NewWindow)
            { 
                sb.Append(string.Format("<a href=\"{0}\" target=\"_blank\">{1}</a>", link, name));                
            }
            else if (openStyle == OpenStyle.ModalDialog)
            {
                sb.Append(string.Format("<a href=\"#\" onclick=\"ShowLernweltDialog('{0}'); return true;\">{1}</a>", link, name));                
            }
            else
            {
                sb.Append(name);
            }

            return sb.ToString();
        }

    }
}
