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
using Microsoft.SharePoint;
using Microsoft.SharePoint.Taxonomy;
using Microsoft.SharePoint.WebControls;
using Lernwelt.SharePoint.Business.Utilities;
using Microsoft.SharePoint.Utilities;
using System.Reflection;
using System.Text;
using System.Globalization;

namespace Lernwelt.SharePoint.Web.Controls
{
    public partial class DocumentControl : UserControl
    {
        public DocumentWebpart WebPart { get; set; }

        public bool IsNew { get; set; }

        public int documentID {get; set;}

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                using (var repository = new SharePointRepository())
                {
                    if (WebPart.DocumentWebpartSettings != null)
                    {
                        dlDocumentType.DataSource = repository.GetDocumentType();
                        dlDocumentType.DataValueField = "Key";
                        dlDocumentType.DataTextField = "Value";
                        dlLanguage.DataSource = repository.GetLanguages();
                        dlLanguage.DataValueField = "Key";
                        dlLanguage.DataTextField = "Value";
                        dlTargetGroup.DataSource = repository.GetTargetGroup();
                        dlTargetGroup.DataValueField = "Key";
                        dlTargetGroup.DataTextField = "Value";

                        dlDocumentType.DataBind();
                        dlLanguage.DataBind();
                        dlTargetGroup.DataBind();

                        if (!IsNew)
                        {
                            LoadDataToControl();
                            if (!dlLanguage.SelectedValue.Contains(Constants.Lists.MtLanguage.EnglishValue))
                            {
                                LoadDocumentList();
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
            if (WebPart.DocumentWebpartSettings != null)
            {
                using (var repository = new SharePointRepository())
                {
                    var taxField = repository.GetTaxonomyValues(WebPart.DocumentWebpartSettings.ListId, WebPart.DocumentWebpartSettings.Keywords.Id);
                    taxKeywords.SspId.Add(taxField.SspId);
                    taxKeywords.TermSetId.Add(taxField.TermSetId);

                    taxField = repository.GetTaxonomyValues(WebPart.DocumentWebpartSettings.ListId, WebPart.DocumentWebpartSettings.KnowledgeArea.Id);
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
            if ((!IsNew) && (documentID > -1))
            {
                var parameters = new Dictionary<FieldSettingsBL, object>
                    {
                        {new FieldSettingsBL(WebPart.DocumentWebpartSettings.Name.Id, WebPart.DocumentWebpartSettings.Name.Name),  string.Empty},
                        {new FieldSettingsBL(WebPart.DocumentWebpartSettings.Title.Id, WebPart.DocumentWebpartSettings.Title.Name), string.Empty},
                        {new FieldSettingsBL(WebPart.DocumentWebpartSettings.DocumentType.Id, WebPart.DocumentWebpartSettings.DocumentType.Name), string.Empty},
                        {new FieldSettingsBL(WebPart.DocumentWebpartSettings.ShortDescription.Id, WebPart.DocumentWebpartSettings.ShortDescription.Name), string.Empty},
                        {new FieldSettingsBL(WebPart.DocumentWebpartSettings.Language.Id, WebPart.DocumentWebpartSettings.Language.Name), string.Empty},
                        {new FieldSettingsBL(WebPart.DocumentWebpartSettings.TargetGroup.Id, WebPart.DocumentWebpartSettings.TargetGroup.Name), string.Empty},
                        {new FieldSettingsBL(WebPart.DocumentWebpartSettings.Author.Id, WebPart.DocumentWebpartSettings.Author.Name), string.Empty},
                    };

                using (var repository = new SharePointRepository())
                {
                    // load controls
                    FileParameter fparams = repository.GetFileParametersForId(
                                WebPart.DocumentWebpartSettings.ListId,
                                documentID,
                                parameters,
                                new KeyValuePair<Guid, string>(WebPart.DocumentWebpartSettings.KnowledgeArea.Id, string.Empty),
                                new KeyValuePair<Guid, string>(WebPart.DocumentWebpartSettings.Keywords.Id, string.Empty));

                    // bind controls
                    if (fparams.parameters != null)
                    {
                        foreach (var prs in fparams.parameters)
                        {
                            if (prs.Value != null)
                            {
                                if (prs.Key.Name.Equals(WebPart.DocumentWebpartSettings.Name.Name))
                                {
                                    txtName.Text = prs.Value == null ? string.Empty : prs.Value.ToString();
                                }
                                else if (prs.Key.Name.Equals(WebPart.DocumentWebpartSettings.Title.Name))
                                {
                                    txtTitle.Text = prs.Value == null ? string.Empty : prs.Value.ToString();
                                }
                                else if (prs.Key.Name.Equals(WebPart.DocumentWebpartSettings.DocumentType.Name))
                                {
                                    int selectedIndex = 0;
                                    foreach (ListItem item in dlDocumentType.Items)
                                    {
                                        if (Utils.EqWithoutHash(item.Text, prs.Value.ToString()))
                                            dlDocumentType.SelectedIndex = selectedIndex;
                                        selectedIndex++;
                                    }
                                }
                                else if (prs.Key.Name.Equals(WebPart.DocumentWebpartSettings.ShortDescription.Name))
                                {
                                    rtxDescription.Text = prs.Value == null ? string.Empty : prs.Value.ToString();
                                }
                                else if (prs.Key.Name.Equals(WebPart.DocumentWebpartSettings.Language.Name))
                                {
                                    int selectedIndex = 0;
                                    foreach (ListItem item in dlLanguage.Items)
                                    {
                                        if (Utils.EqWithoutHash(item.Text,prs.Value.ToString()))
                                            dlLanguage.SelectedIndex = selectedIndex;
                                        selectedIndex++;
                                    }
                                }
                                else if (prs.Key.Name.Equals(WebPart.DocumentWebpartSettings.TargetGroup.Name))
                                {
                                    int selectedIndex = 0;
                                    foreach (ListItem item in dlTargetGroup.Items)
                                    {
                                        if (Utils.EqWithoutHash(item.Text,prs.Value.ToString()))
                                            dlTargetGroup.SelectedIndex = selectedIndex;
                                        selectedIndex++;
                                    }
                                }
                                else if (prs.Key.Name.Equals(WebPart.DocumentWebpartSettings.Author.Name))
                                {
                                    if (prs.Value != null)
                                    {
                                        int peopleID = 0;
                                        try
                                        {
                                            peopleID = int.Parse(Utils.removeHashLeft(prs.Value.ToString()));
                                        }
                                        catch (Exception ex)    
                                        {}

                                        SPUser user = repository.GetSPUserByID(peopleID);
                                        PickerAddUser.CommaSeparatedAccounts = user.LoginName;
                                        PickerAddUser.Validate();
                                    }
                                }
                            }
                        }

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
            if (WebPart.DocumentWebpartSettings != null && Page.IsValid)
            {
                if (Page.IsValid)
                {
                    var parameters = new Dictionary<FieldSettingsBL, object>
                    {
                        {new FieldSettingsBL(WebPart.DocumentWebpartSettings.Name.Id, WebPart.DocumentWebpartSettings.Name.Name),  txtName.Text},
                        {new FieldSettingsBL(WebPart.DocumentWebpartSettings.Title.Id, WebPart.DocumentWebpartSettings.Title.Name), txtTitle.Text},
                        {new FieldSettingsBL(WebPart.DocumentWebpartSettings.DocumentType.Id, WebPart.DocumentWebpartSettings.DocumentType.Name), dlDocumentType.SelectedValue},
                        {new FieldSettingsBL(WebPart.DocumentWebpartSettings.ShortDescription.Id, WebPart.DocumentWebpartSettings.ShortDescription.Name), rtxDescription.Text},
                        {new FieldSettingsBL(WebPart.DocumentWebpartSettings.Language.Id, WebPart.DocumentWebpartSettings.Language.Name), dlLanguage.SelectedValue},
                        {new FieldSettingsBL(WebPart.DocumentWebpartSettings.TargetGroup.Id, WebPart.DocumentWebpartSettings.TargetGroup.Name), dlTargetGroup.SelectedValue},
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
                        parameters.Add(new FieldSettingsBL(WebPart.DocumentWebpartSettings.Author.Id, WebPart.DocumentWebpartSettings.Author.Name), authorUser);

                        if (hdnStarNum.Value != null)
                        {
                            if (int.TryParse(hdnStarNum.Value.ToString(), out  rating))
                                rating = int.Parse(hdnStarNum.Value.ToString());
                        }
                    }

                    // refence file 
                    ReferenceFileWrapper referenceFile = null;
                    if (drpDocuments.SelectedItem != null)
                        referenceFile = new ReferenceFileWrapper(drpDocuments.SelectedItem.Value, drpDocuments.SelectedItem.Text, WebPart.DocumentWebpartSettings.ReferenceId.Id, WebPart.DocumentWebpartSettings.ReferenceId.Name);

                    // upload a new file
                    if (IsNew)
                    {
                        using (var repository = new SharePointRepository())
                        {
                            repository.UploadFileToLibrary(WebPart.DocumentWebpartSettings.ListId,
                                                          flUpload.FileName,
                                                          flUpload.FileContent,
                                                          parameters,
                                                          rating,
                                                          new KeyValuePair<Guid, string>(WebPart.DocumentWebpartSettings.KnowledgeArea.Id, taxKnowledgeArea.Text),
                                                          new KeyValuePair<Guid, string>(WebPart.DocumentWebpartSettings.Keywords.Id, TermHelper.getString(keywords)),
                                                          referenceFile
                                                          );
                        }
                    }
                    // modify exiting file
                    else
                    {
                        using (var repository = new SharePointRepository())
                        {
                            repository.UpdateFileInLibrary(WebPart.DocumentWebpartSettings.ListId,
                              documentID,
                              parameters,
                              rating,
                              new KeyValuePair<Guid, string>(WebPart.DocumentWebpartSettings.KnowledgeArea.Id, taxKnowledgeArea.Text),
                              new KeyValuePair<Guid, string>(WebPart.DocumentWebpartSettings.Keywords.Id, TermHelper.getString(keywords)),
                              referenceFile
                              );
                        }
                    }
                }
                if(this.IsNew)
                    ClearForm();
            }
            }


        protected void ClearForm()
        {
            txtName.Text = "";
            txtTitle.Text = "";
            dlDocumentType.SelectedIndex = 0;
            dlLanguage.SelectedIndex = 0;
            taxKnowledgeArea.Text = "";
            taxKeywords.Text = "";
            dlTargetGroup.SelectedIndex = 0;
            rtxDescription.Text = "";
            setControlVisibility();
            hdnStarNum.Value = string.Empty;
        }


        protected void setControlVisibility()
        {

            if(IsNew)
            {
                ltrlFileUpload.Visible = true;
                flUpload.Visible = true;
            }
            else
            {
                ltrlFileUpload.Visible = false;
                flUpload.Visible = false;            
            }

            // check whatever it is an english
            if (dlLanguage.SelectedValue.Contains(Constants.Lists.MtLanguage.EnglishValue))
            {
                drpDocuments.Visible = false;
                btnRefresh.Visible = false;
            }
            else
            {
                drpDocuments.Visible = true;
                btnRefresh.Visible = true;
            } 
        }

        protected void dlLanguage_OnSelectedIndexChanged(object sender, EventArgs e)
        {
            setControlVisibility();
            if (!dlLanguage.SelectedValue.Contains(Constants.Lists.MtLanguage.EnglishValue))
            {
                LoadDocumentList();
            }
        }

        protected void btnRefresh_Click(object sender, EventArgs e)
        {
            LoadDocumentList();
        }



        public static int x = 0;

        protected void LoadDocumentList()
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
                List<FileWrapper> filesToAssociate = repository.getPossibleAssosiationFiles(
                    WebPart.DocumentWebpartSettings.ListId,
                    new KeyValuePair<Guid, string>(WebPart.DocumentWebpartSettings.KnowledgeArea.Id, taxKnowledgeArea.Text),
                    new KeyValuePair<Guid, string>(WebPart.DocumentWebpartSettings.Keywords.Id, TermHelper.getString(keywords)),
                    WebPart.DocumentWebpartSettings.Title.Id,
                    WebPart.DocumentWebpartSettings.Language.Id);

                drpDocuments.DataTextField = "Name";
                drpDocuments.DataValueField = "intId";
                drpDocuments.DataSource = filesToAssociate;
                drpDocuments.DataBind();
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
