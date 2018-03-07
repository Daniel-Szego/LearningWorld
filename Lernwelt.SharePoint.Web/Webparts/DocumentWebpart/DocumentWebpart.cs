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

namespace Lernwelt.SharePoint.Web.Webparts.DocumentWebpart
{
    [ToolboxItemAttribute(false)]
    public class DocumentWebpart : WebPart
    {
        private const string _ascxPath = @"~/_CONTROLTEMPLATES/Lernwelt.SharePoint.Web/DocumentControl.ascx";

        [Personalizable(true)]
        public DocumentWebpartSettings DocumentWebpartSettings
        {
            get;
            set;
        }

        protected override void CreateChildControls()
        {
            base.CreateChildControls();
            //EnsurePanelFix();

            DocumentControl documentControl = Page.LoadControl(_ascxPath) as DocumentControl;
            if (documentControl != null)
            {
                documentControl.WebPart = this;

                // query string parameter
                if (this.Page.Request.QueryString["LernweltDocID"] != null)
                {
                    documentControl.IsNew = false;
                    documentControl.documentID = int.Parse(this.Page.Request.QueryString["LernweltDocID"].ToString());
                }
                else
                {
                    documentControl.IsNew = true;
                    documentControl.documentID = -1;                   
                }

            }
            Controls.Add(documentControl);
        }

        public void SaveChanges()
        {
            this.SetPersonalizationDirty();
        }

        public override EditorPartCollection CreateEditorParts()
        {
            var editorPart = new DocumentListSelectorEditorPart { ID = this.ID + "_DocumentListSelectorEditorPart" };

            // Create a collection of Editor Parts and add them to the Editor Part collection.
            var editors = new List<EditorPart> { editorPart };
            return new EditorPartCollection(editors);
        }

            private void EnsurePanelFix()
            {
               if (this.Page.Form != null)
               {
                 String fixupScript = @"
                 _spBodyOnLoadFunctionNames.push(""_initFormActionAjax"");
                 function _initFormActionAjax()
                 {
                   if (_spEscapedFormAction == document.forms[0].action)
                   {
                     document.forms[0]._initialAction = 
                     document.forms[0].action;
                   }
                 }
                 var RestoreToOriginalFormActionCore = 
                   RestoreToOriginalFormAction;
                 RestoreToOriginalFormAction = function()
                 {
                   if (_spOriginalFormAction != null)
                   {
                     RestoreToOriginalFormActionCore();
                     document.forms[0]._initialAction = 
                     document.forms[0].action;
                   }
                 }";
               ScriptManager.RegisterStartupScript(this, 
                 typeof(DocumentWebpart), "UpdatePanelFixup", 
                 fixupScript, true);
               }
        }

    }
}
