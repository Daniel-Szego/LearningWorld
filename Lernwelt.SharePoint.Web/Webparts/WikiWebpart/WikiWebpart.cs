using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Lernwelt.SharePoint.Web.Controls;
using Lernwelt.SharePoint.Web.Utilities;
using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;

namespace Lernwelt.SharePoint.Web.Webparts.WikiWebpart
{
    [ToolboxItemAttribute(false)]
    public class WikiWebpart : WebPart
    {
        private const string _ascxPath = @"~/_CONTROLTEMPLATES/Lernwelt.SharePoint.Web/WikiControl.ascx";

        [Personalizable(true)]
        public WikiWebpartSettings WikiWebpartSettings
        {
            get;
            set;
        }

        protected override void CreateChildControls()
        {
            WikiControl wikiControlControl = Page.LoadControl(_ascxPath) as WikiControl;
            if (wikiControlControl != null)
            {
                wikiControlControl.WebPart = this;

                // query string parameter
                if (this.Page.Request.QueryString["LernweltWikiID"] != null)
                {
                    wikiControlControl.IsNew = false;
                    wikiControlControl.wikiID = int.Parse(this.Page.Request.QueryString["LernweltWikiID"].ToString());
                }
                else
                {
                    wikiControlControl.IsNew = true;
                    wikiControlControl.wikiID = -1;
                }


            }
            Controls.Add(wikiControlControl);
        }

        public void SaveChanges()
        {
            this.SetPersonalizationDirty();
        }

        public override EditorPartCollection CreateEditorParts()
        {
            var editorPart = new WikiListSelectorEditorPart { ID = this.ID + "_WikiListSelectorEditorPart" };

            // Create a collection of Editor Parts and add them to the Editor Part collection.
            var editors = new List<EditorPart> { editorPart };
            return new EditorPartCollection(editors);
        }
    }
}
