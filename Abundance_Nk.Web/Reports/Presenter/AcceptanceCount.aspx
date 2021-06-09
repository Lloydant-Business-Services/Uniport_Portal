<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="AcceptanceCount.aspx.cs" Inherits="Abundance_Nk.Web.Reports.Presenter.AcceptanceCount" %>

<%@ Register Assembly="Microsoft.ReportViewer.WebForms, Version=11.0.0.0, Culture=neutral, PublicKeyToken=89845dcd8080cc91" Namespace="Microsoft.Reporting.WebForms" TagPrefix="rsweb" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <link href="../../Content/bootstrap.min.css" rel="stylesheet" />
    <link href="../../Content/bootstrap.css" rel="stylesheet" />
    <script src="../../Scripts/bootstrap.js"></script>
    <link href="../../Content/misc.css" rel="stylesheet" />
    <title></title>


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
                    <div class="contentpanel">
                        <div class="panel panel-default">
                            <div class="panel-body">

                                <%--<div class="row">
                                    <div class="col-md-12">

                                        <div class="form-inline col-md-offset-1">
                                            <div class="form-group">
                                                <label title="Date Range">Date Range: </label>
                                                &nbsp;
                                            </div>
                                            <div class="form-group">

                                                <asp:TextBox ID="txtBoxDateFrom" runat="server" RepeatDirection="Horizontal" placeholder="From Inception" disabled="disabled"></asp:TextBox>

                                                <asp:TextBox ID="txtBoxDateTo" runat="server" RepeatDirection="Horizontal" class="txtDatePicker" placeholder="To"></asp:TextBox>
                                            </div>
                                        </div>

                                    </div>
                                </div>
                                <br />--%>

                                <div class="row">
                                    <div class="col-md-12">
                                        <div class="form-inline col-md-offset-1">
                                            <div class="form-group">
                                                <label title="Date Range">Date Range</label>
                                                &nbsp;
                                            </div>
                                            <div class="form-group">

                                                <asp:TextBox ID="txtBoxDateFrom" runat="server" RepeatDirection="Horizontal" class="txtDatePicker" placeholder="From"></asp:TextBox>

                                                <asp:TextBox ID="txtBoxDateTo" runat="server" RepeatDirection="Horizontal" class="txtDatePicker" placeholder="To"></asp:TextBox>
                                            </div>
                                        </div>
                                    </div>
                                    <%--<div class="col-md-offset-1">
                                        <p><i>For bulk report, select session and programme</i></p>
                                    </div>--%>
                                </div>
                                <br />
                                <div class="row">
                                    <div class="col-md-12">
                                        <div class="form-inline col-md-offset-1">
                                            <div class="form-group">
                                                <asp:DropDownList ID="ddlSession" class="form-control" runat="server" Height="35px" Width="200px">
                                                </asp:DropDownList>

                                            </div>

                                            <div class="form-group">
                                                <asp:Button ID="Display_Button" runat="server" Text="Display Report" Width="111px" class="btn btn-success " OnClick="Display_Button_Click" />
                                            </div>
                                            <div class="form-group">
                                                <asp:UpdateProgress ID="UpdateProgress1" runat="server" AssociatedUpdatePanelID="UpdatePanel1">
                                                    <ProgressTemplate>
                                                        <asp:Image ID="Image1" runat="server" ImageUrl="~/Content/Images/bx_loader.gif" />
                                                        Loading...
                                                    </ProgressTemplate>
                                                </asp:UpdateProgress>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
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
                $('.txtDatePicker').datepicker(
                {
                    dateFormat: 'dd/mm/yy',
                    changeMonth: true,
                    changeYear: true,
                    yearRange: '1950:2100'
                });
            })
        </script>


        <rsweb:ReportViewer ID="ReportViewer1" runat="server" Width="100%">
        </rsweb:ReportViewer>




    </form>

</body>
</html>
