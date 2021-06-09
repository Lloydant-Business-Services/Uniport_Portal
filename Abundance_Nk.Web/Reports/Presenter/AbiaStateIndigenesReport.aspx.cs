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
using System.Web.UI.WebControls;

namespace Abundance_Nk.Web.Reports.Presenter
{
    public partial class AbiaStateIndigenesReport : System.Web.UI.Page
    {
        private List<Department> departments;

        public Session SelectedSession
        {
            get
            {
                return new Session { Id = Convert.ToInt32(ddlSession.SelectedValue), Name = ddlSession.SelectedItem.Text };
            }
            set { ddlSession.SelectedValue = value.Id.ToString(); }
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
        public ReportViewer Viewer
        {
            get { return rv; }
            set { rv = value; }
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                lblMessage.Text = "";

                if (!IsPostBack)
                {
                    Utility.BindDropdownItem(ddlSession, Utility.GetAllSessions(), Utility.ID, Utility.NAME);
                    Utility.BindDropdownItem(ddlDepartment, Utility.GetAllDepartments(), Utility.ID, Utility.NAME);
                }
            }
            catch (Exception ex)
            {
                lblMessage.Text = ex.Message + ex.InnerException.Message;
            }
        }

        private bool InvalidUserInput()
        {
            try
            {
                if (SelectedDepartment == null || SelectedDepartment.Id <= 0 || SelectedSession == null || SelectedSession.Id <= 0)
                {
                    return true;
                }

                return false;
            }
            catch (Exception)
            {
                throw;
            }
        }
        private bool InvalidUserInputForBulk()
        {
            try
            {
                if (SelectedSession == null || SelectedSession.Id <= 0)
                {
                    return true;
                }

                return false;
            }
            catch (Exception)
            {
                throw;
            }
        }

        private void DisplayReportBy(Department department, Session session)
        {


            try
            {
                var studentLogic = new StudentLogic();
                List<Model.Model.Result> report = studentLogic.GetAbuaStateIndigenesRecord(session,department.Id );

                string bind_dsMasterSheet = "dsMasterSheet";
                string reportPath = "";
                reportPath = @"Reports\IndigeneReport.rdlc";

                rv.Reset();
                rv.LocalReport.DisplayName = "List Of Indigene For" + session.Name + " Session";
                rv.LocalReport.ReportPath = reportPath;

                if (report != null)
                {
                    report.FirstOrDefault().SessionName = session.Name;
                    report.FirstOrDefault().DepartmentName = department.Name;
                    rv.ProcessingMode = ProcessingMode.Local;
                    rv.LocalReport.DataSources.Add(new ReportDataSource(bind_dsMasterSheet.Trim(),
                        report));
                    rv.LocalReport.Refresh();
                }
            }
            catch (Exception ex)
            {
                lblMessage.Text = ex.Message + ex.InnerException.Message;

            }



        }


        protected void btnDisplayReport_Click(object sender, EventArgs e)
        {
            try
            {
                if (InvalidUserInput())
                {
                    lblMessage.Text = "All fields must be selected";
                    return;
                }
                DisplayReportBy(SelectedDepartment, SelectedSession);
            }
            catch (Exception ex)
            {
                lblMessage.Text = ex.Message + ex.InnerException.Message;

            }
        }
        protected void btnBulkReport_Click(object sender, EventArgs e)
        {
            try
            {
                if (InvalidUserInputForBulk())
                {
                    lblMessage.Text = "You are required to select session";
                    return;
                }
                BulkOperations(SelectedSession);
            }
            catch (Exception ex)
            {
                lblMessage.Text = ex.Message + ex.InnerException.Message;
            }
        }
        public void BulkOperations(Session session)
        {
            try
            {
                Warning[] warnings;
                string[] streamIds;
                string mimeType = string.Empty;
                string encoding = string.Empty;
                string extension = string.Empty;

                string downloadPath = "/Content/tempIndigene/";
                string downloadPathZip = "/Content/tempIndigene/IndigeneDetailReport_" + SelectedSession.Name.Replace("/", "_") + ".zip";

                if (Directory.Exists(Server.MapPath(downloadPath)))
                {
                    Directory.Delete(Server.MapPath(downloadPath), true);
                    Directory.CreateDirectory(Server.MapPath(downloadPath));
                }
                else
                {
                    Directory.CreateDirectory(Server.MapPath(downloadPath));
                }
                if (Directory.Exists(Server.MapPath(downloadPathZip)))
                {
                    Directory.Delete(Server.MapPath(downloadPath), true);
                }

                var departmentLogic = new DepartmentLogic();

                List<Department> departments = departmentLogic.GetAll();

                foreach (Department deptDepartment in departments)
                {

                    var studentLogic = new StudentLogic();
                    List<Model.Model.Result> report = studentLogic.GetAbuaStateIndigenesRecord(session, deptDepartment.Id);

                    if (report.Count > 0)
                    {


                        string bind_dsMasterSheet = "dsMasterSheet";
                        string reportPath = "";
                        reportPath = @"Reports\IndigeneReport.rdlc";


                        rv.Reset();
                        rv.LocalReport.DisplayName = "List Of Indigene For" + session.Name + " Session";
                        rv.LocalReport.ReportPath = reportPath;

                        if (report != null)
                        {
                            report.FirstOrDefault().SessionName = session.Name;
                            report.FirstOrDefault().DepartmentName = deptDepartment.Name;
                            rv.ProcessingMode = ProcessingMode.Local;
                            rv.LocalReport.DataSources.Add(new ReportDataSource(bind_dsMasterSheet.Trim(),
                                report));
                            rv.LocalReport.Refresh();
                        }


                        byte[] bytes = rv.LocalReport.Render("PDF", null, out mimeType, out encoding,
                        out extension, out streamIds, out warnings);

                        string name = deptDepartment.Code.Replace(" ", "") + "_" + SelectedSession.Name.Replace("/", "_");
                        string path = Server.MapPath(downloadPath);
                        string savelocation = Path.Combine(path, name + ".pdf");
                        if (File.Exists(savelocation))
                        {
                            File.Delete(savelocation);
                        }
                        File.WriteAllBytes(savelocation, bytes);
                    }

                }

                using (var zip = new ZipFile())
                {
                    string file = Server.MapPath(downloadPath);
                    zip.AddDirectory(file, "");
                    string zipFileName = "IndigeneDetailReport_" + SelectedSession.Name.Replace("/", "_");
                    zip.Save(file + zipFileName + ".zip");
                    string export = downloadPath + zipFileName + ".zip";

                    //Response.Redirect(export, false);
                    var urlHelp = new UrlHelper(HttpContext.Current.Request.RequestContext);
                    Response.Redirect(
                        urlHelp.Action("DownloadIndigeneZip",
                            new
                            {
                                controller = "StaffCourseAllocation",
                                area = "Admin",
                                downloadName = zipFileName
                            }), false);
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
    }
}