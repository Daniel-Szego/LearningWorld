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
    public partial class FaqPropertyControl : UserControl
    {
        public FaqWebpartSettings OriginalValues
        {
            get
            {
                return ViewState["OriginalValues"] as FaqWebpartSettings;
            }
            set
            {
                ViewState["OriginalValues"] = value;
            }
        }

        public FaqWebpartSettings GetDocumentWebpartSettings
        {
            get
            {
                var returnValue = new FaqWebpartSettings
                {
                    ListId = new Guid(ddlDocumentLibraries.SelectedValue),
                };

                returnValue.Question = GetFieldSetting(returnValue.ListId, new Guid(ddlQuestion.SelectedValue), SPFieldType.Text);
                returnValue.Answer = GetFieldSetting(returnValue.ListId, new Guid(ddlAnswer.SelectedValue), SPFieldType.Note);
                returnValue.Language = GetFieldSetting(returnValue.ListId, new Guid(ddlLanguage.SelectedValue), SPFieldType.Lookup);
                returnValue.KnowledgeArea = GetFieldSetting(returnValue.ListId, new Guid(ddlKnowledgeArea.SelectedValue), SPFieldType.Choice);
                returnValue.Keywords = GetFieldSetting(returnValue.ListId, new Guid(ddlKeywords.SelectedValue), SPFieldType.Choice);
                returnValue.Author = GetFieldSetting(returnValue.ListId, new Guid(ddlAuthor.SelectedValue), SPFieldType.User);
                returnValue.TargetGroup = GetFieldSetting(returnValue.ListId, new Guid(ddlTargetGroup.SelectedValue), SPFieldType.Lookup);
                returnValue.ReferenceId = GetFieldSetting(returnValue.ListId, new Guid(ddlReference.SelectedValue), SPFieldType.Lookup);
                //returnValue.NoOfRatings = GetFieldSetting(returnValue.ListId, new Guid(ddlNoOfRatings.SelectedValue), SPFieldType.Number);
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
                var documentLibaries = repository.GetLists();

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
                ddlQuestion.Items.FindByValue(OriginalValues.Question.Id.ToString()).Selected = true;
                ddlAnswer.Items.FindByValue(OriginalValues.Answer.Id.ToString()).Selected = true;
                ddlLanguage.Items.FindByValue(OriginalValues.Language.Id.ToString()).Selected = true;
                ddlKnowledgeArea.Items.FindByValue(OriginalValues.KnowledgeArea.Id.ToString()).Selected = true;
                ddlKeywords.Items.FindByValue(OriginalValues.Keywords.Id.ToString()).Selected = true;
                ddlAuthor.Items.FindByValue(OriginalValues.Author.Id.ToString()).Selected = true;
                ddlTargetGroup.Items.FindByValue(OriginalValues.TargetGroup.Id.ToString()).Selected = true;
                ddlReference.Items.FindByValue(OriginalValues.ReferenceId.Id.ToString()).Selected = true;
                //ddlNoOfRatings.Items.FindByValue(OriginalValues.NoOfRatings.Id.ToString()).Selected = true;

            }
        }

        private void ChangeListData(Guid listId)
        {
            using (var repository = new SharePointRepository())
            {
                BindData(ddlQuestion, repository.GetCompatibleSPFields(SPFieldType.Text, listId));
                BindData(ddlAnswer, repository.GetCompatibleSPFields(SPFieldType.Note, listId));
                BindData(ddlLanguage, repository.GetCompatibleSPFields(SPFieldType.Lookup, listId));
                BindData(ddlKnowledgeArea, repository.GetTaxonomyField(listId));
                BindData(ddlKeywords, repository.GetTaxonomyField(listId));
                BindData(ddlAuthor, repository.GetCompatibleSPFields(SPFieldType.User, listId));
                BindData(ddlTargetGroup, repository.GetCompatibleSPFields(SPFieldType.Lookup, listId));
                BindData(ddlReference, repository.GetCompatibleSPFields(SPFieldType.Lookup, listId));
                //BindData(ddlNoOfRatings, repository.GetCompatibleSPFields(SPFieldType.Number, listId));

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
