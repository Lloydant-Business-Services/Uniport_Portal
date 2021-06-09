using Abundance_Nk.Business;
using Abundance_Nk.Model.Model;
using Microsoft.Reporting.WebForms;
using System;
using System.Collections.Generic;
using System.IO;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;

namespace Abundance_Nk.Web.Reports.Presenter
{
    public partial class ApplicantInformationReport :Page
    {
        private int reportType;
        private long studentId;

        protected void Page_Load(object sender,EventArgs e)
        {
            try
            {
                lblMessage.Text = "";
                if(!IsPostBack)
                {
                    if(Request.QueryString["studentId"] != null && Request.QueryString["reportType"] != null)
                    {
                        studentId = Convert.ToInt64(Request.QueryString["studentId"]);
                        reportType = Convert.ToInt32(Request.QueryString["reportType"]);

                        BuildReport(studentId,reportType);
                    }
                }
            }
            catch(Exception ex)
            {
                lblMessage.Text = ex.Message + ex.InnerException.Message;
            }
        }

        private void BuildReport(long studentId,int type)
        {
            try
            {
                var person = new Person { Id = studentId };
                var studentLogic = new StudentLogic();
                var reportData = new List<Model.Model.StudentReport>();
                var OLevelDetail = new List<Model.Model.StudentReport>();

                OLevelDetail = studentLogic.GetStudentOLevelDetail(person);

                reportData = studentLogic.GetStudentBiodata(person);

                string bind_dsStudentReport = "dsStudentReport";
                string bind_dsStudentResult = "dsStudentResult";
                string reportPath = "";
                string returnAction = "";
                if(type == 1)
                {
                    reportPath = @"Reports\AbsuCertificateOfEligibility.rdlc";
                }
                if(type == 2)
                {
                    reportPath = @"Reports\AbsuCheckingCredentials.rdlc";
                }
                if(type == 3)
                {
                    reportPath = @"Reports\AbsuPersonalInfo.rdlc";
                }
                if(type == 4)
                {
                    reportPath = @"Reports\AbsuUnderTaking.rdlc";
                }

                if(Directory.Exists(Server.MapPath("~/Content/studentReportFolder")))
                {
                    Directory.Delete(Server.MapPath("~/Content/studentReportFolder"),true);
                    Directory.CreateDirectory(Server.MapPath("~/Content/studentReportFolder"));
                }
                else
                {
                    Directory.CreateDirectory(Server.MapPath("~/Content/studentReportFolder"));
                }

                Warning[] warnings;
                string[] streamIds;
                string mimeType = string.Empty;
                string encoding = string.Empty;
                string extension = string.Empty;

                var rptViewer = new ReportViewer();
                rptViewer.Visible = false;
                rptViewer.Reset();
                rptViewer.LocalReport.DisplayName = "Student Information";
                rptViewer.ProcessingMode = ProcessingMode.Local;
                rptViewer.LocalReport.ReportPath = reportPath;
                rptViewer.LocalReport.EnableExternalImages = true;
                rptViewer.LocalReport.DataSources.Add(new ReportDataSource(bind_dsStudentReport.Trim(),reportData));
                rptViewer.LocalReport.DataSources.Add(new ReportDataSource(bind_dsStudentResult.Trim(),OLevelDetail));

                byte[] bytes = rptViewer.LocalReport.Render("PDF",null,out mimeType,out encoding,out extension,
                    out streamIds,out warnings);

                string path = Server.MapPath("~/Content/studentReportFolder");
                string savelocation = "";

                savelocation = Path.Combine(path,"StudentInformation.pdf");

                File.WriteAllBytes(savelocation,bytes);

                var urlHelp = new UrlHelper(HttpContext.Current.Request.RequestContext);
                Response.Redirect(
                    urlHelp.Action("DownloadReport",
                        new
                        {
                            controller = "Report",
                            area = "Student",
                            path = "~/Content/studentReportFolder/StudentInformation.pdf"
                        }),false);
                //return File(Server.MapPath(savelocation), "application/zip", reportData.FirstOrDefault().Fullname.Replace(" ", "") + ".zip");
                //Response.Redirect(savelocation, false);
            }
            catch(Exception ex)
            {
                lblMessage.Text = ex.Message + ex.InnerException.Message;
            }
        }
    }
}