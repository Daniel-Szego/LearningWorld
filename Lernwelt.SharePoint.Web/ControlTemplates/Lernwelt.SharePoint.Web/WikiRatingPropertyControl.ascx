<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=3.5.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %> 
<%@ Register Tagprefix="WebPartPages" Namespace="Microsoft.SharePoint.WebPartPages" Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="Taxonomy" Namespace="Microsoft.SharePoint.Taxonomy" Assembly="Microsoft.SharePoint.Taxonomy, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="WikiRatingPropertyControl.ascx.cs" Inherits="Lernwelt.SharePoint.Web.ControlTemplates.Lernwelt.SharePoint.Web.WikiRatingPropertyControl" %>
<asp:HiddenField runat="server" ID="hiddenFieldDetectRequest" Value="0" />
<asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional">
    <ContentTemplate>
        <table cellspacing="0" border="0" style="border-width: 0px; width: 100%; border-collapse: collapse;">
            <tbody>
                <tr>
                    <td>
                        <div class="UserSectionHead">
                            <asp:Label ID="Label1" runat="server" AssociatedControlID="ddlDocumentLibraries" EnableViewState="False" Text="<%$Resources:Lernwelt.SharePoint,Control_Documents_Property_ListTitle%>"></asp:Label>
                        </div>
                        <div class="UserSectionBody">
                            <div class="UserControlGroup">
                                <asp:DropDownList ID="ddlDocumentLibraries" AutoPostBack="True" runat="server" OnSelectedIndexChanged="ddlDocumentLibraries_OnSelectedIndexChanged"></asp:DropDownList>
                            </div>
                        </div>
                    </td>
                </tr>
                <tr>
                    <td>
                        <asp:Panel ID="Panel1" runat="server" GroupingText="<%$Resources:Lernwelt.SharePoint,Control_Documents_Property_FieldsetTitle%>">
                            <div style="padding: 5px; margin: 2px">
                                <table cellspacing="0" border="0" style="border-width: 0px; width: 100%; border-collapse: collapse;">
                                    <tbody>
                                     <tr>
                                            <td>
                                                <div class="UserSectionHead">
                                                    <asp:Label ID="Label4" runat="server" AssociatedControlID="ddlLink" EnableViewState="False" Text="<%$Resources:Lernwelt.SharePoint,Control_Documents_FieldTitle_Link%>"></asp:Label>
                                                </div>
                                                <div class="UserSectionBody">
                                                    <div class="UserControlGroup">
                                                        <asp:DropDownList ID="ddlLink" runat="server"></asp:DropDownList>
                                                    </div>
                                                </div>
                                                <div style="width: 100%" class="UserDottedLine"></div>
                                            </td>
                                        </tr>

                                         <tr>
                                            <td>
                                                <div class="UserSectionHead">
                                                    <asp:Label ID="Label7" runat="server" AssociatedControlID="ddlKeywords" EnableViewState="False" Text="<%$Resources:Lernwelt.SharePoint,Control_Documents_FieldTitle_Keywords%>"></asp:Label>
                                                </div>
                                                <div class="UserSectionBody">
                                                    <div class="UserControlGroup">
                                                        <asp:DropDownList ID="ddlKeywords" runat="server"></asp:DropDownList>
                                                    </div>
                                                </div>
                                                <div style="width: 100%" class="UserDottedLine"></div>
                                            </td>
                                        </tr>
                                       <tr>
                                            <td>
                                                <div class="UserSectionHead">
                                                    <asp:Label ID="Label3" runat="server" AssociatedControlID="taxKeywords" EnableViewState="False" Text="<%$Resources:Lernwelt.SharePoint,Control_Wiki_FieldTitle_FaqKeyword%>"></asp:Label>
                                                </div>
                                                <div class="UserSectionBody">
                                                    <div class="UserControlGroup">
                                                        <Taxonomy:TaxonomyWebTaggingControl ID="taxKeywords" runat="server" IsMulti="True" BorderStyle="None"/>
                                                    </div>
                                                </div>
                                                <div style="width: 100%" class="UserDottedLine"></div>
                                            </td>
                                        </tr>
                                       <tr>
                                            <td>
                                                <div class="UserSectionHead">
                                                    <asp:Label ID="Label11" runat="server" AssociatedControlID="txtEntryLimit" EnableViewState="False" Text="<%$Resources:Lernwelt.SharePoint,Control_Wiki_FieldTitle_EntryLimit%>"></asp:Label>
                                                </div>
                                                <div class="UserSectionBody">
                                                    <div class="UserControlGroup">
                                                        <asp:TextBox ID="txtEntryLimit" runat="server" />
                                                    </div>
                                                </div>
                                                <div style="width: 100%" class="UserDottedLine"></div>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td>
                                                <div class="UserSectionHead">
                                                    <asp:Label ID="Label12" runat="server" AssociatedControlID="ddlOpenLink" EnableViewState="False" Text="<%$Resources:Lernwelt.SharePoint,Control_Documents_FieldTitle_OpenLink%>"></asp:Label>
                                                </div>
                                                <div class="UserSectionBody">
                                                    <div class="UserControlGroup">
                                                        <asp:DropDownList ID="ddlOpenLink" runat="server">
                                                         <asp:ListItem Text="New Window" Value="0"></asp:ListItem>
                                                         <asp:ListItem Text="Same Window" Value="1"></asp:ListItem>
                                                         <asp:ListItem Text="Modal Dialog" Value="2"></asp:ListItem>
                                                        </asp:DropDownList>
                                                    </div>
                                                </div>
                                                <div style="width: 100%" class="UserDottedLine"></div>
                                            </td>
                                        </tr>
                                        <tr  style="display:none">
                                            <td>
                                                <div class="UserSectionHead">
                                                    <asp:Label ID="Label2" runat="server" AssociatedControlID="txtTitle" EnableViewState="False" Text="<%$Resources:Lernwelt.SharePoint,Control_Documents_FieldTitle_Title%>"></asp:Label>
                                                </div>
                                                <div class="UserSectionBody">
                                                    <div class="UserControlGroup">
                                                        <asp:TextBox ID="txtTitle" runat="server"></asp:TextBox>
                                                    </div>
                                                </div>
                                                <div style="width: 100%" class="UserDottedLine"></div>
                                            </td>
                                        </tr>
                                     </tbody>
                                </table>
                            </div>
                        </asp:Panel>
                    </td>
                    <tr>
                        <td>
                            <div class="UserSectionHead">
                                <asp:Label runat="server" AssociatedControlID="ltrVersion" ID="lblVersion" Text="Webpart version" EnableViewState="False"></asp:Label>
                            </div>
                            <div class="UserSectionBody">
                                <div class="UserControlGroup">
                                    <asp:Literal ID="ltrVersion" runat="server"></asp:Literal>
                                </div>
                            </div>
                        </td>
                    </tr>
                </tr>
            </tbody>
        </table>
    </ContentTemplate>
</asp:UpdatePanel>
