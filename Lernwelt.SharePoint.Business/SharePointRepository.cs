using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using Microsoft.Office.Server.SocialData;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Taxonomy;
using Microsoft.SharePoint.WebControls;
using Lernwelt.SharePoint.Business.Utilities;
using Microsoft.SharePoint.Publishing;

namespace Lernwelt.SharePoint.Business
{
    public class SharePointRepository : IDisposable
    {
        private SPWeb _spWeb;
        private SPSite _spSite;

        public SharePointRepository()
        {
            if (SPContext.Current == null)
                throw new InvalidOperationException("Unable to retrieve the current SharePoint context");

            _spSite = new SPSite(SPContext.Current.Web.Url);
            _spWeb = _spSite.OpenWeb();
        }

        public List<KeyValuePair<Guid, string>> GetDocumentLibraries()
        {
            return
                (from SPList list in _spWeb.Lists 
                 where list.BaseType == SPBaseType.DocumentLibrary 
                 select new KeyValuePair<Guid, string>(list.ID, list.Title)).ToList();
        }


        public List<KeyValuePair<Guid, string>> GetLists()
        {
            return
                (from SPList list in _spWeb.Lists
                 where list.BaseType == SPBaseType.GenericList
                 select new KeyValuePair<Guid, string>(list.ID, list.Title)).ToList();
        }



        public List<KeyValuePair<Guid, string>> GetCompatibleSPFieldsAndName(SPFieldType type, Guid listId)
        {
            SPList list = _spWeb.Lists[listId];
            return
                (from SPField field in list.Fields
                 where (field.Type == type) || (field.StaticName.Contains("Name"))
                 select new KeyValuePair<Guid, string>(field.Id, field.Title)).ToList();
        }


        public List<KeyValuePair<Guid, string>> GetCompatibleSPFields(SPFieldType type, Guid listId)
        {
            SPList list = _spWeb.Lists[listId];
            return 
                (from SPField field in list.Fields 
                 where field.Type == type
                 select new KeyValuePair<Guid, string>(field.Id, field.Title)).ToList();
        }

        public List<KeyValuePair<Guid, string>> GetTaxonomyField(Guid listId)
        {
            SPList list = _spWeb.Lists[listId];
            return
                (from SPField field in list.Fields
                 where field.GetType() == typeof(TaxonomyField)
                 select new KeyValuePair<Guid, string>(field.Id, field.Title)).ToList();
        }

        public List<KeyValuePair<string, string>> GetLanguages()
        {
            SPList list = GetSharePointListByName(Constants.Lists.MtLanguage.ListName);

            if (list != null)
                return (from SPListItem listItem in list.Items
                        select new KeyValuePair<string, string>(string.Format("{0};#{1}", listItem["ID"], listItem["Title"]), listItem["Title"].ToString())).ToList();
            return new List<KeyValuePair<string, string>>();
        }

        public List<KeyValuePair<string, string>> GetDocumentType()
        {
            SPList list = GetSharePointListByName(Constants.Lists.MtDocumentType.ListName);

            if (list != null)
                return (from SPListItem listItem in list.Items
                        select new KeyValuePair<string, string>(string.Format("{0};#{1}", listItem["ID"], listItem["Title"]), listItem["Title"].ToString())).ToList();
            return new List<KeyValuePair<string, string>>();
        }

        public List<KeyValuePair<string, string>> GetTargetGroup()
        {
            SPList list = GetSharePointListByName(Constants.Lists.MtTargetGroup.ListName);

            if (list != null)
                return (from SPListItem listItem in list.Items
                        select new KeyValuePair<string, string>(string.Format("{0};#{1}", listItem["ID"], listItem["Title"]), listItem["Title"].ToString())).ToList();
            return new List<KeyValuePair<string, string>>();
        }

        public TaxonomyField GetTaxonomyValues(Guid listId, Guid fieldId)
        {
            SPList list = _spWeb.Lists[listId];
            TaxonomyField returnValue = null;

            if (list != null)
            {
                returnValue = list.Fields[fieldId] as TaxonomyField;
            }

            return returnValue;
        }



        public string GetFieldInternalNameById(Guid listId, Guid fieldId)
        {
            SPList list = _spWeb.Lists[listId];
            return list.Fields[fieldId].InternalName;
        }

        public SPUser GetSPUser(string selectedAuthor)
        {
            return _spWeb.EnsureUser(selectedAuthor);
        }

        public SPUser GetSPUserByID(int id)
        {
            return _spWeb.AllUsers.GetByID(id);            
        }

        #region PageLayout_Functionalities


        public List<PageLayoutWrapper> getPageLayouts()
        {
            List<PageLayoutWrapper> result = new List<PageLayoutWrapper>();
            SPSecurity.RunWithElevatedPrivileges(delegate()
            {
                PublishingWeb publishingWeb = PublishingWeb.GetPublishingWeb(_spWeb);
                PageLayout[] layouts = publishingWeb.GetAvailablePageLayouts();
                foreach (PageLayout layout in layouts)
                {
                    if(!Constants.NonSupportedPageLayout.layouts.Contains(layout.Title.ToLower()))
                        result.Add(new PageLayoutWrapper(layout.Name, layout.Title));
                }                  
            });
            return result;
        }

        public PageLayout getPageLayoutByName(string _name)
        {
            PageLayout result = null;
            SPSecurity.RunWithElevatedPrivileges(delegate()
            {
                PublishingWeb publishingWeb = PublishingWeb.GetPublishingWeb(_spWeb);
                PageLayout[] layouts = publishingWeb.GetAvailablePageLayouts();
                foreach (PageLayout layout in layouts)
                {
                    if (layout.Name.Equals(_name))
                        result = layout;
                }
            });
            return result;
        }

