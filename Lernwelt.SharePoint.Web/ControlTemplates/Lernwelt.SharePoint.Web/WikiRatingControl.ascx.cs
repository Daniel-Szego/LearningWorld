using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Lernwelt.SharePoint.Web.Webparts.WikiRatingWebpart;
using Lernwelt.SharePoint.Business;
using Lernwelt.SharePoint.Web.Utilities;
using System.Collections.Generic;
using Lernwelt.SharePoint.Business.Utilities;

namespace Lernwelt.SharePoint.Web.ControlTemplates.Lernwelt.SharePoint.Web
{
    public partial class WikiRatingControl : UserControl
    {
        public WikiRatingWebpart WebPart { get; set; }

        protected void Page_Load(object sender, EventArgs e)
        {
            if(!IsPostBack)
                LoadDataToControl();

        }

        protected void LoadDataToControl()
        {
            //string result = string.Empty;
            //if (WebPart.wikiRatingWebpartSettings != null)
            //{
            //    using (var repository = new SharePointRepository())
            //    {
            //        //repository.
            //    }
            //}
            //pnlResult.Controls.Add(new LiteralControl(result));
        }

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);

            if (WebPart.wikiRatingWebpartSettings != null)
            {
                using (var repository = new SharePointRepository())
                {
                    OpenStyle openStyle = WebPart.wikiRatingWebpartSettings.OpenLink;
                    Guid ListID = WebPart.wikiRatingWebpartSettings.ListId;
                    Guid LinkFieldID = WebPart.wikiRatingWebpartSettings.LinkField.Id;
                    Guid KeywordFieldID = WebPart.wikiRatingWebpartSettings.KeywordsField.Id;
                    List<WikiRatingWrapper> queryRes = repository.GetWikiRatings(ListID, LinkFieldID, new KeyValuePair<Guid, string>(KeywordFieldID, WebPart.wikiRatingWebpartSettings.Keywords), WebPart.wikiRatingWebpartSettings.EntryLimit);
                    string generatedHTML = HTMLHelper.genetrateHTML(queryRes, openStyle);
                    pnlResult.Controls.Add(new LiteralControl(generatedHTML));
                }
            }
        }


    }
}
