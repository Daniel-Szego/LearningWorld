using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Lernwelt.SharePoint.Web.Utilities;
using Lernwelt.SharePoint.Business;
using Microsoft.SharePoint;
using System.Reflection;
using System.Collections.Generic;
using Microsoft.SharePoint.WebControls;
using Microsoft.SharePoint.Taxonomy;

namespace Lernwelt.SharePoint.Web.ControlTemplates.Lernwelt.SharePoint.Web
{
    public partial class WikiRatingPropertyControl : UserControl
    {
        public WikiRatingWebpartSettings OriginalValues
        {
            get
            {
                return ViewState["OriginalValues"] as WikiRatingWebpartSettings;
            }
            set
            {
                ViewState["OriginalValues"] = value;
            }
        }


        public WikiRatingWebpartSettings GetDocumentWebpartSettings
        {
            get
            {
                var returnValue = new WikiRatingWebpartSettings
                {
                    ListId = new Guid(ddlDocumentLibraries.SelectedValue),
                };

                returnValue.LinkField = GetFieldSetting(returnValue.ListId, new Guid(ddlLink.SelectedValue), SPFieldType.Text);
                returnValue.KeywordsField = GetFieldSetting(returnValue.ListId, new Guid(ddlKeywords.SelectedValue), SPFieldType.Choice);
                returnValue.Keywords = taxKeywords.Text;

                try
                {
                    returnValue.EntryLimit =  int.Parse(txtEntryLimit.Text);
                    returnValue.OpenLink = (OpenStyle)(int.Parse(ddlOpenLink.SelectedValue));
                }
                catch (Exception ex)
                { }

                returnValue.LinkTitle = txtTitle.Text;

                return returnValue;
            }
        }

        private FieldSettings GetFieldSetting(Guid listId, Guid fieldId, SPFieldType fieldType)
        {
            var returnValue = new FieldSettings { Id = fieldId, Type = fieldType };

            using (var repository = new SharePointRepository())
            {
                returnValue.Name = repository.GetFieldInternalNameById(listId, fieldId);
            }

            return returnValue;
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (hiddenFieldDetectRequest.Value == "0")
            {
                ltrVersion.Text = Assembly.GetExecutingAssembly().GetName().Version.ToString();

                LoadData();

                hiddenFieldDetectRequest.Value = "1";
            }

            initTaxonomyFields();
        }


        private void LoadData()
        {
            using (var repository = new SharePointRepository())
            {
                var documentLibaries = repository.GetDocumentLibraries();

                ddlDocumentLibraries.DataSource = documentLibaries;
                ddlDocumentLibraries.DataTextField = "Value";
                ddlDocumentLibraries.DataValueField = "Key";
                ddlDocumentLibraries.DataBind();
            }

            if (OriginalValues != null)
            {
                ddlDocumentLibraries.Items.FindByValue(OriginalValues.ListId.ToString()).Selected = true;
                ChangeListData(OriginalValues.ListId);

                // Setting
                ddlLink.Items.FindByValue(OriginalValues.LinkField.Id.ToString()).Selected = true;
                ddlKeywords.Items.FindByValue(OriginalValues.KeywordsField.Id.ToString()).Selected = true;

                taxKeywords.Text = OriginalValues.LinkTitle;
                txtEntryLimit.Text = OriginalValues.EntryLimit.ToString();
                taxKeywords.Text = OriginalValues.Keywords;
                ddlOpenLink.Items.FindByValue(((int)OriginalValues.OpenLink).ToString()).Selected = true;
            }
        }

        private void ChangeListData(Guid listId)
        {
            using (var repository = new SharePointRepository())
            {
                BindData(ddlLink, repository.GetCompatibleSPFields(SPFieldType.Text, listId));
                BindData(ddlKeywords, repository.GetTaxonomyField(listId));
            }
        }

        private void BindData(DropDownList dropdownList, List<KeyValuePair<Guid, string>> data)
        {
            dropdownList.DataSource = data;
            dropdownList.DataTextField = "Value";
            dropdownList.DataValueField = "Key";
            dropdownList.DataBind();
        }

        protected void ddlDocumentLibraries_OnSelectedIndexChanged(object sender, EventArgs e)
        {
            var list = new Guid(ddlDocumentLibraries.SelectedValue);

            ChangeListData(list);
        }

        /// <summary>
        /// reinit taxonomy fields
        /// </summary>
        public void initTaxonomyFields()
        {
            if (this.OriginalValues != null)
            {
                using (var repository = new SharePointRepository())
                {
                    var taxField = repository.GetTaxonomyValues(this.OriginalValues.ListId, this.OriginalValues.KeywordsField.Id);
                    taxKeywords.SspId.Add(taxField.SspId);
                    taxKeywords.TermSetId.Add(taxField.TermSetId);
                }
            }
        }


        #region Hacking_TaxonomyField_Postback

        protected Boolean IsAsyncPostBack
        {
            get
            {
                return ScriptManager.GetCurrent(Page).IsInAsyncPostBack;
            }
        }

        private Boolean IsInPartialRendering(Control control)
        {
            if (control is UpdatePanel && ((UpdatePanel)control).IsInPartialRendering)
                return true;
            if (control.Parent != null)
                return IsInPartialRendering(control.Parent);
            return false;
        }
        protected override void Render(HtmlTextWriter writer)
        {
            base.Render(writer);
            // Fix advanced field
            if (IsAsyncPostBack)
            {
                WebPartManager wpm = WebPartManager.GetCurrentWebPartManager(Page);
                if (((SPContext.Current.FormContext.FormMode == SPControlMode.Edit) || wpm.DisplayMode.Name.Equals("Design") || wpm.DisplayMode.Name.Equals("Edit")))
                {

                    // Get BaseFieldControls to fix
                    List<TaxonomyWebTaggingControl> taxonomyFieldControlList = new List<TaxonomyWebTaggingControl>();
                    taxonomyFieldControlList.Add(taxKeywords);
                    // Script Header
                    String stringScript = "<script>\n";
                    // Fix TaxonomyFieldControl
                    foreach (TaxonomyWebTaggingControl taxonomyFieldControlTemp in taxonomyFieldControlList)
                    {
                        Type typeTaxonomyWebTaggingControl = typeof(TaxonomyWebTaggingControl);
                        MethodInfo methodInfoGetOnloadJavascript = typeTaxonomyWebTaggingControl.GetMethod("getOnloadJavascript", BindingFlags.NonPublic | BindingFlags.Instance);
                        //TaxonomyWebTaggingControl taxonomyWebTaggingControl = ((TaxonomyWebTaggingControl)taxonomyFieldControlTemp.Controls[0]);
                        stringScript += ((String)methodInfoGetOnloadJavascript.Invoke(taxonomyFieldControlTemp, null));
                    }
                    // Fix RichTextField
                    // Script Footer
                    stringScript += "</script>\n";
                    // Execute Script
                    ScriptManager.RegisterStartupScript(Page, Page.GetType(), "Page_Load", stringScript, false);
                }
            }
        }

        #endregion

    }
}