        public void setPageLayoutByName(string _name, SPListItem _item)
        {
            PublishingWeb publishingWeb = PublishingWeb.GetPublishingWeb(_spWeb);
            PublishingPageCollection page = publishingWeb.GetPublishingPages();
            int i = 0;
            i++;
        }

        #endregion

        #region WIKI_Functionalities


        /// <summary>
        /// Create a new WIKI item
        /// </summary>
        /// <param name="faqlistId"></param>
        /// <param name="parameters"></param>
        /// <param name="rating"></param>
        /// <param name="knowledgeTax"></param>
        /// <param name="keywordTax"></param>
        public void CreateWIKI(Guid faqlistId, Dictionary<FieldSettingsBL, object> parameters, int rating,
                              KeyValuePair<Guid, string> knowledgeTax, KeyValuePair<Guid, string> keywordTax, ReferenceWIKIWrapper referenceFAQ, string fileType, bool IsStartPage, string newName, Guid pageLinkID)
        {
            bool allowUnsafeUpdateOld = _spWeb.AllowUnsafeUpdates;
            try
            {
                _spWeb.AllowUnsafeUpdates = true;

                var list =  (SPDocumentLibrary)_spWeb.Lists[faqlistId];

                string pageUrl = list.RootFolder.ServerRelativeUrl + "/" + newName + Constants.General.AspxExtention;

                // SPFile newFile = list.RootFolder.Files.Add(pageUrl, SPTemplateFileType.WikiPage);
                PublishingWeb publishingWeb = PublishingWeb.GetPublishingWeb(_spWeb);
                PublishingPageCollection pages = publishingWeb.GetPublishingPages();
                PublishingPage newPage = pages.Add(newName + Constants.General.AspxExtention, getPageLayoutByName(fileType));
               

                newPage.Update();
                // Check in
                //newPage.CheckIn("");

                SPListItem listItem = newPage.ListItem;

                foreach (var parameter in parameters)
                {
                    if (parameter.Key.Name.Equals("BaseName"))
                        listItem[parameter.Key.Name] = parameter.Value;
                    else
                        listItem[parameter.Key.Id] = parameter.Value;
                }

                // write In PageLink URL

                listItem[pageLinkID] = _spWeb.ServerRelativeUrl + "/" + newPage.Url + "?Page=" + listItem.ID;
                //listItem[pageLinkID] = "/" + newPage.Url + "?Page=" + listItem.ID;
                listItem.Update();

                //this.setPageLayoutByName(fileType, listItem);

                // now we have the url
                RateTheContent((string)listItem[SPBuiltInFieldId.EncodedAbsUrl], rating);
                listItem.Update();

                //// set the noOfRatings
                //int noofrating = GetNoOfRating((string)listItem[SPBuiltInFieldId.EncodedAbsUrl]);
                //listItem[noofratingcolumnID] = noofrating;
                //listItem.Update();

                // Now the tax fields
                var knowledgeField = list.Fields[knowledgeTax.Key] as TaxonomyField;
                var keywordField = list.Fields[keywordTax.Key] as TaxonomyField;

                var knowledgeValues = new TaxonomyFieldValue(String.Empty, knowledgeField);
                knowledgeValues.PopulateFromLabelGuidPair(knowledgeTax.Value);
                //listItem[knowledgeTax.Key] = knowledgeValues;
                knowledgeField.SetFieldValue(listItem, knowledgeValues);

                var keywordsValues = new TaxonomyFieldValueCollection(string.Empty, keywordField);
                keywordsValues.PopulateFromLabelGuidPairs(keywordTax.Value);
                //listItem[keywordTax.Key] = keywordsValues;
                keywordField.SetFieldValue(listItem, keywordsValues);

                listItem.Update();

                // and the lookup
                if ((referenceFAQ != null) && (referenceFAQ.intId != null))
                {
                    listItem[referenceFAQ.columnID] = referenceFAQ.Lookup;
                }

                listItem.Update();
            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
#if DEBUG
                throw ex;
#endif
            }
            finally
            {
                _spWeb.AllowUnsafeUpdates = allowUnsafeUpdateOld;
            }
        }


        /// <summary>
        /// Returns the all parameters for a given WIKI element
        /// </summary>
        /// <param name="documentLibraryId"></param>
        /// <param name="itemID"></param>
        /// <returns></returns>
        public WikiParameter GetWIKIParametersForId(Guid wikiListId, int wikiID, Dictionary<FieldSettingsBL, object> parameters, KeyValuePair<Guid, string> knowledgeTax, KeyValuePair<Guid, string> keywordTax)
        {
            WikiParameter paramsToReturn = new WikiParameter();

            try
            {
                var list = _spWeb.Lists[wikiListId];
                SPListItem wikiItem = list.GetItemById(wikiID);
                Dictionary<FieldSettingsBL, object> newParams = new Dictionary<FieldSettingsBL, object>();

                // get general parameters
                foreach (var parameter in parameters)
                {
                    newParams.Add(new FieldSettingsBL(parameter.Key.Id, parameter.Key.Name), wikiItem[parameter.Key.Id]);
                }

                paramsToReturn.parameters = newParams;

                // rating
                paramsToReturn.rating = GetRating((string)wikiItem[SPBuiltInFieldId.EncodedAbsUrl]);

                paramsToReturn.keywordTaxString = wikiItem[keywordTax.Key] == null ? string.Empty : wikiItem[keywordTax.Key].ToString();

                paramsToReturn.knowledgeTaxString = wikiItem[knowledgeTax.Key] == null ? string.Empty : wikiItem[knowledgeTax.Key].ToString();

                //pubishing page layout
                PublishingWeb publishingWeb = PublishingWeb.GetPublishingWeb(_spWeb);
                PublishingPage page = PublishingPage.GetPublishingPage(wikiItem);
                paramsToReturn.layoutName = page.Layout.Title;

            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
#if DEBUG
                throw ex;
#endif
            }
            return paramsToReturn;
        }

