<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="PaymentReportGeneral.aspx.cs" Inherits="Abundance_Nk.Web.Reports.Presenter.PaymentReportGeneral" %>

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
                                 <asp:TextBox ID="txtBoxDateFrom" runat="server" RepeatDirection="Horizontal" class="form-control" placeholder="Start Date"></asp:TextBox>

                                 <asp:TextBox ID="txtBoxDateTo" runat="server" RepeatDirection="Horizontal" class="form-control" placeholder="End Date"></asp:TextBox>
                            </div>

                            <div class="form-group">
                                <label class="sr-only" for="ddlProgramme"></label>
                                <asp:DropDownList ID="ddlProgramme" CssClass="form-control" runat="server" AutoPostBack="True" OnSelectedIndexChanged="ddlProgramme_SelectedIndexChanged"></asp:DropDownList>
                            </div>
                            <div class="form-group">
                                <label class="sr-only" for="ddlDepartment"></label>
                                <asp:DropDownList ID="ddlDepartment" CssClass="form-control" runat="server"></asp:DropDownList>
                            </div>
                            <div class="form-group">
                                <label class="sr-only" for="ddlFeeTypes"></label>
                                <asp:DropDownList ID="ddlFeeTypes" CssClass="form-control" runat="server"></asp:DropDownList>
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
                <script src="~/Scripts/jquery-1.11.1.min.js"></script>
    <script src="../../Scripts/jquery-2.1.3.js"></script>
    <script src="../../Scripts/bootstrap.min.js"></script>
 
        <link href="../../Content/jquery-ui-1.10.3.css" rel="stylesheet" />
        <script src="../../Scripts/file-upload/jquery-ui-1.9.2.min.js"></script>

      <script type="text/javascript">
          $(function () {
              $('#txtBoxDateFrom').datepicker(
              {
                  dateFormat: 'dd/mm/yy',
                  changeMonth: true,
                  changeYear: true,
                  yearRange: '1950:2100'
              });

              $('#txtBoxDateTo').datepicker(
             {
                 dateFormat: 'dd/mm/yy',
                 changeMonth: true,
                 changeYear: true,
                 yearRange: '1950:2100'
             });

              ////function myAlrt() {
              //    alert("Hi");
              ////};
          })
          
        </script>

    </form>
</body>
</html>