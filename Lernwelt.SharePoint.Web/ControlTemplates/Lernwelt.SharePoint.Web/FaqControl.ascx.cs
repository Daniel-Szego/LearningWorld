using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Lernwelt.SharePoint.Business;
using Lernwelt.SharePoint.Web.Utilities;
using Lernwelt.SharePoint.Web.Webparts.DocumentWebpart;
using Lernwelt.SharePoint.Web.Webparts.FaqWebpart;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Taxonomy;
using Microsoft.SharePoint.WebControls;
using Lernwelt.SharePoint.Business.Utilities;
using System.Reflection;

namespace Lernwelt.SharePoint.Web.Controls
{
    public partial class FaqControl : UserControl
    {
        public FaqWebpart WebPart { get; set; }

        public bool IsNew { get; set; }

        public int faqID { get; set; }


        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
              if (WebPart.FaqWebpartSettings != null)
                    {
                        using (var repository = new SharePointRepository())
                        {
                            dlLanguage.DataSource = repository.GetLanguages();
                            dlLanguage.DataValueField = "Key";
                            dlLanguage.DataTextField = "Value";
                            dlTargetGroup.DataSource = repository.GetTargetGroup();
                            dlTargetGroup.DataValueField = "Key";
                            dlTargetGroup.DataTextField = "Value";

                            dlLanguage.DataBind();
                            dlTargetGroup.DataBind();

                            if (!IsNew)
                            {
                                LoadDataToControl();
                                if (!dlLanguage.SelectedValue.Contains(Constants.Lists.MtLanguage.EnglishValue))
                                {
                                    LoadFAQList();
                                }
                            }
                     }
                }
            }
            