        /// <summary>
        /// Updating an exiting WIKI 
        /// </summary>
        /// <param name="documentLibraryId"></param>
        /// <param name="fileID"></param>
        /// <param name="parameters"></param>
        /// <param name="rating"></param>
        /// <param name="knowledgeTax"></param>
        /// <param name="keywordTax"></param>
        public void UpdateWIKI(Guid wikiListId, int wikiID, Dictionary<FieldSettingsBL, object> parameters, int rating,
                                KeyValuePair<Guid, string> knowledgeTax, KeyValuePair<Guid, string> keywordTax, ReferenceWIKIWrapper referenceFAQ)
        {
            bool allowUnsafeUpdateOld = _spWeb.AllowUnsafeUpdates;
            try
            {
                _spWeb.AllowUnsafeUpdates = true;

                var list = _spWeb.Lists[wikiListId];
                SPListItem listItem = list.GetItemById(wikiID);

                foreach (var parameter in parameters)
                {
                    if (parameter.Key.Name.Equals("BaseName"))
                        listItem[parameter.Key.Name] = parameter.Value;
                    else
                        listItem[parameter.Key.Id] = parameter.Value;
                }

                listItem.Update();
                // now we have the url
                RateTheContent((string)listItem[SPBuiltInFieldId.EncodedAbsUrl], rating);
                listItem.Update();

                //// set the noOfRatings
                //int noofrating = GetNoOfRating((string)listItem[SPBuiltInFieldId.EncodedAbsUrl]);
                //listItem[noofratingcolumnID] = noofrating;
                //listItem.Update();

                // Now the tax fields
                var knowledgeField = list.Fields[knowledgeTax.Key] as TaxonomyField;
                var keywordField = list.Fields[keywordTax.Key] as TaxonomyField;

                var knowledgeValues = new TaxonomyFieldValue(String.Empty, knowledgeField);
                knowledgeValues.PopulateFromLabelGuidPair(knowledgeTax.Value);
                //file.Item[knowledgeTax.Key] =  knowledgeValues;
                knowledgeField.SetFieldValue(listItem, knowledgeValues);

                var keywordsValues = new TaxonomyFieldValueCollection(string.Empty, keywordField);
                keywordsValues.PopulateFromLabelGuidPairs(keywordTax.Value);
                //file.Item[keywordTax.Key] = keywordsValues;
                keywordField.SetFieldValue(listItem, keywordsValues);

                listItem.Update();

                // and the lookup
                if ((referenceFAQ != null) && (referenceFAQ.intId != null))
                {
                    listItem[referenceFAQ.columnID] = referenceFAQ.Lookup;
                }

                listItem.Update();
                //list.Update();

            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
#if DEBUG
                throw ex;
#endif
            }
            finally
            {
                _spWeb.AllowUnsafeUpdates = allowUnsafeUpdateOld;
            }
        }


        public List<WikiWrapper> getPossibleAssosiationWIKI(Guid wikiListId, KeyValuePair<Guid, string> knowledgeAreaInfo, KeyValuePair<Guid, string> keywordsInfo, Guid titleColumnID, Guid languageColumnID)
        {

            List<WikiWrapper> result = new List<WikiWrapper>();

            try
            {
                SPList list = _spWeb.Lists[wikiListId];

                SPQuery query = new SPQuery();

                string keywordsQuery = TermHelper.GetCAMLString(keywordsInfo);
                string knowledgeQuery = new TermHelper(TermHelper.getNameString(knowledgeAreaInfo.Value), knowledgeAreaInfo.Key).getCAMLFragment(knowledgeAreaInfo.Key);

                string camlQuery = string.Empty;

                if (keywordsQuery.Equals(string.Empty) && knowledgeQuery.Equals(string.Empty))
                    camlQuery = string.Format("<Where><Eq><FieldRef ID='{0}'/><Value Type='Text'>{1}</Value></Eq></Where>", languageColumnID, Constants.Lists.MtLanguage.EnglishValue);
                else if (keywordsQuery.Equals(string.Empty) && (!knowledgeQuery.Equals(string.Empty)))
                    camlQuery = string.Format("<Where><And>{0}<Eq><FieldRef ID='{1}'/><Value Type='Text'>{2}</Value></Eq></And></Where>", knowledgeQuery, languageColumnID, Constants.Lists.MtLanguage.EnglishValue);
                else if ((!keywordsQuery.Equals(string.Empty)) && knowledgeQuery.Equals(string.Empty))
                    camlQuery = string.Format("<Where><And>{0}<Eq><FieldRef ID='{1}'/><Value Type='Text'>{2}</Value></Eq></And></Where>", keywordsQuery, languageColumnID, Constants.Lists.MtLanguage.EnglishValue);
                else
                    camlQuery = string.Format("<Where><And>{0}<And>{1}<Eq><FieldRef ID='{2}'/><Value Type='Text'>{3}</Value></Eq></And></And></Where>", knowledgeQuery, keywordsQuery, languageColumnID, Constants.Lists.MtLanguage.EnglishValue);

                query.Query = camlQuery;

                query.ViewFields = string.Format("<FieldRef ID='{0}'/>", titleColumnID);

                SPListItemCollection listItemCollection = list.GetItems(query);

                foreach (SPListItem item in listItemCollection)
                {
                    result.Add(new WikiWrapper(item.UniqueId, item[titleColumnID].ToString(), item.ID));
                }

            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
#if DEBUG
                throw ex;
#endif
            }

            return result;
        }

        #endregion

        #region FAQ_Functionalities

