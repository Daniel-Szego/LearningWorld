using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Web.UI.WebControls.WebParts;
using Lernwelt.SharePoint.Business.Utilities;
using Lernwelt.SharePoint.Web.Controls;
using Lernwelt.SharePoint.Web.Utilities;
using Microsoft.SharePoint;
using Microsoft.SharePoint.WebPartPages;

namespace Lernwelt.SharePoint.Web.Webparts.WikiWebpart
{
    public sealed class WikiListSelectorEditorPart : EditorPart
    {
        private const string EditorControlPath = @"~/_CONTROLTEMPLATES/Lernwelt.SharePoint.Web/WikiPropertyControl.ascx";

        private WikiPropertyControl _control;
        private WikiWebpart _faqWebpart;
        private WikiWebpartSettings _faqWebpartSettings;

        public WikiListSelectorEditorPart()
        {
            Title = LocalizeHelper.GetLocalizedString("Control_WIKI_Property_Title");
        }

        public override bool ApplyChanges()
        {
            _faqWebpart = this.WebPartToEdit as WikiWebpart;
            if (_faqWebpart != null)
            {
                _faqWebpart.WikiWebpartSettings = _control.GetDocumentWebpartSettings;
                _faqWebpart.SaveChanges();
            }
            return true;
        }

        public override void SyncChanges()
        {
            EnsureChildControls();

            _faqWebpart = this.WebPartToEdit as WikiWebpart;
            if (_faqWebpart != null)
            {
                _faqWebpartSettings = _faqWebpart.WikiWebpartSettings;
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

            _control = Page.LoadControl(EditorControlPath) as WikiPropertyControl;
            if (_control != null)
            {
                _control.ID = "WikiPropertyControl";
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
