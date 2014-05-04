﻿<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<OpenLawOffice.WebClient.ViewModels.Matters.MatterViewModel>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
	Matters of Contact
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <h2>Matters of Contact<a id="pageInfo" class="btn-question" style="padding-left: 15px;">Help</a></h2>
    <table class="listing_table">
        <tr>
            <th>
                Display Name
            </th>
            <th>
                Role
            </th>
            <th style="width: 40px;">
            </th>
        </tr>
    <% foreach (var item in Model)
        { %>
        <tr>
            <td>
                <%: Html.ActionLink(item.Contact.DisplayName, "Details", "Contacts", new { id = item.Id }, null)%>
            </td>
            <td>
                <%: Html.ActionLink(item.Role, "Details", "MatterContact", new { id = item.Id }, null)%>
            </td>
            <td>
                <%: Html.ActionLink("Edit", "Edit", "MatterContact", new { id = item.Id }, new { @class = "btn-edit", title = "Edit" })%>
                <%: Html.ActionLink("Unassign", "Delete", "MatterContact", new { id = item.Id }, new { @class = "btn-remove", title = "Unassign" })%>
            </td>
        </tr>
    <% } %>
    </table>
    <div id="pageInfoDialog" title="Help">
        <p>
        <span style="font-weight: bold; text-decoration: underline;">Info:</span>
        Contacts are the people (individuals or organizations) that do things or have things done to or for them. 
        The contacts on this page are assigned to the matter with the role stated.<br /><br />
        <span style="font-weight: bold; text-decoration: underline;">Usage:</span>
        Clicking the contact will show the details of the contact.
        Clicking the role will show the details of the contact assignment.  Click the 
        <img src="../../Content/fugue-icons-3.5.6/icons-shadowless/pencil.png" /> (edit icon) to make 
        changes to the contact.  Click the 
        <img src="../../Content/fugue-icons-3.5.6/icons-shadowless/cross.png" /> (remove icon) to 
        unassign the contact from the matter.
        </p>
    </div>
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="MenuContent" runat="server">
</asp:Content>