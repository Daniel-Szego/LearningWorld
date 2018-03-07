<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=3.5.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %>
<%@ Register TagPrefix="WebPartPages" Namespace="Microsoft.SharePoint.WebPartPages" Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="Taxonomy" Namespace="Microsoft.SharePoint.Taxonomy" Assembly="Microsoft.SharePoint.Taxonomy, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="Rating" Namespace="Microsoft.SharePoint.Portal.WebControls" Assembly="Microsoft.SharePoint.Portal, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="DocumentControl.ascx.cs" Inherits="Lernwelt.SharePoint.Web.Controls.DocumentControl" %>

<script type="text/javascript" src="/_layouts/Lernwelt.SharePoint.Web/jquery.min.js"></script>

<script type="text/javascript" src="/_layouts/Lernwelt.SharePoint.Web/LernweltRatings.js"></script>

<script type="text/javascript">
    // init
    $(document).ready(function () {
        InitializeRating();
    });

    //Re-bind for callbacks
    var prm = Sys.WebForms.PageRequestManager.getInstance();

    prm.add_endRequest(function () {
        InitializeRating();
      });


</script>

     <asp:UpdateProgress ID="progressUpdate3" runat="server" DisplayAfter="1000" AssociatedUpdatePanelID="UpdatePanel1">
       <ProgressTemplate>
         <div style="position: absolute; top: 0px; left: 0px; width: 100%; height: 100%; background-color: #e6e6e6;
           filter: alpha(opacity=50); opacity: 0.50;">
           <div style="position:relative; width:100px; height:100px; top:35%; left:50%">
              <div style="position: absolute; width: 64px; height: 64px; top: 50%; left: 50%; margin-left: -32px;
                 margin-top: -32px; z-index: 9999">
              <img src="/_layouts/Lernwelt.SharePoint.Web/ajaxLoading.gif" alt="" />
           </div>
         </div>
        </div>
      </ProgressTemplate>
    </asp:UpdateProgress> 

        <table>
            <tbody>
                <tr>
                    <td>
                        <asp:Literal runat="server" Text="<%$Resources:Lernwelt.SharePoint,Control_Documents_FieldTitle_Name%>" />
                    </td>
                    <td>
                        <asp:TextBox ID="txtName" runat="server"></asp:TextBox>
                        <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ErrorMessage="<%$Resources:Lernwelt.SharePoint,Name_Not_Found_Error%>" Text="*"
                        ControlToValidate="txtName" ValidationGroup="ErrorGroup1"/>
                    </td>
                </tr>
                <tr>
                    <td>
                        <asp:Literal runat="server" Text="<%$Resources:Lernwelt.SharePoint,Control_Documents_FieldTitle_Title%>" />
                    </td>
                    <td>

                        <asp:TextBox ID="txtTitle" runat="server"></asp:TextBox>
                        <asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server" ErrorMessage="<%$Resources:Lernwelt.SharePoint,Title_Not_Found_Error%>" Text="*"
                        ControlToValidate="txtTitle" ValidationGroup="ErrorGroup1"/>
                    </td>
                </tr>
                <tr>
                    <td>
                        <asp:Literal runat="server" Text="<%$Resources:Lernwelt.SharePoint,Control_Documents_FieldTitle_Documenttype%>" />
                    </td>
                    <td>
                        <asp:DropDownList ID="dlDocumentType" runat="server" />
                    </td>
                </tr>
                <tr>
                    <td>
                        <asp:Literal runat="server" Text="<%$Resources:Lernwelt.SharePoint,Control_Documents_FieldTitle_ShortDescription%>" />
                    </td>
                    <td>
                        <SharePoint:InputFormTextBox ID="rtxDescription" class="ms-input" runat="server" Rows="8" Columns="40" RichText="False" TextMode="MultiLine" RichTextMode="Compatible" />
                        <asp:RequiredFieldValidator ID="RequiredFieldValidator4" runat="server" ErrorMessage="<%$Resources:Lernwelt.SharePoint,Desc_Not_Found_Error%>" Text="*"
                        ControlToValidate="rtxDescription" ValidationGroup="ErrorGroup1"/>
                    </td>
                </tr>
                <tr>
                    <td>
                        <asp:Literal runat="server" Text="<%$Resources:Lernwelt.SharePoint,Control_Documents_FieldTitle_Language%>" />
                    </td>
                    <td>
                        <asp:DropDownList ID="dlLanguage" runat="server" AutoPostBack="True" OnSelectedIndexChanged="dlLanguage_OnSelectedIndexChanged" />
                    </td>
                </tr>
                <tr>
                    <td>
                        <asp:Literal runat="server" Text="<%$Resources:Lernwelt.SharePoint,Control_Documents_FieldTitle_KnowledgeArea%>" />
                    </td>
                    <td>
                        <Taxonomy:TaxonomyWebTaggingControl ID="taxKnowledgeArea" runat="server" IsMulti="False"/>
                        <asp:CustomValidator ID="taxKnowledgeAreaValidator" runat="server" Display="Dynamic" OnServerValidate="taxKnowledgeArea_ServerValidate" ValidationGroup="ErrorGroup1"></asp:CustomValidator>
                    </td>
                </tr>
                <tr>
                    <td>
                        <asp:Literal runat="server" Text="<%$Resources:Lernwelt.SharePoint, Control_Documents_FieldTitle_Author%>"></asp:Literal>
                    </td>
                    <td>
                        <SharePoint:PeopleEditor AllowEmpty="False" SingleLine="True" ValidatorEnabled="True" MultiSelect="False" ID="PickerAddUser" runat="server" SelectionSet="User" Width="350px" BorderColor="Black" BorderWidth="1" />
                        <asp:RequiredFieldValidator ID="RequiredFieldValidator3" runat="server" ErrorMessage="<%$Resources:Lernwelt.SharePoint,Author_Not_Found_Error%>" Text="*"
                        ControlToValidate="PickerAddUser" ValidationGroup="ErrorGroup1"/>
                    </td>
                </tr>
                <tr>
                    <td>
                        <asp:Literal runat="server" Text="<%$Resources:Lernwelt.SharePoint, Control_Documents_FieldTitle_Targetgroup%>"></asp:Literal>
                    </td>
                    <td>
                        <asp:DropDownList ID="dlTargetGroup" runat="server" />
                    </td>
                </tr>
                <tr>
                    <td>
                        <asp:Literal runat="server" Text="<%$Resources:Lernwelt.SharePoint, Control_Documents_FieldTitle_Rating%>"></asp:Literal>
                    </td>
                    <td>                    
                    <div id="starPanel">
		                <img id="star1" tabIndex="0" alt="1 star selected. Press SHIFT+ENTER to submit. Press TAB to increase rating. Press SHIFT+ESCAPE to leave rating submit mode." src="/_layouts/Lernwelt.SharePoint.Web/RatingsNew.png" num="1"> </img>
                        <img id="star2" tabIndex="0" alt="2 stars selected. Press SHIFT+ENTER to submit. Press TAB to increase rating. Press SHIFT+TAB to decrease rating. Press SHIFT+ESCAPE to leave rating submit mode." src="/_layouts/Lernwelt.SharePoint.Web/RatingsNew.png" num="2"> </img>
                        <img id="star3" tabIndex="0" alt="3 stars selected. Press SHIFT+ENTER to submit. Press TAB to increase rating. Press SHIFT+TAB to decrease rating. Press SHIFT+ESCAPE to leave rating submit mode." src="/_layouts/Lernwelt.SharePoint.Web/RatingsNew.png" num="3"> </img>
                        <img id="star4" tabIndex="0" alt="4 stars selected. Press SHIFT+ENTER to submit. Press TAB to increase rating. Press SHIFT+TAB to decrease rating. Press SHIFT+ESCAPE to leave rating submit mode." src="/_layouts/Lernwelt.SharePoint.Web/RatingsNew.png" num="4"> </img>
                        <img id="star5" tabIndex="0" alt="5 stars selected. Press SHIFT+ENTER to submit. Press SHIFT+TAB to decrease rating. Press SHIFT+ESCAPE to leave rating submit mode." src="/_layouts/Lernwelt.SharePoint.Web/RatingsNew.png" num="5"> </img>
                    </div>
                        <asp:HiddenField ID="hdnStarNum" runat="server"/>

