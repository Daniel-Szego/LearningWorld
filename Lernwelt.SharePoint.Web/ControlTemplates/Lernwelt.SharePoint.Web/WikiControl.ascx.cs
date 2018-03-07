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
using Lernwelt.SharePoint.Web.Webparts.WikiWebpart;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Taxonomy;
using Microsoft.SharePoint.WebControls;
using Lernwelt.SharePoint.Business.Utilities;
using System.Reflection;

namespace Lernwelt.SharePoint.Web.Controls
{
    public partial class WikiControl : UserControl
    {
        public WikiWebpart WebPart { get; set; }

        public bool IsNew { get; set; }

        public int wikiID { get; set; }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (WebPart.WikiWebpartSettings != null)
                {

                    using (var repository = new SharePointRepository())
                    {
                        dlLanguage.DataSource = repository.GetLanguages();
                        dlLanguage.DataValueField = "Key";
                        dlLanguage.DataTextField = "Value";
                        dlTargetGroup.DataSource = repository.GetTargetGroup();
                        dlTargetGroup.DataValueField = "Key";
                        dlTargetGroup.DataTextField = "Value";
                        lsbLayouts.DataTextField = "Title";
                        lsbLayouts.DataValueField = "Name";
                        lsbLayouts.DataSource = repository.getPageLayouts();

                        lsbLayouts.DataBind();
                        dlLanguage.DataBind();
                        dlTargetGroup.DataBind();

                        if (!IsNew)
                        {
                            LoadDataToControl();
                            if (!dlLanguage.SelectedValue.Contains(Constants.Lists.MtLanguage.EnglishValue))
                            {
                                LoadWIKIList();
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
            if (WebPart.WikiWebpartSettings != null)
            {
                using (var repository = new SharePointRepository())
                {
                    var taxField = repository.GetTaxonomyValues(WebPart.WikiWebpartSettings.ListId, WebPart.WikiWebpartSettings.Keywords.Id);
                    taxKeywords.SspId.Add(taxField.SspId);
                    taxKeywords.TermSetId.Add(taxField.TermSetId);

                    taxField = repository.GetTaxonomyValues(WebPart.WikiWebpartSettings.ListId, WebPart.WikiWebpartSettings.KnowledgeArea.Id);
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
            if ((!IsNew) && (wikiID > -1))
            {
                var parameters = new Dictionary<FieldSettingsBL, object>
                    {
                        {new FieldSettingsBL(WebPart.WikiWebpartSettings.Title.Id, WebPart.WikiWebpartSettings.Title.Name),  string.Empty},
                        {new FieldSettingsBL(WebPart.WikiWebpartSettings.Description.Id, WebPart.WikiWebpartSettings.Description.Name),  string.Empty},
                        {new FieldSettingsBL(WebPart.WikiWebpartSettings.Language.Id, WebPart.WikiWebpartSettings.Language.Name), string.Empty},
                        {new FieldSettingsBL(WebPart.WikiWebpartSettings.TargetGroup.Id, WebPart.WikiWebpartSettings.TargetGroup.Name), string.Empty},
                        {new FieldSettingsBL(WebPart.WikiWebpartSettings.Author.Id, WebPart.WikiWebpartSettings.Author.Name), string.Empty},
                    };

                using (var repository = new SharePointRepository())
                {
                    // load controls
                    WikiParameter fparams = repository.GetWIKIParametersForId(
                                WebPart.WikiWebpartSettings.ListId,
                                wikiID,
                                parameters,
                                new KeyValuePair<Guid, string>(WebPart.WikiWebpartSettings.KnowledgeArea.Id, string.Empty),
                                new KeyValuePair<Guid, string>(WebPart.WikiWebpartSettings.Keywords.Id, string.Empty));

                    if (fparams.parameters != null)
                    {
                        // bind controls
                        foreach (var prs in fparams.parameters)
                        {
                            if (prs.Value != null)
                            {
                                if (prs.Key.Name.Equals(WebPart.WikiWebpartSettings.Title.Name))
                                {
                                    txtPageTitle.Text = prs.Value == null ? string.Empty : prs.Value.ToString();
                                }
                                else if (prs.Key.Name.Equals(WebPart.WikiWebpartSettings.Description.Name))
                                {
                                    rtxDescription.Text = prs.Value == null ? string.Empty : prs.Value.ToString();
                                }
                                else if (prs.Key.Name.Equals(WebPart.WikiWebpartSettings.Language.Name))
                                {
                                    int selectedIndex = 0;
                                    foreach (ListItem item in dlLanguage.Items)
                                    {
                                        if (Utils.EqWithoutHash(item.Text, prs.Value.ToString()))
                                            dlLanguage.SelectedIndex = selectedIndex;
                                        selectedIndex++;
                                    }
                                }
                                else if (prs.Key.Name.Equals(WebPart.WikiWebpartSettings.TargetGroup.Name))
                                {
                                    int selectedIndex = 0;
                                    foreach (ListItem item in dlTargetGroup.Items)
                                    {
                                        if (Utils.EqWithoutHash(item.Text, prs.Value.ToString()))
                                            dlTargetGroup.SelectedIndex = selectedIndex;
                                        selectedIndex++;
                                    }
                                }
                                else if (prs.Key.Name.Equals(WebPart.WikiWebpartSettings.Author.Name))
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

                        // taxonomy control
                        taxKnowledgeArea.Text = fparams.knowledgeTaxString;
                        taxKnowledgeArea.Text = fparams.knowledgeTaxString;
                        List<TermHelper> data = TermHelper.getTermHelperList(fparams.keywordTaxString);
                        taxKeywords.Text = fparams.keywordTaxString;

                        // setting layout
                        int selectedIndexLayout = 1;
                        foreach (ListItem item in lsbLayouts.Items)
                        {
                            if (item.Text.Equals(fparams.layoutName))
                                lsbLayouts.SelectedIndex = selectedIndexLayout;
                            selectedIndexLayout++;
                        }  

                        hdnStarNum.Value = fparams.rating.ToString();
                    }
                }
            }
        }


        protected void btnSubmit_OnClick(object sender, EventArgs e)
        {
            if (WebPart.WikiWebpartSettings != null && Page.IsValid)
            {
                var parameters = new Dictionary<FieldSettingsBL, object>
                    {
                        {new FieldSettingsBL(WebPart.WikiWebpartSettings.Title.Id, WebPart.WikiWebpartSettings.Title.Name),  txtPageTitle.Text},
                        {new FieldSettingsBL(WebPart.WikiWebpartSettings.Description.Id, WebPart.WikiWebpartSettings.Description.Name),  rtxDescription.Text},
                        {new FieldSettingsBL(WebPart.WikiWebpartSettings.Language.Id, WebPart.WikiWebpartSettings.Language.Name), dlLanguage.SelectedValue},
                        {new FieldSettingsBL(WebPart.WikiWebpartSettings.TargetGroup.Id, WebPart.WikiWebpartSettings.TargetGroup.Name), dlTargetGroup.SelectedValue},
                        {new FieldSettingsBL(WebPart.WikiWebpartSettings.Author.Id, WebPart.WikiWebpartSettings.Author.Name), string.Empty},
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
                    parameters.Add(new FieldSettingsBL(WebPart.WikiWebpartSettings.Author.Id, WebPart.WikiWebpartSettings.Author.Name), authorUser);

                    if (hdnStarNum.Value != null)
                    {
                        if (int.TryParse(hdnStarNum.Value.ToString(), out  rating))
                            rating = int.Parse(hdnStarNum.Value.ToString());
                    }
                }

                // refence Wiki
                ReferenceWIKIWrapper referenceWIKI = null;
                if (drpWikiItems.SelectedItem != null)
                    referenceWIKI = new ReferenceWIKIWrapper(drpWikiItems.SelectedItem.Value, drpWikiItems.SelectedItem.Text, WebPart.WikiWebpartSettings.ReferenceId.Id, WebPart.WikiWebpartSettings.ReferenceId.Name);

                // fileType
                string fileType = lsbLayouts.SelectedValue;

                //Start Page
                bool isStartPage = chkStartPage.Checked;

                // file name
                string fileName = txtPageTitle.Text;

                // upload a new file
                if (IsNew)
                {

                    using (var repository = new SharePointRepository())
                    {


                        SPUser authorUser = repository.GetSPUser(selectedAuthor);
                        parameters.Add(new FieldSettingsBL(WebPart.WikiWebpartSettings.Author.Id, WebPart.WikiWebpartSettings.Author.Name), authorUser);

                        repository.CreateWIKI(WebPart.WikiWebpartSettings.ListId,
                                                              parameters,
                                                              rating,
                                                              new KeyValuePair<Guid, string>(WebPart.WikiWebpartSettings.KnowledgeArea.Id, taxKnowledgeArea.Text),
                                                              new KeyValuePair<Guid, string>(WebPart.WikiWebpartSettings.Keywords.Id, TermHelper.getString(keywords)),
                                                              referenceWIKI,
                                                              fileType,
                                                              isStartPage,
                                                              fileName,
                                                              WebPart.WikiWebpartSettings.PageLink.Id
                                                              );

                    }


                }
                // modify exiting file
                else
                {
                    using (var repository = new SharePointRepository())
                    {
                        repository.UpdateWIKI(WebPart.WikiWebpartSettings.ListId,
                                          wikiID,
                                          parameters,
                                          rating,
                                          new KeyValuePair<Guid, string>(WebPart.WikiWebpartSettings.KnowledgeArea.Id, taxKnowledgeArea.Text),
                                          new KeyValuePair<Guid, string>(WebPart.WikiWebpartSettings.Keywords.Id, TermHelper.getString(keywords)),
                                          referenceWIKI
                                          );
                    }
                }
                if (this.IsNew)
                    ClearForm();
            }
        }

        protected void ClearForm()
        {
            txtPageTitle.Text = "";
            rtxDescription.Text = "";
            dlLanguage.SelectedIndex = 0;
            taxKnowledgeArea.Text = "";
            taxKeywords.Text = "";
            dlTargetGroup.SelectedIndex = 0;
            lsbLayouts.SelectedIndex = 0;
            setControlVisibility();
            hdnStarNum.Value = string.Empty;
        }

        protected void setControlVisibility()
        {

            if (IsNew)
            {
                lsbLayouts.Enabled = true;
                drpWikiItems.Visible = true;
            }
            else
            {
                lsbLayouts.Enabled = false;
                drpWikiItems.Visible = false;
            }

            // check whatever it is an english
            if (dlLanguage.SelectedValue.Contains(Constants.Lists.MtLanguage.EnglishValue))
            {
                drpWikiItems.Visible = false;
                btnRefresh.Visible = false;
            }
            else
            {
                drpWikiItems.Visible = true;
                btnRefresh.Visible = true;
            }
        }

        protected void dlLanguage_OnSelectedIndexChanged(object sender, EventArgs e)
        {
            setControlVisibility();
            if (!dlLanguage.SelectedValue.Contains(Constants.Lists.MtLanguage.EnglishValue))
            {
                LoadWIKIList();
            }
        }

        protected void btnRefresh_Click(object sender, EventArgs e)
        {
            LoadWIKIList();
        }

        protected void LoadWIKIList()
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
                List<WikiWrapper> faqsToAssociate = repository.getPossibleAssosiationWIKI(
                    WebPart.WikiWebpartSettings.ListId,
                    new KeyValuePair<Guid, string>(WebPart.WikiWebpartSettings.KnowledgeArea.Id, taxKnowledgeArea.Text),
                    new KeyValuePair<Guid, string>(WebPart.WikiWebpartSettings.Keywords.Id, TermHelper.getString(keywords)),
                    WebPart.WikiWebpartSettings.Title.Id,
                    WebPart.WikiWebpartSettings.Language.Id);

                drpWikiItems.DataTextField = "Name";
                drpWikiItems.DataValueField = "intId";
                drpWikiItems.DataSource = faqsToAssociate;
                drpWikiItems.DataBind();
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