        /// <summary>
        /// Returns the all parameters for a given FAQ element
        /// </summary>
        /// <param name="documentLibraryId"></param>
        /// <param name="itemID"></param>
        /// <returns></returns>
        public FAQParameter GetFaqParametersForId(Guid faqListId, int faqID, Dictionary<FieldSettingsBL, object> parameters, KeyValuePair<Guid, string> knowledgeTax, KeyValuePair<Guid, string> keywordTax)
        {
            FAQParameter paramsToReturn = new FAQParameter();

            try
            {
                var list = _spWeb.Lists[faqListId];
                SPListItem faqItem = list.GetItemById(faqID);
                Dictionary<FieldSettingsBL, object> newParams = new Dictionary<FieldSettingsBL, object>();


                // get general parameters
                foreach (var parameter in parameters)
                {
                    newParams.Add(new FieldSettingsBL(parameter.Key.Id, parameter.Key.Name), faqItem[parameter.Key.Id]);
                }

                paramsToReturn.parameters = newParams;

                // rating
                paramsToReturn.rating = GetRating((string)faqItem[SPBuiltInFieldId.EncodedAbsUrl]);

                paramsToReturn.keywordTaxString = faqItem[keywordTax.Key] == null ? string.Empty : faqItem[keywordTax.Key].ToString();

                paramsToReturn.knowledgeTaxString = faqItem[knowledgeTax.Key] == null ? string.Empty : faqItem[knowledgeTax.Key].ToString();
            }
            catch (ArgumentException argex)
            { 
                //... error handling
                Logger.LogError(argex);
#if DEBUG
                throw argex;
#endif

            }
        return paramsToReturn;
        }

        /// <summary>
        /// Create a new FAQ item
        /// </summary>
        /// <param name="faqlistId"></param>
        /// <param name="parameters"></param>
        /// <param name="rating"></param>
        /// <param name="knowledgeTax"></param>
        /// <param name="keywordTax"></param>
        public void CreateFaq(Guid faqlistId, Dictionary<FieldSettingsBL, object> parameters, int rating,
                              KeyValuePair<Guid, string> knowledgeTax, KeyValuePair<Guid, string> keywordTax, ReferenceFAQWrapper referenceFAQ)
        {
            bool allowUnsafeUpdateOld = _spWeb.AllowUnsafeUpdates;
            try
            {

                _spWeb.AllowUnsafeUpdates = true;
                var list = _spWeb.Lists[faqlistId];

                var listItem = list.AddItem();

                foreach (var parameter in parameters)
                {
                    listItem[parameter.Key.Id] = parameter.Value;
                }

                listItem.Update();

                // now we have the url
                RateTheContent((string)listItem[SPBuiltInFieldId.EncodedAbsUrl], rating);
                listItem.Update();

                //// set the noOfRatings
                //int noofrating = GetNoOfRating((string)listItem[SPBuiltInFieldId.EncodedAbsUrl]);
                //listItem[noofratingcolumnID] = noofrating;
                //listItem.Update();

                // Now the tax fields
                var knowledgeField = list.Fields[knowledgeTax.Key] as TaxonomyField;
                var keywordField = list.Fields[keywordTax.Key] as TaxonomyField;

                var knowledgeValues = new TaxonomyFieldValue(String.Empty, knowledgeField);
                knowledgeValues.PopulateFromLabelGuidPair(knowledgeTax.Value);
                //listItem[knowledgeTax.Key] = knowledgeValues;
                knowledgeField.SetFieldValue(listItem, knowledgeValues);

                var keywordsValues = new TaxonomyFieldValueCollection(string.Empty, keywordField);
                keywordsValues.PopulateFromLabelGuidPairs(keywordTax.Value);
                //listItem[keywordTax.Key] = keywordsValues;
                keywordField.SetFieldValue(listItem, keywordsValues);

                listItem.Update();

                // and the lookup
                if ((referenceFAQ != null) && (referenceFAQ.intId != null))
                {
                    listItem[referenceFAQ.columnID] = referenceFAQ.Lookup;
                }

                listItem.Update();
            }
            catch(Exception ex) {
                //... error handling
                Logger.LogError(ex);
#if DEBUG
                throw ex;
#endif            
            }
            finally 
            {
                _spWeb.AllowUnsafeUpdates = allowUnsafeUpdateOld;
            }
        }


        /// <summary>
        /// Updating an exiting file in the document library
        /// </summary>
        /// <param name="documentLibraryId"></param>
        /// <param name="fileID"></param>
        /// <param name="parameters"></param>
        /// <param name="rating"></param>
        /// <param name="knowledgeTax"></param>
        /// <param name="keywordTax"></param>
        public void UpdateFAQ(Guid faqListId, int faqID, Dictionary<FieldSettingsBL, object> parameters, int rating,
                                KeyValuePair<Guid, string> knowledgeTax, KeyValuePair<Guid, string> keywordTax, ReferenceFAQWrapper referenceFAQ)
        {
            bool oldUnsafeUpdatValue = _spWeb.AllowUnsafeUpdates;

            try
            {
                _spWeb.AllowUnsafeUpdates = true;

                var list = _spWeb.Lists[faqListId];
                SPListItem listItem = list.GetItemById(faqID);


                //file.Item["Name"] = "xxx";

                //file.Update();
                
                foreach (var parameter in parameters)
                {
                    if (parameter.Key.Name.Equals("BaseName"))
                        listItem[parameter.Key.Name] = parameter.Value;
                    else
                        listItem[parameter.Key.Id] = parameter.Value;
                }

                listItem.Update();
                // now we have the url
                RateTheContent((string)listItem[SPBuiltInFieldId.EncodedAbsUrl], rating);
                listItem.Update();

                //// set the noOfRatings
                //int noofrating = GetNoOfRating((string)listItem[SPBuiltInFieldId.EncodedAbsUrl]);
                //listItem[noofratingcolumnID] = noofrating;
                //listItem.Update();

                // Now the tax fields
                var knowledgeField = list.Fields[knowledgeTax.Key] as TaxonomyField;
                var keywordField = list.Fields[keywordTax.Key] as TaxonomyField;

                var knowledgeValues = new TaxonomyFieldValue(String.Empty, knowledgeField);
                knowledgeValues.PopulateFromLabelGuidPair(knowledgeTax.Value);
                //file.Item[knowledgeTax.Key] =  knowledgeValues;
                knowledgeField.SetFieldValue(listItem, knowledgeValues);

                var keywordsValues = new TaxonomyFieldValueCollection(string.Empty, keywordField);
                keywordsValues.PopulateFromLabelGuidPairs(keywordTax.Value);
                //file.Item[keywordTax.Key] = keywordsValues;
                keywordField.SetFieldValue(listItem, keywordsValues);

                listItem.Update();

                // and the lookup
                if ((referenceFAQ != null) && (referenceFAQ.intId != null))
                {
                    listItem[referenceFAQ.columnID] = referenceFAQ.Lookup;
                }

                listItem.Update();
                //list.Update();

            }
            catch (Exception ex)
            {
                //... error handling
                Logger.LogError(ex);
#if DEBUG
                throw ex;
#endif            
            }
            finally
            {
                _spWeb.AllowUnsafeUpdates = oldUnsafeUpdatValue;
            }
        }