            initTaxonomyFields();
            setControlVisibility();
        }


        /// <summary>
        /// reinit taxonomy fields
        /// </summary>
        public void initTaxonomyFields()
        {
            if (WebPart.FaqWebpartSettings != null)
            {
                using (var repository = new SharePointRepository())
                {
                    var taxField = repository.GetTaxonomyValues(WebPart.FaqWebpartSettings.ListId, WebPart.FaqWebpartSettings.Keywords.Id);
                    taxKeywords.SspId.Add(taxField.SspId);
                    taxKeywords.TermSetId.Add(taxField.TermSetId);

                    taxField = repository.GetTaxonomyValues(WebPart.FaqWebpartSettings.ListId, WebPart.FaqWebpartSettings.KnowledgeArea.Id);
                    taxKnowledgeArea.SspId.Add(taxField.SspId);
                    taxKnowledgeArea.TermSetId.Add(taxField.TermSetId);
                }
            }
        }


        /// <summary>
        /// Loading the data to the control if it is not a new upload
        /// </summary>
        protected void LoadDataToControl()
        {
            if ((!IsNew) && (faqID > -1))
            {
                var parameters = new Dictionary<FieldSettingsBL, object>
                    {
                        {new FieldSettingsBL(WebPart.FaqWebpartSettings.Question.Id, WebPart.FaqWebpartSettings.Question.Name),  string.Empty},
                        {new FieldSettingsBL(WebPart.FaqWebpartSettings.Answer.Id, WebPart.FaqWebpartSettings.Answer.Name), string.Empty},
                        {new FieldSettingsBL(WebPart.FaqWebpartSettings.Language.Id, WebPart.FaqWebpartSettings.Language.Name), string.Empty},
                        {new FieldSettingsBL(WebPart.FaqWebpartSettings.TargetGroup.Id, WebPart.FaqWebpartSettings.TargetGroup.Name), string.Empty},
                        {new FieldSettingsBL(WebPart.FaqWebpartSettings.Author.Id, WebPart.FaqWebpartSettings.Author.Name), string.Empty},

                    };

                using (var repository = new SharePointRepository())
                {
                    // load controls
                    FAQParameter fparams = repository.GetFaqParametersForId(
                                WebPart.FaqWebpartSettings.ListId,
                                faqID,
                                parameters,
                                new KeyValuePair<Guid, string>(WebPart.FaqWebpartSettings.KnowledgeArea.Id, string.Empty),
                                new KeyValuePair<Guid, string>(WebPart.FaqWebpartSettings.Keywords.Id, string.Empty));

                    if (fparams.parameters != null)
                    {
                        // bind controls
                        foreach (var prs in fparams.parameters)
                        {
                            if (prs.Value != null)
                            {
                                if (prs.Key.Name.Equals(WebPart.FaqWebpartSettings.Question.Name))
                                {
                                    txtQuestion.Text = prs.Value == null ? string.Empty : prs.Value.ToString();
                                }
                                else if (prs.Key.Name.Equals(WebPart.FaqWebpartSettings.Answer.Name))
                                {
                                    rtxAnswer.Text = prs.Value == null ? string.Empty : prs.Value.ToString();
                                }
                                else if (prs.Key.Name.Equals(WebPart.FaqWebpartSettings.Language.Name))
                                {
                                    int selectedIndex = 0;
                                    foreach (ListItem item in dlLanguage.Items)
                                    {
                                        if (Utils.EqWithoutHash(item.Text,prs.Value.ToString()))
                                            dlLanguage.SelectedIndex = selectedIndex;
                                        selectedIndex++;
                                    }
                                }
                                else if (prs.Key.Name.Equals(WebPart.FaqWebpartSettings.TargetGroup.Name))
                                {
                                    int selectedIndex = 0;
                                    foreach (ListItem item in dlTargetGroup.Items)
                                    {
                                        if (Utils.EqWithoutHash(item.Text,prs.Value.ToString()))
                                            dlTargetGroup.SelectedIndex = selectedIndex;
                                        selectedIndex++;
                                    }
                                }
                                else if (prs.Key.Name.Equals(WebPart.FaqWebpartSettings.Author.Name))
                                {
                                    if (prs.Value != null)
                                    {
                                        int peopleID = 0;
                                        try
                                        {
                                            peopleID = int.Parse(Utils.removeHashLeft(prs.Value.ToString()));
                                        }
                                        catch (Exception ex)
                                        { }

                                        SPUser user = repository.GetSPUserByID(peopleID);
                                        PickerAddUser.CommaSeparatedAccounts = user.LoginName;
                                        PickerAddUser.Validate();
                                    }
                                }


                            }
                        }

                        // setting the Taxonomy fields
                        taxKnowledgeArea.Text = fparams.knowledgeTaxString;

                        List<TermHelper> data = TermHelper.getTermHelperList(fparams.keywordTaxString);
                        taxKeywords.Text = fparams.keywordTaxString;

                        hdnStarNum.Value = fparams.rating.ToString();
                    }
                }
            }
        }


        protected void btnSubmit_OnClick(object sender, EventArgs e)
        {
            if (WebPart.FaqWebpartSettings != null && Page.IsValid)
            {

                var parameters = new Dictionary<FieldSettingsBL, object>
                {
                    {new FieldSettingsBL(WebPart.FaqWebpartSettings.Question.Id, WebPart.FaqWebpartSettings.Question.Name), txtQuestion.Text},
                    {new FieldSettingsBL(WebPart.FaqWebpartSettings.Answer.Id, WebPart.FaqWebpartSettings.Answer.Name), rtxAnswer.Text},
                    {new FieldSettingsBL(WebPart.FaqWebpartSettings.Language.Id, WebPart.FaqWebpartSettings.Language.Name), dlLanguage.SelectedValue},
                    {new FieldSettingsBL(WebPart.FaqWebpartSettings.TargetGroup.Id, WebPart.FaqWebpartSettings.TargetGroup.Name), dlTargetGroup.SelectedValue},
                };

                // author
                string selectedAuthor = (from PickerEntity pickEn in PickerAddUser.Entities select pickEn.Key).FirstOrDefault();

                // Taxonomy fields
                List<TermHelper> keywords = new List<TermHelper>();
                TaxonomyFieldValueCollection values = new TaxonomyFieldValueCollection(String.Empty);
                values.PopulateFromLabelGuidPairs(taxKeywords.Text);

                foreach (TaxonomyFieldValue value in values)
                {
                    keywords.Add(new TermHelper(value.Label, value.TermGuid));
                }

                // rating
                int rating = -1;
                using (var repository = new SharePointRepository())
                {
                    SPUser authorUser = repository.GetSPUser(selectedAuthor);
                    parameters.Add(new FieldSettingsBL(WebPart.FaqWebpartSettings.Author.Id, WebPart.FaqWebpartSettings.Author.Name), authorUser);

                    if (hdnStarNum.Value != null)
                    {
                        if (int.TryParse(hdnStarNum.Value.ToString(), out  rating))
                            rating = int.Parse(hdnStarNum.Value.ToString());
                    }
                }

                // refence FAQ
                ReferenceFAQWrapper referenceFAQ = null;
                if (drpFaqItems.SelectedItem != null)
                    referenceFAQ = new ReferenceFAQWrapper(drpFaqItems.SelectedItem.Value, drpFaqItems.SelectedItem.Text, WebPart.FaqWebpartSettings.ReferenceId.Id, WebPart.FaqWebpartSettings.ReferenceId.Name);

                // upload a new file
                if (IsNew)
                {

                    using (var repository = new SharePointRepository())
                    {


                        SPUser authorUser = repository.GetSPUser(selectedAuthor);
                        parameters.Add(new FieldSettingsBL(WebPart.FaqWebpartSettings.Author.Id, WebPart.FaqWebpartSettings.Author.Name), authorUser);

                        repository.CreateFaq(WebPart.FaqWebpartSettings.ListId,
                                                              parameters,
                                                              rating,
                                                              new KeyValuePair<Guid, string>(WebPart.FaqWebpartSettings.KnowledgeArea.Id, taxKnowledgeArea.Text),
                                                              new KeyValuePair<Guid, string>(WebPart.FaqWebpartSettings.Keywords.Id, TermHelper.getString(keywords)),
                                                              referenceFAQ
                                                              );

                    }
                }
                // modify exiting file
                else
                {
                    using (var repository = new SharePointRepository())
                    {
                        repository.UpdateFAQ(WebPart.FaqWebpartSettings.ListId,
                                          faqID,
                                          parameters,
                                          rating,
                                          new KeyValuePair<Guid, string>(WebPart.FaqWebpartSettings.KnowledgeArea.Id, taxKnowledgeArea.Text),
                                          new KeyValuePair<Guid, string>(WebPart.FaqWebpartSettings.Keywords.Id, TermHelper.getString(keywords)),
                                          referenceFAQ
                                          );
                    }
                }
                if (this.IsNew)
                    ClearForm();
            }
        }

        protected void ClearForm()
        {
            rtxAnswer.Text = "";
            txtQuestion.Text = "";
            dlLanguage.SelectedIndex = 0;
            taxKnowledgeArea.Text = "";
            taxKeywords.Text = "";
            dlTargetGroup.SelectedIndex = 0;
            setControlVisibility();
            hdnStarNum.Value = string.Empty;
        }


        protected void setControlVisibility()
        {

            if (IsNew)
            {
                drpFaqItems.Visible = true;
            }
            else
            {
                drpFaqItems.Visible = false;
            }

            // check whatever it is an english
            if (dlLanguage.SelectedValue.Contains(Constants.Lists.MtLanguage.EnglishValue))
            {
                drpFaqItems.Visible = false;
                btnRefresh.Visible = false;
            }
            else
            {
                drpFaqItems.Visible = true;
                btnRefresh.Visible = true;
            }
        }

        protected void dlLanguage_OnSelectedIndexChanged(object sender, EventArgs e)
        {
            setControlVisibility();
            if (!dlLanguage.SelectedValue.Contains(Constants.Lists.MtLanguage.EnglishValue))
            {
                LoadFAQList();
            }
        }

        protected void btnRefresh_Click(object sender, EventArgs e)
        {
            LoadFAQList();
        }

        protected void LoadFAQList()
        {
            // Taxonomy fields
            List<TermHelper> keywords = new List<TermHelper>();
            TaxonomyFieldValueCollection values = new TaxonomyFieldValueCollection(String.Empty);
            values.PopulateFromLabelGuidPairs(taxKeywords.Text);

            foreach (TaxonomyFieldValue value in values)
            {
                keywords.Add(new TermHelper(value.Label, value.TermGuid));
            }

            using (var repository = new SharePointRepository())
            {
                List<FAQWrapper> faqsToAssociate = repository.getPossibleAssosiationFAQ(
                    WebPart.FaqWebpartSettings.ListId,
                    new KeyValuePair<Guid, string>(WebPart.FaqWebpartSettings.KnowledgeArea.Id, taxKnowledgeArea.Text),
                    new KeyValuePair<Guid, string>(WebPart.FaqWebpartSettings.Keywords.Id, TermHelper.getString(keywords)),
                    WebPart.FaqWebpartSettings.Question.Id,
                    WebPart.FaqWebpartSettings.Language.Id);

                drpFaqItems.DataTextField = "Name";
                drpFaqItems.DataValueField = "intId";
                drpFaqItems.DataSource = faqsToAssociate;
                drpFaqItems.DataBind();
                //txtNum.Text = filesToAssociate.Count.ToString();
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
                if (!((SPContext.Current.FormContext.FormMode == SPControlMode.Edit) || wpm.DisplayMode.Name.Equals("Design") || wpm.DisplayMode.Name.Equals("Edit")))
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


        #region Server_Side_Validators

        protected void taxKnowledgeArea_ServerValidate(object source, System.Web.UI.WebControls.ServerValidateEventArgs args)
        {
            if (taxKnowledgeArea.Text.Equals(string.Empty))
            {
                args.IsValid = false;
                taxKnowledgeAreaValidator.ErrorMessage = LocalizeHelper.GetLocalizedString("KnowedgeArea_Not_Found_Error");
            }
        }

        //protected void taxKeywords_ServerValidate(object source, System.Web.UI.WebControls.ServerValidateEventArgs args)
        //{
        //    if (taxKeywords.Text.Equals(string.Empty))
        //    {
        //        args.IsValid = false;
        //        taxKeywordsValidator.ErrorMessage = LocalizeHelper.GetLocalizedString("Keyword_Not_Found_Error");
        //    }
        //}

        #endregion
    }
}
