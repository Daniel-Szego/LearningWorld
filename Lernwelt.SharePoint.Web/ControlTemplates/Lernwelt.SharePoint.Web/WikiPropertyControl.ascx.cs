using System;
using System.Collections.Generic;
using System.Reflection;
using System.Web.UI;
using System.Web.UI.WebControls;
using Lernwelt.SharePoint.Business;
using Lernwelt.SharePoint.Web.Utilities;
using Microsoft.SharePoint;

namespace Lernwelt.SharePoint.Web.Controls
{
    public partial class WikiPropertyControl : UserControl
    {
        public WikiWebpartSettings OriginalValues
        {
            get
            {
                return ViewState["OriginalValues"] as WikiWebpartSettings;
            }
            set
            {
                ViewState["OriginalValues"] = value;
            }
        }



        public WikiWebpartSettings GetDocumentWebpartSettings
        {
            get
            {
                var returnValue = new WikiWebpartSettings
                {
                    ListId = new Guid(ddlDocumentLibraries.SelectedValue),
                };

                returnValue.Title = GetFieldSetting(returnValue.ListId, new Guid(ddlTitle.SelectedValue), SPFieldType.Text);
                returnValue.Description = GetFieldSetting(returnValue.ListId, new Guid(ddlDescription.SelectedValue), SPFieldType.Note);
                returnValue.Language = GetFieldSetting(returnValue.ListId, new Guid(ddlLanguage.SelectedValue), SPFieldType.Lookup);
                returnValue.KnowledgeArea = GetFieldSetting(returnValue.ListId, new Guid(ddlKnowledgeArea.SelectedValue), SPFieldType.Choice);
                returnValue.Keywords = GetFieldSetting(returnValue.ListId, new Guid(ddlKeywords.SelectedValue), SPFieldType.Choice);
                returnValue.Author = GetFieldSetting(returnValue.ListId, new Guid(ddlAuthor.SelectedValue), SPFieldType.User);
                returnValue.TargetGroup = GetFieldSetting(returnValue.ListId, new Guid(ddlTargetGroup.SelectedValue), SPFieldType.Lookup);
                returnValue.ReferenceId = GetFieldSetting(returnValue.ListId, new Guid(ddlReference.SelectedValue), SPFieldType.Lookup);
                returnValue.PageLink = GetFieldSetting(returnValue.ListId, new Guid(ddlPageLink.SelectedValue), SPFieldType.URL);

                return returnValue;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (hiddenFieldDetectRequest.Value == "0")
            {
                ltrVersion.Text = Assembly.GetExecutingAssembly().GetName().Version.ToString();

                LoadData();

                hiddenFieldDetectRequest.Value = "1";
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
                ddlTitle.Items.FindByValue(OriginalValues.Title.Id.ToString()).Selected = true;
                ddlDescription.Items.FindByValue(OriginalValues.Description.Id.ToString()).Selected = true;
                ddlLanguage.Items.FindByValue(OriginalValues.Language.Id.ToString()).Selected = true;
                ddlKnowledgeArea.Items.FindByValue(OriginalValues.KnowledgeArea.Id.ToString()).Selected = true;
                ddlKeywords.Items.FindByValue(OriginalValues.Keywords.Id.ToString()).Selected = true;
                ddlAuthor.Items.FindByValue(OriginalValues.Author.Id.ToString()).Selected = true;
                ddlTargetGroup.Items.FindByValue(OriginalValues.TargetGroup.Id.ToString()).Selected = true;
                ddlReference.Items.FindByValue(OriginalValues.ReferenceId.Id.ToString()).Selected = true;
                ddlPageLink.Items.FindByValue(OriginalValues.PageLink.Id.ToString()).Selected = true;
            }
        }

        private void ChangeListData(Guid listId)
        {
            using (var repository = new SharePointRepository())
            {
                BindData(ddlTitle, repository.GetCompatibleSPFields(SPFieldType.Text, listId));
                BindData(ddlDescription, repository.GetCompatibleSPFields(SPFieldType.Note, listId));
                BindData(ddlLanguage, repository.GetCompatibleSPFields(SPFieldType.Lookup, listId));
                BindData(ddlKnowledgeArea, repository.GetTaxonomyField(listId));
                BindData(ddlKeywords, repository.GetTaxonomyField(listId));
                BindData(ddlAuthor, repository.GetCompatibleSPFields(SPFieldType.User, listId));
                BindData(ddlTargetGroup, repository.GetCompatibleSPFields(SPFieldType.Lookup, listId));
                BindData(ddlReference, repository.GetCompatibleSPFields(SPFieldType.Lookup, listId));
                BindData(ddlPageLink, repository.GetCompatibleSPFields(SPFieldType.URL, listId));
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
    }
}
