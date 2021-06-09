<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ApplicantInformationReportBulk.aspx.cs" Inherits="Abundance_Nk.Web.Reports.Presenter.ApplicantInformationReportBulk" %>
<%@ Register Assembly="Microsoft.ReportViewer.WebForms, Version=11.0.0.0, Culture=neutral, PublicKeyToken=89845dcd8080cc91" Namespace="Microsoft.Reporting.WebForms" TagPrefix="rsweb" %>
<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <link href="~/Content/bootstrap.min.css" rel="stylesheet" />
    <title></title>
</head>
<body style="border: none; margin: 0; padding: 0">

    <form id="form1" runat="server">
        <asp:ScriptManager ID="ScriptManager1" runat="server" AsyncPostBackTimeout="60000">
        </asp:ScriptManager>
        <div>
            <asp:UpdatePanel ID="UpdatePanel1" runat="server">
                <ContentTemplate>
                    <p>
                        <asp:Label ID="lblMessage" runat="server"></asp:Label>
                        <div class="form-inline">
                            <div class="form-group">
                                <asp:DropDownList ID="ddlSession" runat="server" AutoPostBack="True" class="form-control" Height="35px" Width="200px">
                                </asp:DropDownList>

                                &nbsp
                                <asp:DropDownList ID="ddlProgramme" runat="server" AutoPostBack="True" class="form-control" Height="35px" OnSelectedIndexChanged="ddlProgramme_SelectedIndexChanged1" Width="200px">
                                </asp:DropDownList>
                                &nbsp
                                <asp:DropDownList ID="ddlDepartment" runat="server" class="form-control" Height="35px" Width="200px">
                                </asp:DropDownList>

                                &nbsp
                                <asp:Button ID="Display_Button" runat="server" class="btn btn-success " OnClick="Display_Button_Click1" Text="Bulk Download" Width="155px" />
                                &nbsp
                                <asp:UpdateProgress ID="UpdateProgress1" runat="server" AssociatedUpdatePanelID="UpdatePanel1">
                                    <ProgressTemplate>
                                        <asp:Image ID="Image1" runat="server" ImageUrl="~/Content/Images/bx_loader.gif" />
                                        Loading...
                                    </ProgressTemplate>
                                </asp:UpdateProgress>
                            </div>
                        </div>
                        <p>
                        </p>
                        <p>
                        </p>
                        <p>
                        </p>
                        <p>
                        </p>
                        <p>
                        </p>
                        <p>
                        </p>
                        <p>
                        </p>
                        <p>
                        </p>
                    </p>
                </ContentTemplate>
            </asp:UpdatePanel>
        </div>
        <script src="~/Scripts/jquery-1.11.1.min.js"></script>
        <script src="../../Scripts/jquery-2.1.3.js"></script>
        <script src="../../Scripts/bootstrap.min.js"></script>
    </form>
</body>
</html>