        /// <summary>
        /// Get the possible associated FAQs for the given input parameters
        /// </summary>
        /// <param name="listId"></param>
        /// <param name="knowledgeAreaInfo"></param>
        /// <param name="keywordsInfo"></param>
        /// <param name="titleColumnID"></param>
        /// <param name="languageColumnID"></param>
        /// <returns></returns>
        public List<FAQWrapper> getPossibleAssosiationFAQ(Guid listId, KeyValuePair<Guid, string> knowledgeAreaInfo, KeyValuePair<Guid, string> keywordsInfo, Guid titleColumnID, Guid languageColumnID)
        {
            List<FAQWrapper> result = new List<FAQWrapper>();

            try
            {
                SPList list = _spWeb.Lists[listId];

                SPQuery query = new SPQuery();

                string keywordsQuery = TermHelper.GetCAMLString(keywordsInfo);
                string knowledgeQuery = new TermHelper(TermHelper.getNameString(knowledgeAreaInfo.Value), knowledgeAreaInfo.Key).getCAMLFragment(knowledgeAreaInfo.Key);

                string camlQuery = string.Empty;

                if (keywordsQuery.Equals(string.Empty) && knowledgeQuery.Equals(string.Empty))
                    camlQuery = string.Format("<Where><Eq><FieldRef ID='{0}'/><Value Type='Text'>{1}</Value></Eq></Where>", languageColumnID, Constants.Lists.MtLanguage.EnglishValue);
                else if (keywordsQuery.Equals(string.Empty) && (!knowledgeQuery.Equals(string.Empty)))
                    camlQuery = string.Format("<Where><And>{0}<Eq><FieldRef ID='{1}'/><Value Type='Text'>{2}</Value></Eq></And></Where>", knowledgeQuery, languageColumnID, Constants.Lists.MtLanguage.EnglishValue);
                else if ((!keywordsQuery.Equals(string.Empty)) && knowledgeQuery.Equals(string.Empty))
                    camlQuery = string.Format("<Where><And>{0}<Eq><FieldRef ID='{1}'/><Value Type='Text'>{2}</Value></Eq></And></Where>", keywordsQuery, languageColumnID, Constants.Lists.MtLanguage.EnglishValue);
                else
                    camlQuery = string.Format("<Where><And>{0}<And>{1}<Eq><FieldRef ID='{2}'/><Value Type='Text'>{3}</Value></Eq></And></And></Where>", knowledgeQuery, keywordsQuery, languageColumnID, Constants.Lists.MtLanguage.EnglishValue);

                query.Query = camlQuery;

                query.ViewFields = string.Format("<FieldRef ID='{0}'/>", titleColumnID);

                SPListItemCollection listItemCollection = list.GetItems(query);

                foreach (SPListItem item in listItemCollection)
                {
                    result.Add(new FAQWrapper(item.UniqueId, item[titleColumnID].ToString(), item.ID));
                }
            }
            catch (Exception ex)
            {
                //... error handling
                Logger.LogError(ex);
#if DEBUG
                throw ex;
#endif            
            }

            return result;
        }


        #endregion

        #region WIKI_Rating_Functionalities

        /// <summary>
        /// Get the wiki rating information for the certain input combination
        /// </summary>
        /// <param name="faqListId"></param>
        /// <param name="?"></param>
        /// <returns></returns>
        public List<WikiRatingWrapper> GetWikiRatings(Guid listId, Guid linkColumnID, KeyValuePair<Guid, string> keywordsInfo, int numOfElements)
        {
            List<WikiRatingWrapper> result = new List<WikiRatingWrapper>();

            try
            {
                SPList list = _spWeb.Lists[listId];

                SPQuery query = new SPQuery();

                string keywordsQuery = TermHelper.GetCAMLString(keywordsInfo);
                string camlQuery = string.Empty;

                if (keywordsQuery.Equals(string.Empty))
                    camlQuery = string.Format("<Where></Where>");
                else
                    camlQuery = string.Format("<Where>{0}</Where>", keywordsQuery);

                query.Query = camlQuery;

                query.ViewFields = string.Format("<FieldRef ID='{0}'/><FieldRef ID='{1}'/>", linkColumnID, SPBuiltInFieldId.EncodedAbsUrl);

                query.RowLimit = (uint)numOfElements;

                SPListItemCollection listItemCollection = list.GetItems(query);

                foreach (SPListItem item in listItemCollection)
                {
                    // rating
                    Guid url = SPBuiltInFieldId.EncodedAbsUrl;
                    string link = (string)item[SPBuiltInFieldId.EncodedAbsUrl];
                    int rating = GetRating(link);
                    result.Add(new WikiRatingWrapper(item.UniqueId, (string)item[linkColumnID], item.ID, rating, link));
                }
            }
            catch (Exception ex)
            {
                //... error handling
                Logger.LogError(ex);
#if DEBUG
                throw ex;
#endif
            }

            return result;
        }



