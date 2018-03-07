using System;
using System.ComponentModel;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;
using Lernwelt.SharePoint.Web.Utilities;
using Lernwelt.SharePoint.Web.ControlTemplates.Lernwelt.SharePoint.Web;
using System.Collections.Generic;

namespace Lernwelt.SharePoint.Web.Webparts.WikiRatingWebpart
{
    [ToolboxItemAttribute(false)]
    public class WikiRatingWebpart : WebPart
    {
        private const string _ascxPath = @"~/_CONTROLTEMPLATES/Lernwelt.SharePoint.Web/WikiRatingControl.ascx";

        [Personalizable(true)]
        public WikiRatingWebpartSettings wikiRatingWebpartSettings 
        {
            get;
            set;
        }

        protected override void CreateChildControls()
        {
            WikiRatingControl wikiControlControl = Page.LoadControl(_ascxPath) as WikiRatingControl;
            if (wikiControlControl != null)
            {
                wikiControlControl.WebPart = this;

            }
            Controls.Add(wikiControlControl);
        }

        public void SaveChanges()
        {
            this.SetPersonalizationDirty();
        }

        public override EditorPartCollection CreateEditorParts()
        {
            var editorPart = new WikiRatingEditorPart { ID = this.ID + "_WikiRatingEditorPart" };

            // Create a collection of Editor Parts and add them to the Editor Part collection.
            var editors = new List<EditorPart> { editorPart };
            return new EditorPartCollection(editors);
        }

    
    }
}
