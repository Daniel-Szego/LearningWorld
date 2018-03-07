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

namespace Lernwelt.SharePoint.Web.Webparts.FaqWebpart
{
    [ToolboxItemAttribute(false)]
    public class FaqWebpart : WebPart
    {
        private const string _ascxPath = @"~/_CONTROLTEMPLATES/Lernwelt.SharePoint.Web/FaqControl.ascx";

        [Personalizable(true)]
        public FaqWebpartSettings FaqWebpartSettings
        {
            get;
            set;
        }

        protected override void CreateChildControls()
        {
            base.CreateChildControls();

            FaqControl faqControlControl = Page.LoadControl(_ascxPath) as FaqControl;
            if (faqControlControl != null)
            {
                faqControlControl.WebPart = this;

                // query string parameter
                if (this.Page.Request.QueryString["LernweltFaqID"] != null)
                {
                    faqControlControl.IsNew = false;
                    faqControlControl.faqID = int.Parse(this.Page.Request.QueryString["LernweltFaqID"].ToString());
                }
                else
                {
                    faqControlControl.IsNew = true;
                    faqControlControl.faqID = -1;
                }

            }
            Controls.Add(faqControlControl);
        }

        public void SaveChanges()
        {
            this.SetPersonalizationDirty();
        }

        public override EditorPartCollection CreateEditorParts()
        {
            var editorPart = new FaqListSelectorEditorPart { ID = this.ID + "_FaqListSelectorEditorPart" };

            // Create a collection of Editor Parts and add them to the Editor Part collection.
            var editors = new List<EditorPart> { editorPart };
            return new EditorPartCollection(editors);
        }
    }
}