        #endregion


        #region Document_Functionalities

        /// <summary>
        /// Returns the all parameters for a given document element
        /// </summary>
        /// <param name="documentLibraryId"></param>
        /// <param name="itemID"></param>
        /// <returns></returns>
        public FileParameter GetFileParametersForId(Guid documentLibraryId, int fileID, Dictionary<FieldSettingsBL, object> parameters, KeyValuePair<Guid, string> knowledgeTax, KeyValuePair<Guid, string> keywordTax)
        {
            FileParameter paramsToReturn = new FileParameter();

            var list = (SPDocumentLibrary)_spWeb.Lists[documentLibraryId];
            try
            {
                SPFile file = list.GetItemById(fileID).File;
                if (file != null)
                {
                    Dictionary<FieldSettingsBL, object> newParams = new Dictionary<FieldSettingsBL, object>();


                    // get general parameters
                    foreach (var parameter in parameters)
                    {
                        newParams.Add(new FieldSettingsBL(parameter.Key.Id, parameter.Key.Name), file.Item[parameter.Key.Id]);
                    }

                    paramsToReturn.parameters = newParams;

                    // rating
                    paramsToReturn.rating = GetRating((string)file.Item[SPBuiltInFieldId.EncodedAbsUrl]);

                    paramsToReturn.keywordTaxString = file.Item[keywordTax.Key] == null ? string.Empty : file.Item[keywordTax.Key].ToString();

                    paramsToReturn.knowledgeTaxString = file.Item[knowledgeTax.Key] == null ? string.Empty : file.Item[knowledgeTax.Key].ToString();
                }
            }
            catch (ArgumentException argex)
            { 
                //... error handling
                Logger.LogError(argex);
#if DEBUG
                throw argex;
#endif            
            }
            return paramsToReturn;
        }


        /// <summary>
        /// Uploading a new document to the document library
        /// </summary>
        /// <param name="documentLibraryId"></param>
        /// <param name="fileName"></param>
        /// <param name="fileContent"></param>
        /// <param name="parameters"></param>
        /// <param name="rating"></param>
        /// <param name="knowledgeTax"></param>
        /// <param name="keywordTax"></param>
        public void UploadFileToLibrary(Guid documentLibraryId, string fileName, Stream fileContent, Dictionary<FieldSettingsBL, object> parameters, int rating,
                                        KeyValuePair<Guid, string> knowledgeTax, KeyValuePair<Guid, string> keywordTax, ReferenceFileWrapper referenceFile)
        {
            bool oldUnsafeUpdatValue = _spWeb.AllowUnsafeUpdates;

            try
            {
                _spWeb.AllowUnsafeUpdates = true;

                var list = (SPDocumentLibrary)_spWeb.Lists[documentLibraryId];

                string destUrl = list.RootFolder.Url + "/" + fileName;

                SPFile file = list.RootFolder.Files.Add(destUrl, fileContent, true);

                Guid fileID = file.UniqueId;
                //file.Item["Name"] = "xxx";

                //file.Update();

                foreach (var parameter in parameters)
                {
                    if (parameter.Key.Name.Equals("BaseName"))
                        file.Item[parameter.Key.Name] = parameter.Value;
                    else
                        file.Item[parameter.Key.Id] = parameter.Value;
                }

                file.Update();

                file.Item.Update();

                //file = _spWeb.GetFile(fileID);
                // now we have the url
                RateTheContent((string)file.Item[SPBuiltInFieldId.EncodedAbsUrl], rating);
                file.Item.Update();

                //// set the noOfRatings
                //int noofrating = GetNoOfRating((string)file.Item[SPBuiltInFieldId.EncodedAbsUrl]);
                //file.Item[noofratingcolumnID] = noofrating;
                //file.Item.Update();

                // Now the tax fields
                var knowledgeField = list.Fields[knowledgeTax.Key] as TaxonomyField;
                var keywordField = list.Fields[keywordTax.Key] as TaxonomyField;

                var knowledgeValues = new TaxonomyFieldValue(String.Empty, knowledgeField);
                knowledgeValues.PopulateFromLabelGuidPair(knowledgeTax.Value);
                //file.Item[knowledgeTax.Key] = knowledgeValues;
                knowledgeField.SetFieldValue(file.Item, knowledgeValues);
                file.Item.Update();

                var keywordsValues = new TaxonomyFieldValueCollection(string.Empty, keywordField);
                keywordsValues.PopulateFromLabelGuidPairs(keywordTax.Value);
                //file.Item[keywordTax.Key] = keywordsValues;
                keywordField.SetFieldValue(file.Item, keywordsValues);

                file.Item.Update();

                // and the lookup
                if ((referenceFile!= null) && (referenceFile.intId != null))
                {
                    file.Item[referenceFile.columnID] = referenceFile.Lookup;
                }

                file.Item.Update();

                //list.Update();

            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
#if DEBUG
                throw ex;
#endif
            }
            finally
            {
                _spWeb.AllowUnsafeUpdates = oldUnsafeUpdatValue;
            }
        }

