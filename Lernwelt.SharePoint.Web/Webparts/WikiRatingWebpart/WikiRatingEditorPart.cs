using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI.WebControls.WebParts;
using Lernwelt.SharePoint.Web.ControlTemplates.Lernwelt.SharePoint.Web;
using Lernwelt.SharePoint.Web.Utilities;
using Lernwelt.SharePoint.Business.Utilities;
using Microsoft.SharePoint.WebPartPages;

namespace Lernwelt.SharePoint.Web.Webparts.WikiRatingWebpart
{
    public sealed class WikiRatingEditorPart : EditorPart
    {

        private const string EditorControlPath = @"~/_CONTROLTEMPLATES/Lernwelt.SharePoint.Web/WikiRatingPropertyControl.ascx";

        private WikiRatingPropertyControl _control;
        private WikiRatingWebpart _faqWebpart;
        private WikiRatingWebpartSettings _faqWebpartSettings;

        public WikiRatingEditorPart()
        {
            Title = LocalizeHelper.GetLocalizedString("Control_Rating_Property_Title");
        }

        public override bool ApplyChanges()
        {
            _faqWebpart = this.WebPartToEdit as WikiRatingWebpart;
            if (_faqWebpart != null)
            {
                _faqWebpart.wikiRatingWebpartSettings = _control.GetDocumentWebpartSettings;
                _faqWebpart.SaveChanges();
            }
            return true;
        }

        public override void SyncChanges()
        {
            EnsureChildControls();

            _faqWebpart = this.WebPartToEdit as WikiRatingWebpart;
            if (_faqWebpart != null)
            {
                _faqWebpartSettings = _faqWebpart.wikiRatingWebpartSettings;
                if (_control != null && _faqWebpartSettings != null)
                {
                    _control.OriginalValues = _faqWebpartSettings;
                }
            }
        }

        protected override void CreateChildControls()
        {
            base.CreateChildControls();

            var pane = Zone as ToolPane;
            if (pane != null)
            {
                // Disable the validation on Cancel Button of ToolPane
                pane.Cancel.CausesValidation = false;
                pane.Cancel.Click += Cancel_Click;
            }

            _control = Page.LoadControl(EditorControlPath) as WikiRatingPropertyControl;
            if (_control != null)
            {
                _control.ID = "WikiRatingPropertyControl";
                Controls.Add(_control);
            }
        }

        void Cancel_Click(object sender, EventArgs e)
        {
            // On Cancel rollback all the changes by restoring the Original List
            if (_control.OriginalValues != null)
            {
                _faqWebpartSettings = _control.OriginalValues;
                ApplyChanges();
            }
        }
    }

}
