using Abundance_Nk.Business;
using Abundance_Nk.Model.Model;
using Abundance_Nk.Web.Models;
using Ionic.Zip;
using Microsoft.Reporting.WebForms;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;

namespace Abundance_Nk.Web.Reports.Presenter
{
    public partial class ApplicantInformationReportBulk : System.Web.UI.Page
    {
        private List<Department> departments;

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {

                lblMessage.Text = "";
                if (!IsPostBack)
                {
                    Utility.BindDropdownItem(ddlSession, Utility.GetAllSessions(), Utility.ID, Utility.NAME);

                    Utility.BindDropdownItem(ddlProgramme, Utility.GetAllProgrammes(), Utility.ID, Utility.NAME);


                    ddlDepartment.Visible = false;
                }
            }
            catch (Exception ex)
            {
                lblMessage.Text = ex.Message + ex.InnerException.Message;
            }
        }
        public Session SelectedSession
        {
            get
            {
                return new Session { Id = Convert.ToInt32(ddlSession.SelectedValue), Name = ddlSession.SelectedItem.Text };
            }
            set { ddlSession.SelectedValue = value.Id.ToString(); }
        }


        public Programme SelectedProgramme
        {
            get
            {
                return new Programme
                {
                    Id = Convert.ToInt32(ddlProgramme.SelectedValue),
                    Name = ddlProgramme.SelectedItem.Text
                };
            }
            set { ddlProgramme.SelectedValue = value.Id.ToString(); }
        }

        public Department SelectedDepartment
        {
            get
            {
                return new Department
                {
                    Id = Convert.ToInt32(ddlDepartment.SelectedValue),
                    Name = ddlDepartment.SelectedItem.Text
                };
            }
            set { ddlDepartment.SelectedValue = value.Id.ToString(); }
        }
        private void BuildReport(Programme programme,Department department, Session session)
        {
            try
            {
                StudentLevelLogic studentLevelLogic = new StudentLevelLogic();
                var studentList= studentLevelLogic.GetModelsBy(f => f.Programme_Id == programme.Id && f.Department_Id==department.Id && f.Level_Id==1 && f.Session_Id==session.Id);
                if(studentList != null && studentList.Count > 0)
                {

                    string downloadPath = "~/Content/studentReportFolder" + DateTime.Now.Hour + DateTime.Now.Minute + DateTime.Now.Second + DateTime.Now.Millisecond;
                    if (Directory.Exists(Server.MapPath(downloadPath)))
                    {
                        Directory.Delete(Server.MapPath(downloadPath), true);
                        Directory.CreateDirectory(Server.MapPath(downloadPath));
                    }
                    else
                    {
                        Directory.CreateDirectory(Server.MapPath(downloadPath));
                    }
                    for (int i=0; i<studentList.Count; i++)
                    {
                        
                        var studentId = studentList[i].Student.Id;
                        var studentName = studentList[i].Student.FullName;
                        var person = new Person { Id = studentId };
                        var studentLogic = new StudentLogic();
                        var reportData = new List<Model.Model.StudentReport>();
                        var OLevelDetail = new List<Model.Model.StudentReport>();

                        //OLevelDetail = studentLogic.GetStudentOLevelDetail(person);

                        reportData = studentLogic.GetStudentBiodata(person);

                        string bind_dsStudentReport = "dsStudentReport";
                        //string bind_dsStudentResult = "dsStudentResult";
                        string reportPath = "";
                        //if (type == 1)
                        //{
                        //    reportPath = @"Reports\AbsuCertificateOfEligibility.rdlc";
                        //}
                        //if (type == 2)
                        //{
                        //    reportPath = @"Reports\AbsuCheckingCredentials.rdlc";
                        //}
                        //if (type == 3)
                        //{
                            reportPath = @"Reports\AbsuPersonalInfo.rdlc";
                        //}
                        //if (type == 4)
                        //{
                        //    reportPath = @"Reports\AbsuUnderTaking.rdlc";
                        //}

                       

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
                        rptViewer.LocalReport.DataSources.Add(new ReportDataSource(bind_dsStudentReport.Trim(), reportData));
                        //rptViewer.LocalReport.DataSources.Add(new ReportDataSource(bind_dsStudentResult.Trim(), OLevelDetail));

                        byte[] bytes = rptViewer.LocalReport.Render("PDF", null, out mimeType, out encoding, out extension,
                            out streamIds, out warnings);

                        string path = Server.MapPath(downloadPath);
                        string savelocation = "";
                        
                        savelocation = Path.Combine(path, studentName + ".pdf");


                        File.WriteAllBytes(savelocation, bytes);

                        //var urlHelp = new UrlHelper(HttpContext.Current.Request.RequestContext);
                        //Response.Redirect(
                        //    urlHelp.Action("DownloadReport",
                        //        new
                        //        {
                        //            controller = "Report",
                        //            area = "Student",
                        //            path = "~/Content/studentReportFolder/StudentInformation.pdf"
                        //        }), false);
                    }
                    using (var zip = new ZipFile())
                    {
                        string file = Server.MapPath(downloadPath);
                        zip.AddDirectory(file, "");
                        string zipFileName = department.Name.Replace('/', '_');
                        zip.Save(file + zipFileName + ".zip");
                        string export = downloadPath + zipFileName + ".zip";

                        //Response.Redirect(export, false);
                        //var urlHelp = new UrlHelper(HttpContext.Current.Request.RequestContext);
                        Response.Redirect(export, false);
                        //Response.Redirect(
                        //    urlHelp.Action("DownloadApplicantInfornationReportZip",
                        //        new { controller = "Result", area = "Admin", downloadName = department.Name }), false);
                    }
                }
                
                //return File(Server.MapPath(savelocation), "application/zip", reportData.FirstOrDefault().Fullname.Replace(" ", "") + ".zip");
                //Response.Redirect(savelocation, false);
            }
            catch (Exception ex)
            {
                lblMessage.Text = ex.Message + ex.InnerException.Message;
            }
        }
        protected void ddlProgramme_SelectedIndexChanged1(object sender, EventArgs e)
        {
            try
            {
                var programme = new Programme { Id = Convert.ToInt32(ddlProgramme.SelectedValue) };
                var departmentLogic = new DepartmentLogic();
                departments = departmentLogic.GetBy(programme);
                Utility.BindDropdownItem(ddlDepartment, departments, Utility.ID, Utility.NAME);
                ddlDepartment.Visible = true;
            }
            catch (Exception)
            {
                throw;
            }
        }
        protected void Display_Button_Click1(object sender, EventArgs e)
        {
            Session session = SelectedSession;
            Programme programme = SelectedProgramme;
            Department department = SelectedDepartment;
            BuildReport(programme, department, session);
        }
        }
}