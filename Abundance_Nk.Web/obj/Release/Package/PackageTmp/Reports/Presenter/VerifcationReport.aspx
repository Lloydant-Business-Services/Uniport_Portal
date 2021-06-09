<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="VerifcationReport.aspx.cs" Inherits="Abundance_Nk.Web.Reports.Presenter.VerifcationReport" %>

<%@ Register TagPrefix="rsweb" Namespace="Microsoft.Reporting.WebForms" Assembly="Microsoft.ReportViewer.WebForms, Version=11.0.0.0, Culture=neutral, PublicKeyToken=89845dcd8080cc91" %>
<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <link href="../../Content/bootstrap.min.css" rel="stylesheet" />
</head>
<body>
    <form id="form1" runat="server">
        <asp:ScriptManager ID="ScriptManager1" runat="server" AsyncPostBackTimeout="60000">
        </asp:ScriptManager>
        <div>
            <asp:UpdatePanel ID="UpdatePanel1" runat="server">
                <ContentTemplate>
                    <p>
                        <asp:Label ID="lblMessage" runat="server"></asp:Label>
                    </p>
                    <div class="row table-responsive">
                        <table class="table">
                            <tr>
                                <td>&nbsp;</td>
                                <td>
                                    <asp:TextBox ID="txtBoxDateFrom" runat="server" RepeatDirection="Horizontal" class="form-control txtDatePicker" placeholder="Start Date"></asp:TextBox>
                                </td>
                                <td>&nbsp;</td>
                                <td>
                                    <asp:TextBox ID="txtBoxDateTo" runat="server" RepeatDirection="Horizontal" class="form-control txtDatePicker" placeholder="End Date"></asp:TextBox>
                                </td>
                                <td>&nbsp;</td>
                                <td>
                                    <asp:DropDownList ID="ddlFeeType" class="form-control" runat="server" Height="35px" Width="250px">
                                    </asp:DropDownList></td>
                            </tr>
                            <tr>
                                <td></td>
                                <td>
                                    <asp:DropDownList ID="ddlSession" class="form-control" runat="server" Height="35px" Width="250px"></asp:DropDownList>
                                </td>
                                <td></td>
                                <td>
                                    <asp:DropDownList ID="ddlLevel" class="form-control" runat="server" Height="35px" Width="250px"></asp:DropDownList>
                                </td>
                                <td></td>
                                <td>
                                    <asp:DropDownList ID="ddlProgramme" class="form-control" runat="server" Height="35px" Width="200px" AutoPostBack="True" OnSelectedIndexChanged="ddlProgramme_SelectedIndexChanged">
                                    </asp:DropDownList></td>
                            </tr>
                            <tr>
                                <td></td>
                                <td>
                                    <asp:DropDownList ID="ddlDepartment" class="form-control" runat="server" Height="35px" Width="250px">
                                    </asp:DropDownList></td>
                                <td></td>
                                <td></td>
                                <td></td>
                                <td>
                                    <asp:Button ID="Display_Button" runat="server" Text="Display Report" Width="111px" class="btn btn-success " OnClick="Display_Button_Click1" />
                                </td>
                            </tr>
                        </table>
                    </div>

                </ContentTemplate>
            </asp:UpdatePanel>
        </div>
        <script src="../../Scripts/jquery-2.1.3.js"></script>
        <script src="../../Scripts/bootstrap.min.js"></script>
        <link href="../../Content/jquery-ui-1.10.3.css" rel="stylesheet" />
        <script src="../../Scripts/file-upload/jquery-ui-1.9.2.min.js"></script>

        <script type="text/javascript">
            $(function () {
                $('.txtDatePicker').datepicker(
                {
                    dateFormat: 'dd/mm/yy',
                    changeMonth: true,
                    changeYear: true,
                    yearRange: '1950:2100'
                });
            })
        </script>

        <div class="row">
            <table class="table">
                <tr>
                    <td></td>
                    <td> 
                        <rsweb:ReportViewer  ID="ReportViewer1" runat="server" Width="900px" ></rsweb:ReportViewer>
                    </td>
                </tr>
            </table>
           
        </div>

    </form>
</body>
</html>
