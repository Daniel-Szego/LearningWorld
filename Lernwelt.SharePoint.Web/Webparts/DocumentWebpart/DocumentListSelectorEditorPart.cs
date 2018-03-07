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

namespace Lernwelt.SharePoint.Web.Webparts.DocumentWebpart
{
    public sealed class DocumentListSelectorEditorPart : EditorPart
    {
        private const string EditorControlPath = @"~/_CONTROLTEMPLATES/Lernwelt.SharePoint.Web/DocumentPropertyControl.ascx";

        private DocumentPropertyControl _control;
        private DocumentWebpart _documentWebpart;
        private DocumentWebpartSettings _documentWebpartSettings;

        public DocumentListSelectorEditorPart()
        {
            Title = LocalizeHelper.GetLocalizedString("Control_Documents_Property_Title");
        }

        public override bool ApplyChanges()
        {
            _documentWebpart = this.WebPartToEdit as DocumentWebpart;
            if (_documentWebpart != null)
            {
                _documentWebpart.DocumentWebpartSettings = _control.GetDocumentWebpartSettings;
                _documentWebpart.SaveChanges();
            }
            return true;
        }

        public override void SyncChanges()
        {
            EnsureChildControls();

            _documentWebpart = this.WebPartToEdit as DocumentWebpart;
            if (_documentWebpart != null)
            {
                _documentWebpartSettings = _documentWebpart.DocumentWebpartSettings;
                if (_control != null && _documentWebpartSettings != null)
                {
                    _control.OriginalValues = _documentWebpartSettings;
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

            _control = Page.LoadControl(EditorControlPath) as DocumentPropertyControl;
            if (_control != null)
            {
                _control.ID = "DocumentPropertyControl";
                Controls.Add(_control);
            }
        }

        void Cancel_Click(object sender, EventArgs e)
        {
            // On Cancel rollback all the changes by restoring the Original List
            if (_control.OriginalValues != null)
            {
                _documentWebpartSettings = _control.OriginalValues;
                ApplyChanges();
            }
        }
    }
}
