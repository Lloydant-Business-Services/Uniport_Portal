<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="AdmittedStudentBreakdown.aspx.cs" Inherits="Abundance_Nk.Web.Reports.Presenter.AdmittedStudentBreakdown" %>
<%@ Register Assembly="Microsoft.ReportViewer.WebForms, Version=11.0.0.0, Culture=neutral, PublicKeyToken=89845dcd8080cc91" Namespace="Microsoft.Reporting.WebForms" TagPrefix="rsweb" %>
<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <link href="../../Content/bootstrap.min.css" rel="stylesheet" />
    <title></title>
</head>
<body>

    <form id="form1" runat="server">
        <asp:ScriptManager ID="ScriptManager1" runat="server" AsyncPostBackTimeout="6000000">
        </asp:ScriptManager>
        <div>
            <asp:UpdatePanel ID="UpdatePanel1" runat="server">
                <ContentTemplate>
                    <p>
                        <asp:Label ID="lblMessage" runat="server"></asp:Label>
                    </p>
                    <div class="form-inline">
                       
                        <br />

                        <div class="form-group">
     
                          
                            <div class="form-group">
                                <label class="sr-only" for="ddlSessions"></label>
                                <asp:DropDownList ID="ddlSession" CssClass="form-control" runat="server"></asp:DropDownList>
                            </div>

                            <asp:Button ID="Display_Button" runat="server" Text="Display Report" Width="111px" class="btn btn-success " OnClick="btnDisplayReport_Click" />
                            <div class="form-group">
                                <asp:UpdateProgress ID="UpdateProgress1" AssociatedUpdatePanelID="UpdatePanel1" runat="server">
                                    <ProgressTemplate>
                                        <asp:Image ID="Image1" runat="server" ImageUrl="~/Content/Images/bx_loader.gif" />
                                        Loading...
                                    </ProgressTemplate>
                                </asp:UpdateProgress>
                            </div>
                        </div>
                    </div>
                    <rsweb:ReportViewer ID="rv" runat="server"
                        Font-Names="Verdana" Font-Size="8pt"
                        ShowBackButton="False" ShowCredentialPrompts="False"
                        ShowDocumentMapButton="False" ShowFindControls="False"
                        ShowPageNavigationControls="False" ShowParameterPrompts="False"
                        ShowPromptAreaButton="False" ShowPrintButton="true" Height="800px" Width="100%" BackColor="White" BorderStyle="None">
                    </rsweb:ReportViewer>

                </ContentTemplate>
            </asp:UpdatePanel>
        </div>

    </form>
</body>
</html>