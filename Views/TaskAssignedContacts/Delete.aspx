﻿<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<OpenLawOffice.WebClient.ViewModels.Tasks.TaskAssignedContactViewModel>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    Unassign Contact from Task
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <h2>
        Unassign Contact from Task<a id="pageInfo" class="btn-question" style="padding-left: 15px;">Help</a></h2>
    <% using (Html.BeginForm())
       {%>
    <%: Html.ValidationSummary(true) %>
    <h3>
        Are you sure you want to unlink the contact from the task?</h3>
    <table class="detail_table">
        <tr>
            <td class="display-label">
                Task
            </td>
            <td class="display-field">
                <%: Model.Task.Title %>
            </td>
        </tr>
        <tr>
            <td class="display-label">
                User
            </td>
            <td class="display-field">
                <%: Model.Contact.DisplayName %>
            </td>
        </tr>
        <tr>
            <td class="display-label">
                Assignment
            </td>
            <td class="display-field">
                <%: Model.AssignmentType %>
            </td>
        </tr>
    </table>
    <table class="detail_table">
        <tr>
            <td colspan="5" style="font-weight: bold;">
                Core Details
            </td>
        </tr>
        <tr>
            <td class="display-label">
                Created By
            </td>
            <td class="display-field">
                <%: Model.CreatedBy.Username %>
            </td>
            <td style="width: 10px;">
            </td>
            <td class="display-label">
                Created At
            </td>
            <td class="display-field">
                <%: String.Format("{0:g}", DateTime.SpecifyKind(Model.UtcCreated.Value, DateTimeKind.Utc).ToLocalTime())%>
            </td>
        </tr>
        <tr>
            <td class="display-label">
                Modified By
            </td>
            <td class="display-field">
                <%: Model.ModifiedBy.Username %>
            </td>
            <td style="width: 10px;">
            </td>
            <td class="display-label">
                Modified At
            </td>
            <td class="display-field">
                <%: String.Format("{0:g}", DateTime.SpecifyKind(Model.UtcModified.Value, DateTimeKind.Utc).ToLocalTime())%>
            </td>
        </tr>
        <tr>
            <td class="display-label">
                Disabled By
            </td>
            <% if (Model.DisabledBy != null)
               { %>
            <td class="display-field">
                <%: Model.DisabledBy.Username%>
            </td>
            <% }
               else
               { %>
            <td />
            <% } %>
            <td style="width: 10px;">
            </td>
            <td class="display-label">
                Disabled At
            </td>
            <% if (Model.UtcDisabled.HasValue)
               { %>
            <td class="display-field">
                <%: String.Format("{0:g}", DateTime.SpecifyKind(Model.UtcDisabled.Value, DateTimeKind.Utc).ToLocalTime())%>
            </td>
            <% }
               else
               { %>
            <td class="display-field">
            </td>
            <% } %>
        </tr>
    </table>
    <p>
        <input type="submit" value="Delete" />
    </p>
    <% } %>
    <div id="pageInfoDialog" title="Help">
        <p>
        <span style="font-weight: bold; text-decoration: underline;">Info:</span>
        This page allows a contact to be removed from a task by "unassigning" that contact.<br /><br />
        <span style="font-weight: bold; text-decoration: underline;">Usage:</span>
        To unassign the contact from the task, click the unassign button.
        </p>
    </div>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="MenuContent" runat="server">
</asp:Content>