        /// <summary>
        /// Updating an exiting file in the document library
        /// </summary>
        /// <param name="documentLibraryId"></param>
        /// <param name="fileID"></param>
        /// <param name="parameters"></param>
        /// <param name="rating"></param>
        /// <param name="knowledgeTax"></param>
        /// <param name="keywordTax"></param>
        public void UpdateFileInLibrary(Guid documentLibraryId, int fileID, Dictionary<FieldSettingsBL, object> parameters, int rating,
                                KeyValuePair<Guid, string> knowledgeTax, KeyValuePair<Guid, string> keywordTax, ReferenceFileWrapper referenceFile)
        {
            bool oldUnsafeUpdatValue = _spWeb.AllowUnsafeUpdates;

            try
            {
                _spWeb.AllowUnsafeUpdates = true;

                var list = (SPDocumentLibrary)_spWeb.Lists[documentLibraryId];
                SPFile file = list.GetItemById(fileID).File;


                //file.Item["Name"] = "xxx";

                //file.Update();

                foreach (var parameter in parameters)
                {
                    if (parameter.Key.Name.Equals("BaseName"))
                        file.Item[parameter.Key.Name] = parameter.Value;
                    else
                        file.Item[parameter.Key.Id] = parameter.Value;
                }

                file.Update();
                // now we have the url
                RateTheContent((string)file.Item[SPBuiltInFieldId.EncodedAbsUrl], rating);
                file.Item.Update();

                //// set the noOfRatings
                //int noofrating = GetNoOfRating((string)file.Item[SPBuiltInFieldId.EncodedAbsUrl]);
                //file.Item[noofratingcolumnID] = noofrating;
                //file.Item.Update();

                // Now the tax fields
                var knowledgeField = list.Fields[knowledgeTax.Key] as TaxonomyField;
                var keywordField = list.Fields[keywordTax.Key] as TaxonomyField;

                var knowledgeValues = new TaxonomyFieldValue(String.Empty, knowledgeField);
                knowledgeValues.PopulateFromLabelGuidPair(knowledgeTax.Value);
                //file.Item[knowledgeTax.Key] =  knowledgeValues;
                knowledgeField.SetFieldValue(file.Item, knowledgeValues);

                var keywordsValues = new TaxonomyFieldValueCollection(string.Empty, keywordField);
                keywordsValues.PopulateFromLabelGuidPairs(keywordTax.Value);
                //file.Item[keywordTax.Key] = keywordsValues;
                keywordField.SetFieldValue(file.Item, keywordsValues);

                file.Item.Update();

                // and the lookup
                if ((referenceFile!= null) && (referenceFile.intId != null))
                {
                    file.Item[referenceFile.columnID] = referenceFile.Lookup;
                }

                file.Item.Update();


                //list.Update();

            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
#if DEBUG
                throw ex;
#endif
            }
            finally
            {
                _spWeb.AllowUnsafeUpdates = oldUnsafeUpdatValue;
            }
        }

