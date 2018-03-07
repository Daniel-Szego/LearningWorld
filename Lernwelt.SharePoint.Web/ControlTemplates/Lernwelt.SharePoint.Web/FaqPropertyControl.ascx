<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=3.5.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %>
<%@ Register TagPrefix="WebPartPages" Namespace="Microsoft.SharePoint.WebPartPages" Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="FaqPropertyControl.ascx.cs" Inherits="Lernwelt.SharePoint.Web.Controls.FaqPropertyControl" %>
<asp:HiddenField runat="server" ID="hiddenFieldDetectRequest" Value="0" />
<asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional">
    <ContentTemplate>
        <table cellspacing="0" border="0" style="border-width: 0px; width: 100%; border-collapse: collapse;">
            <tbody>
                <tr>
                    <td>
                        <div class="UserSectionHead">
                            <asp:Label runat="server" AssociatedControlID="ddlDocumentLibraries" EnableViewState="False" Text="<%$Resources:Lernwelt.SharePoint,Control_Documents_Property_ListTitle%>"></asp:Label>
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
                        <asp:Panel runat="server" GroupingText="<%$Resources:Lernwelt.SharePoint,Control_Documents_Property_FieldsetTitle%>">
                            <div style="padding: 5px; margin: 2px">
                                <table cellspacing="0" border="0" style="border-width: 0px; width: 100%; border-collapse: collapse;">
                                    <tbody>
                                        <tr>
                                            <td>
                                                <div class="UserSectionHead">
                                                    <asp:Label ID="Label1" runat="server" AssociatedControlID="ddlQuestion" EnableViewState="False" Text="<%$Resources:Lernwelt.SharePoint,Control_Faq_FieldTitle_Question%>"></asp:Label>
                                                </div>
                                                <div class="UserSectionBody">
                                                    <div class="UserControlGroup">
                                                        <asp:DropDownList ID="ddlQuestion" runat="server"></asp:DropDownList>
                                                    </div>
                                                </div>
                                                <div style="width: 100%" class="UserDottedLine"></div>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td>
                                                <div class="UserSectionHead">
                                                    <asp:Label ID="Label2" runat="server" AssociatedControlID="ddlAnswer" EnableViewState="False" Text="<%$Resources:Lernwelt.SharePoint,Control_Faq_FieldTitle_Answer%>"></asp:Label>
                                                </div>
                                                <div class="UserSectionBody">
                                                    <div class="UserControlGroup">
                                                        <asp:DropDownList ID="ddlAnswer" runat="server"></asp:DropDownList>
                                                    </div>
                                                </div>
                                                <div style="width: 100%" class="UserDottedLine"></div>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td>
                                                <div class="UserSectionHead">
                                                    <asp:Label ID="Label5" runat="server" AssociatedControlID="ddlLanguage" EnableViewState="False" Text="<%$Resources:Lernwelt.SharePoint,Control_Documents_FieldTitle_Language%>"></asp:Label>
                                                </div>
                                                <div class="UserSectionBody">
                                                    <div class="UserControlGroup">
                                                        <asp:DropDownList ID="ddlLanguage" runat="server"></asp:DropDownList>
                                                    </div>
                                                </div>
                                                <div style="width: 100%" class="UserDottedLine"></div>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td>
                                                <div class="UserSectionHead">
                                                    <asp:Label ID="Label6" runat="server" AssociatedControlID="ddlKnowledgeArea" EnableViewState="False" Text="<%$Resources:Lernwelt.SharePoint,Control_Documents_FieldTitle_KnowledgeArea%>"></asp:Label>
                                                </div>
                                                <div class="UserSectionBody">
                                                    <div class="UserControlGroup">
                                                        <asp:DropDownList ID="ddlKnowledgeArea" runat="server"></asp:DropDownList>
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
                                                    <asp:Label ID="Label8" runat="server" AssociatedControlID="ddlAuthor" EnableViewState="False" Text="<%$Resources:Lernwelt.SharePoint,Control_Documents_FieldTitle_Author%>"></asp:Label>
                                                </div>
                                                <div class="UserSectionBody">
                                                    <div class="UserControlGroup">
                                                        <asp:DropDownList ID="ddlAuthor" runat="server"></asp:DropDownList>
                                                    </div>
                                                </div>
                                                <div style="width: 100%" class="UserDottedLine"></div>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td>
                                                <div class="UserSectionHead">
                                                    <asp:Label ID="Label9" runat="server" AssociatedControlID="ddlTargetGroup" EnableViewState="False" Text="<%$Resources:Lernwelt.SharePoint,Control_Documents_FieldTitle_Targetgroup%>"></asp:Label>
                                                </div>
                                                <div class="UserSectionBody">
                                                    <div class="UserControlGroup">
                                                        <asp:DropDownList ID="ddlTargetGroup" runat="server"></asp:DropDownList>
                                                    </div>
                                                </div>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td>
                                                <div class="UserSectionHead">
                                                    <asp:Label ID="Label10" runat="server" AssociatedControlID="ddlReference" EnableViewState="False" Text="<%$Resources:Lernwelt.SharePoint,Control_Documents_FieldTitle_Reference%>"></asp:Label>
                                                </div>
                                                <div class="UserSectionBody">
                                                    <div class="UserControlGroup">
                                                        <asp:DropDownList ID="ddlReference" runat="server"></asp:DropDownList>
                                                    </div>
                                                </div>
                                            </td>
                                        </tr>
<%--                                        <tr>
                                            <td>
                                                <div class="UserSectionHead">
                                                    <asp:Label ID="Label3" runat="server" AssociatedControlID="ddlNoOfRatings" EnableViewState="False" Text="<%$Resources:Lernwelt.SharePoint,Control_Documents_No_Of_Ratings%>"></asp:Label>
                                                </div>
                                                <div class="UserSectionBody">
                                                    <div class="UserControlGroup">
                                                        <asp:DropDownList ID="ddlNoOfRatings" runat="server"></asp:DropDownList>
                                                    </div>
                                                </div>
                                            </td>
                                        </tr>--%>
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
