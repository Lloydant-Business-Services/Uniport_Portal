using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;
using System.Web.UI.WebControls;
using Abundance_Nk.Business;
using Abundance_Nk.Model.Model;
using Abundance_Nk.Web.Models;
using Ionic.Zip;
using Microsoft.Reporting.WebForms;

namespace Abundance_Nk.Web.Reports.Presenter
{
    public partial class GstSlugData :System.Web.UI.Page
    {
         private List<Department> departments;
        private List<Semester> semesters;
        private List<SessionSemester> sessionSemesterList;

        public Session SelectedSession
        {
            get
            {
                return new Session { Id = Convert.ToInt32(ddlSession.SelectedValue),Name = ddlSession.SelectedItem.Text };
            }
            set { ddlSession.SelectedValue = value.Id.ToString(); }
        }

        public Semester SelectedSemester
        {
            get
            {
                return new Semester
                {
                    Id = Convert.ToInt32(ddlSemester.SelectedValue),
                    Name = ddlSemester.SelectedItem.Text
                };
            }
            set { ddlSemester.SelectedValue = value.Id.ToString(); }
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

        public Level SelectedLevel
        {
            get { return new Level { Id = Convert.ToInt32(ddlLevel.SelectedValue),Name = ddlLevel.SelectedItem.Text }; }
            set { ddlLevel.SelectedValue = value.Id.ToString(); }
        }

        protected void Page_Load(object sender,EventArgs e)
        {
             try
            {
                lblMessage.Text = "";

                if(!IsPostBack)
                {
                    Utility.BindDropdownItem(ddlSession,Utility.GetAllSessions(),Utility.ID,Utility.NAME);

                    Utility.BindDropdownItem(ddlProgramme,Utility.GetAllProgrammes(),Utility.ID,Utility.NAME);

                    Utility.BindDropdownItem(ddlLevel,Utility.GetAllLevels(),Utility.ID,Utility.NAME);

                    ddlDepartment.Visible = false;
                    ddlSemester.Visible = false;
                }
            }
            catch(Exception ex)
            {
                lblMessage.Text = ex.Message + ex.InnerException.Message;
            }
        }
    
        
        private bool InvalidUserInput()
        {
            try
            {
                if(SelectedSession == null || SelectedSession.Id <= 0 || SelectedSemester == null ||
                    SelectedSemester.Id <= 0 || SelectedDepartment == null || SelectedDepartment.Id <= 0 ||
                    SelectedProgramme == null || SelectedProgramme.Id <= 0 || SelectedLevel == null ||
                    SelectedLevel.Id <= 0)
                {
                    return true;
                }

                return false;
            }
            catch(Exception)
            {
                throw;
            }
        }

        protected void ddlProgramme_SelectedIndexChanged1(object sender,EventArgs e)
        {
            try
            {
                var programme = new Programme { Id = Convert.ToInt32(ddlProgramme.SelectedValue) };
                var departmentLogic = new DepartmentLogic();
                departments = departmentLogic.GetBy(programme);
                Utility.BindDropdownItem(ddlDepartment,departments,Utility.ID,Utility.NAME);
                ddlDepartment.Visible = true;
            }
            catch(Exception)
            {
                throw;
            }
        }

        protected void Display_Button_Click1(object sender,EventArgs e)
        {
            try
            {
                Session session = SelectedSession;
                Semester semester = SelectedSemester;
                Programme programme = SelectedProgramme;
                Department department = SelectedDepartment;
                Level level = SelectedLevel;

                if(InvalidUserInput())
                {
                    lblMessage.Text = "All fields must be selected";
                    return;
                }

                Warning[] warnings;
                string[] streamIds;
                string mimeType = string.Empty;
                string encoding = string.Empty;
                string extension = string.Empty;

                if(Directory.Exists(Server.MapPath("~/Content/temp")))
                {
                    Directory.Delete(Server.MapPath("~/Content/temp"),true);
                }
                Directory.CreateDirectory(Server.MapPath("~/Content/temp"));

                List<Course> courseList = Utility.GetCoursesByLevelDepartmentAndSemester(level,department,semester,
                    programme);
                foreach(Course course in courseList)
                {
                    var courseRegistrationDetailLogic = new CourseRegistrationDetailLogic();
                    List<AttendanceFormat> report = courseRegistrationDetailLogic.GetCourseAttendanceSheet(session,
                        semester,programme,department,level,course);

                    if(report.Count > 0)
                    {
                        string bind_dsAttendanceList = "dsAttendanceList";
                        string reportPath = @"Reports\SlugData.rdlc";

                        var rptViewer = new ReportViewer();
                        rptViewer.Visible = false;
                        rptViewer.Reset();
                        rptViewer.LocalReport.DisplayName = "Attendance Sheet";
                        rptViewer.ProcessingMode = ProcessingMode.Local;
                        rptViewer.LocalReport.ReportPath = reportPath;
                        rptViewer.LocalReport.DataSources.Add(new ReportDataSource(bind_dsAttendanceList.Trim(),report));

                        byte[] bytes = rptViewer.LocalReport.Render("Excel",null,out mimeType,out encoding,
                            out extension,out streamIds,out warnings);

                        var courseLogic = new CourseLogic();
                        Course courseGenerated = courseLogic.GetBy(course.Id);
                        string path = Server.MapPath("~/Content/temp");
                        string savelocation = Path.Combine(path,courseGenerated.Code + ".xls");
                        File.WriteAllBytes(savelocation,bytes);
                    }
                }
                using(var zip = new ZipFile())
                {
                    string file = Server.MapPath("~/Content/temp/");
                    zip.AddDirectory(file,"");
                    string zipFileName = SelectedDepartment.Name;
                    zip.Save(file + zipFileName + ".zip");
                    string export = "~/Content/temp/" + zipFileName + ".zip";

                    //Response.Redirect(export, false);
                    var urlHelp = new UrlHelper(HttpContext.Current.Request.RequestContext);
                    Response.Redirect(
                        urlHelp.Action("DownloadZip",
                            new
                            {
                                controller = "StaffCourseAllocation",
                                area = "Admin",
                                downloadName = SelectedDepartment.Name
                            }),false);
                }
            }
            catch(Exception ex)
            {
                lblMessage.Text = ex.Message + ex.InnerException.Message;
            }
        }

        protected void ddlSession_SelectedIndexChanged(object sender,EventArgs e)
        {
            try
            {
                var session = new Session { Id = Convert.ToInt32(ddlSession.SelectedValue) };
                var semesterLogic = new SemesterLogic();
                var sessionSemesterLogic = new SessionSemesterLogic();
                sessionSemesterList = sessionSemesterLogic.GetModelsBy(p => p.Session_Id == session.Id);

                semesters = new List<Semester>();
                foreach(SessionSemester item in sessionSemesterList)
                {
                    semesters.Add(item.Semester);
                }
                Utility.BindDropdownItem(ddlSemester,semesters,Utility.ID,Utility.NAME);
                ddlSemester.Visible = true;
            }
            catch(Exception)
            {
                throw;
            }
        }
    
    }
}