        /// <summary>
        /// Get the possible associated files for the given input parameters
        /// </summary>
        /// <param name="listId"></param>
        /// <param name="knowledgeAreaInfo"></param>
        /// <param name="keywordsInfo"></param>
        /// <param name="titleColumnID"></param>
        /// <param name="languageColumnID"></param>
        /// <returns></returns>
        public List<FileWrapper> getPossibleAssosiationFiles(Guid listId, KeyValuePair<Guid, string> knowledgeAreaInfo, KeyValuePair<Guid, string> keywordsInfo, Guid titleColumnID, Guid languageColumnID)
        {
            List<FileWrapper> result = new List<FileWrapper>();

            try
            {
                SPList list = _spWeb.Lists[listId];

                SPQuery query = new SPQuery();

                string keywordsQuery = TermHelper.GetCAMLString(keywordsInfo);
                string knowledgeQuery = new TermHelper(TermHelper.getNameString(knowledgeAreaInfo.Value), knowledgeAreaInfo.Key).getCAMLFragment(knowledgeAreaInfo.Key);

                string camlQuery = string.Empty;

                if (keywordsQuery.Equals(string.Empty) && knowledgeQuery.Equals(string.Empty))
                    camlQuery = string.Format("<Where><Eq><FieldRef ID='{0}'/><Value Type='Text'>{1}</Value></Eq></Where>", languageColumnID, Constants.Lists.MtLanguage.EnglishValue);
                else if (keywordsQuery.Equals(string.Empty) && (!knowledgeQuery.Equals(string.Empty)))
                    camlQuery = string.Format("<Where><And>{0}<Eq><FieldRef ID='{1}'/><Value Type='Text'>{2}</Value></Eq></And></Where>", knowledgeQuery, languageColumnID, Constants.Lists.MtLanguage.EnglishValue);
                else if ((!keywordsQuery.Equals(string.Empty)) && knowledgeQuery.Equals(string.Empty))
                    camlQuery = string.Format("<Where><And>{0}<Eq><FieldRef ID='{1}'/><Value Type='Text'>{2}</Value></Eq></And></Where>", keywordsQuery, languageColumnID, Constants.Lists.MtLanguage.EnglishValue);
                else
                    camlQuery = string.Format("<Where><And>{0}<And>{1}<Eq><FieldRef ID='{2}'/><Value Type='Text'>{3}</Value></Eq></And></And></Where>", knowledgeQuery, keywordsQuery, languageColumnID, Constants.Lists.MtLanguage.EnglishValue);

                query.Query = camlQuery;

                query.ViewFields = string.Format("<FieldRef ID='{0}'/>", titleColumnID);

                SPListItemCollection listItemCollection = list.GetItems(query);

                foreach (SPListItem item in listItemCollection)
                {
                    result.Add(new FileWrapper(item.UniqueId, item[titleColumnID] == null ? string.Empty : item[titleColumnID].ToString(), item.ID));
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
#if DEBUG
                throw ex;
#endif                
            }
            return result;
        }


        #endregion

        #region Term_and_rating_stuff

        /// <summary>
        /// get all posibly terms for a given field
        /// </summary>
        /// <param name="listId"></param>
        /// <param name="fieldId"></param>
        /// <returns></returns>
        public List<string> GetTerms(Guid listId, Guid fieldId)
        {
            List<string> result = new List<string>();
            SPSecurity.RunWithElevatedPrivileges(delegate()
            {
                SPList list = _spWeb.Lists[listId];
                var field = (TaxonomyField)list.Fields[fieldId];

                TaxonomySession session = new TaxonomySession(_spSite);
                TermStore termStore = session.TermStores[field.SspId];
                TermSet termSet = termStore.GetTermSet(field.TermSetId);
                result = termSet.GetAllTerms().Select(term => term.Name).ToList();
            });
            return result;
        }

        /// <summary>
        /// get all posibly terms for a given field with GUID
        /// </summary>
        /// <param name="listId"></param>
        /// <param name="fieldId"></param>
        /// <returns></returns>
        public List<TermHelper> GetTermsWithID(Guid listId, Guid fieldId)
        {
            List<TermHelper> result = new List<TermHelper>();
            SPSecurity.RunWithElevatedPrivileges(delegate()
            {
                SPList list = _spWeb.Lists[listId];
                var field = (TaxonomyField)list.Fields[fieldId];

                TaxonomySession session = new TaxonomySession(_spSite);
                TermStore termStore = session.TermStores[field.SspId];
                TermSet termSet = termStore.GetTermSet(field.TermSetId);

                foreach (Term term in termSet.GetAllTerms())
                {
                    result.Add(new TermHelper(term.Name, term.Id));
                }
            });

            return result;
        }


        /// <summary>
        /// get all posibly terms for a given field
        /// </summary>
        /// <param name="listId"></param>
        /// <param name="fieldId"></param>
        /// <returns></returns>
        public List<string> GetTermsForNames(Guid listId, Guid fieldId, List<string> names)
        {
            List<string> ret = new List<string>();

            SPSecurity.RunWithElevatedPrivileges(delegate()
            {

                SPList list = _spWeb.Lists[listId];
                var field = (TaxonomyField)list.Fields[fieldId];

                TaxonomySession session = new TaxonomySession(_spSite);
                TermStore termStore = session.TermStores[field.SspId];
                TermSet termSet = termStore.GetTermSet(field.TermSetId);

                foreach (string name in names)
                {
                    foreach (Term term in termSet.Terms)
                    {
                        string namex = term.Name;
                    }
                    //if (termSet.Terms[name] != null)
                    //   ret.Add(string.Format("{0}|{1}", name, termSet.Terms[name].Id.ToString()));
                }

            });
            return ret;
        }

        /// <summary>
        /// get the string list separated by ;
        /// </summary>
        /// <param name="listId"></param>
        /// <param name="fieldId"></param>
        /// <param name="names"></param>
        /// <returns></returns>
        public string GetTermsStringForNames(Guid listId, Guid fieldId, List<string> names)
        {
            List<string> res = GetTermsForNames(listId, fieldId, names);
            string resString = string.Empty;
            if (res.Count < 1)
                resString = string.Empty;
            else if (res.Count == 1)
                resString = res.ElementAt(1);
            else {
                foreach (string elem in res)
                {
                    resString += elem +";";
                }
            }
            return resString;
        }


        /// <summary>
        /// Rate the content on the given url
        /// </summary>
        /// <param name="url"></param>
        /// <param name="rateValue"></param>
       private void RateTheContent(string url, int rateValue)
       {
           SPSecurity.RunWithElevatedPrivileges(delegate()
           {
               if (rateValue > 0 && rateValue < 6)
               {
                   var itemUrl = new Uri(url);
                   var feedbackData = new FeedbackData { UserTitle = "" };
                   SPServiceContext context = SPServiceContext.GetContext(_spSite);
                   var socialRatingManager = new SocialRatingManager(context);
                   socialRatingManager.SetRating(itemUrl, rateValue, feedbackData);
               }
           });
        }

        /// <summary>
        /// Get the rating of a given url
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
       private int GetRating(string url)
       {
            int result = 0;
            SPSecurity.RunWithElevatedPrivileges(delegate()
            {
                var itemUrl = new Uri(url);
                var feedbackData = new FeedbackData { UserTitle = "" };
                SPServiceContext context = SPServiceContext.GetContext(_spSite);
                var socialRatingManager = new SocialRatingManager(context);
                result = socialRatingManager.GetAverage(itemUrl) == null ? 0 : (int)socialRatingManager.GetAverage(itemUrl).Average;
            });
           return result;
       }

       /// <summary>
       /// Get the rating of a given url
       /// </summary>
       /// <param name="url"></param>
       /// <returns></returns>
       private int GetNoOfRating(string url)
       {
            int result = 0;
            SPSecurity.RunWithElevatedPrivileges(delegate()
            {

               var itemUrl = new Uri(url);
               var feedbackData = new FeedbackData { UserTitle = "" };
               SPServiceContext context = SPServiceContext.GetContext(_spSite);
               var socialRatingManager = new SocialRatingManager(context);
               result = socialRatingManager.GetCount(itemUrl);
            });
            return result;
       }

        private SPList GetSharePointListByName(string listName)
        {
            return (from SPList list in _spWeb.Lists
                    where list.RootFolder.Name.Equals(listName, StringComparison.InvariantCulture)
                    select list).FirstOrDefault();
        }

        #endregion

        #region Dispose implementation

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (disposing)
            {
                _spWeb.Dispose();
                _spSite.Dispose();
                _spWeb = null;
                _spSite = null;
            }
        }

        #endregion
    }
}