<%--                        <asp:RadioButtonList ID="rbRating" runat="server">
                            <asp:ListItem Value="1" Text="*" />
                            <asp:ListItem Value="2" Text="**" />
                            <asp:ListItem Value="3" Text="***" />
                            <asp:ListItem Value="4" Text="****" />
                            <asp:ListItem Value="5" Text="*****" />
                        </asp:RadioButtonList>--%>
                    </td>
                </tr>
                <tr>
                    <td>
                     <asp:Literal ID="Literal2" runat="server" Text="<%$Resources:Lernwelt.SharePoint, Control_Documents_FieldTitle_DocumentsKeyword%>"></asp:Literal>
                    </td>
                <td  class="width:350px">
                <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Always" ChildrenAsTriggers="true">
                    <ContentTemplate>
                    <table class="ms-input">
                    <tr>
                      <td>
                          <Taxonomy:TaxonomyWebTaggingControl ID="taxKeywords" runat="server" IsMulti="True" />
<%--                          <asp:CustomValidator ID="taxKeywordsValidator" runat="server" Display="Dynamic" OnServerValidate="taxKeywords_ServerValidate" ValidationGroup="ErrorGroup1"></asp:CustomValidator>
--%>
                      </td>
                    </tr>
                    <tr>
                      <td>
                          <asp:Button ID="btnRefresh" runat="server" Text="<%$Resources:Lernwelt.SharePoint, Control_Documents_FieldTitle_Refresh%>" onclick="btnRefresh_Click"></asp:Button>
                      </td>
                    </tr>
 <%--                   <asp:Panel ID="pnlKeywordSearch" runat="server" Visible="True">--%>
                    <tr>
                        <td>
                            <asp:DropDownList ID="drpDocuments" runat="server" Width="350px">
                            </asp:DropDownList>
                        </td>
                    </tr>
                    </table>
<%--                    </asp:Panel>--%>
                    </ContentTemplate>
                    <Triggers>
                        <asp:AsyncPostBackTrigger ControlID="btnRefresh"  />
                    </Triggers>
                </asp:UpdatePanel>
                </td>
                </tr>
                <tr>
                    <td>
                        <asp:Literal runat="server" ID="ltrlFileUpload" Text="<%$Resources:Lernwelt.SharePoint, Control_Documents_FieldTitle_FileUpload%>"></asp:Literal>
                    </td>
                    <td>
                        <asp:FileUpload ID="flUpload" runat="server" />
                    </td>
                </tr>
                <tr>
                    <td></td>
                    <td>
                        <asp:Button ID="btnSubmit" runat="server" Text="<%$Resources:Lernwelt.SharePoint, Control_Documents_Button%>" OnClick="btnSubmit_OnClick" ValidationGroup="ErrorGroup1"/>
                    </td>
                </tr>
                <tr>
                    <td>
                        <asp:ValidationSummary ID="ValidationSummary1" runat="server"
                            HeaderText="Validation errors:" ValidationGroup="ErrorGroup1"/>
                    </td>
                </tr>
            </tbody>
        </table>

