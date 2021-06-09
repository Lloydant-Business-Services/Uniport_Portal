using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;
using System.Web.UI.WebControls;
using Abundance_Nk.Business;
using Abundance_Nk.Model.Entity;
using Abundance_Nk.Model.Model;
using Abundance_Nk.Web.Models;
using Ionic.Zip;
using Microsoft.Reporting.WebForms;

namespace Abundance_Nk.Web.Reports.Presenter
{
    public partial class GstReport :System.Web.UI.Page
    {
        
        private Abundance_NkEntities db = new Abundance_NkEntities();
        private readonly List<Semester> semesters = new List<Semester>();
        private Course course;
        private string courseId;
        private List<Course> courses;
        private Department department;
        private List<Department> departments;
        private string deptId;
        private Level level;
        private string levelId;
        private string progId;
        private Programme programme;
        private Semester semester;
        private string semesterId;
        private Session session;
        private string sessionId;

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

        public Course SelectedCourse
        {
            get
            {
                return new Course { Id = Convert.ToInt32(ddlCourse.SelectedValue),Name = ddlCourse.SelectedItem.Text };
            }
            set { ddlCourse.SelectedValue = value.Id.ToString(); }
        }

        protected void Page_Load(object sender,EventArgs e)
        {
            try
            {
                lblMessage.Text = "";

                if(Request.QueryString["levelId"] != null && Request.QueryString["semesterId"] != null &&
                    Request.QueryString["progId"] != null && Request.QueryString["deptId"] != null &&
                    Request.QueryString["sessionId"] != null && Request.QueryString["courseId"] != null)
                {
                    levelId = Request.QueryString["levelId"];
                    semesterId = Request.QueryString["semesterId"];
                    progId = Request.QueryString["progId"];
                    deptId = Request.QueryString["deptId"];
                    sessionId = Request.QueryString["sessionId"];
                    courseId = Request.QueryString["courseId"];
                    int deptId1 = Convert.ToInt32(deptId);
                    int progId1 = Convert.ToInt32(progId);
                    int sessionId1 = Convert.ToInt32(sessionId);
                    int levelId1 = Convert.ToInt32(levelId);
                    int semesterId1 = Convert.ToInt32(semesterId);
                    int courseId1 = Convert.ToInt32(courseId);
                    session = new Session { Id = sessionId1 };
                    semester = new Semester { Id = semesterId1 };
                    course = new Course { Id = courseId1 };
                    department = new Department { Id = deptId1 };
                    level = new Level { Id = levelId1 };
                    programme = new Programme { Id = progId1 };
                }
                else
                {
                    Display_Button.Visible = true;
                }

              
                if(!IsPostBack)
                {
                    if(string.IsNullOrEmpty(deptId) || string.IsNullOrEmpty(progId) || string.IsNullOrEmpty(sessionId))
                    {
                        Utility.BindDropdownItem(ddlSession, Utility.GetAllSessions(), Utility.ID, Utility.NAME);
                        ddlSemester.Visible = false;
                        Utility.BindDropdownItem(ddlProgramme, Utility.GetAllProgrammes(), Utility.ID, Utility.NAME);
                        Utility.BindDropdownItem(ddlLevel, Utility.GetAllLevels(), Utility.ID, Utility.NAME);

                        ddlDepartment.Visible = false;
                        ddlCourse.Visible = false;
                        //BulkOperations();

                    }
                    else
                    {
                        DisplayStaffReport(session,semester,course,department,level,programme);
                        ddlDepartment.Visible = false;
                        ddlCourse.Visible = false;
                        ddlLevel.Visible = false;
                        ddlProgramme.Visible = false;
                        ddlSemester.Visible = false;
                        ddlSession.Visible = false;
                    }
                }
            }
            catch(Exception ex)
            {
                lblMessage.Text = ex.Message + ex.InnerException.Message;
            }
        }

        private void DisplayStaffReport(Session session,Semester semester,Course course,Department department,
            Level level,Programme programme)
        {
            try
            {
                DisplayReportBy(session,semester,course,department,level,programme);
            }
            catch(Exception ex)
            {
                lblMessage.Text = ex.Message + ex.InnerException.Message;
                ;
            }
        }

        public void BulkOperations()
        {
            try
            {
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
                List<Course> courses = new List<Course>();

                var courseLogic = new CourseLogic();
                var programmeLogic = new ProgrammeLogic();
                var departmentLogic = new DepartmentLogic();
                var levelLogic = new LevelLogic();

                var scanCourses = db.GST_SCAN_RESULT.Select(x => x.COURSE_CODE).Distinct().ToList();
                for (int i =0; i < scanCourses.Count; i++)
                {
                    Course course = new Course();
                    course.Id = i;
                    course.Name = scanCourses[i];
                    courses.Add(course);
                }

                List<Level> levels = levelLogic.GetAll();
                List<Department> departments = departmentLogic.GetAll();
                List<Programme> programmes = programmeLogic.GetAll();
                List<Course> courseList = courses;
                Model.Model.Session seSession = new Session(){Id = 7,Name = "2016/2017"};
                Semester seSemester = new Semester(){Id = 2};
                Programme programme = new Programme(){Id = 4};
                //Course coCourse =  new Course(){Id=0,Name = "GST 107"};
               // foreach(Programme prProgramme in programmes)
               // {
                    foreach (Department deptDepartment in departments)
                    {
                        foreach (Level levlevel in levels)
                        {
                            foreach (Course coCourse in courseList)
                            {
                                    var studentResultLogic = new StudentResultLogic();
                                    List<Model.Model.Result> report = studentResultLogic.GetStudentGSTResultBy(seSession,seSemester,levlevel,
                                            programme,deptDepartment,coCourse);
                                    if(report.Count > 0)
                                    {
                                        
                                        string bind_dsStudentResultSummary = "dsExamRawScoreSheet";

                                        string reportPath = null;

                                        if (coCourse.Name.Contains("ENT"))
                                        {
                                            reportPath = @"Reports\Result\ExamRawScoreSheetENT.rdlc";
                                        }
                                        else
                                        {
                                            reportPath = @"Reports\Result\ExamRawScoreSheetGST.rdlc";
                                        }

                                        var rptViewer = new ReportViewer();
                                        rptViewer.Visible = false;
                                        rptViewer.Reset();
                                        rptViewer.LocalReport.DisplayName = "GST Result";
                                        rptViewer.ProcessingMode = ProcessingMode.Local;
                                        rptViewer.LocalReport.ReportPath = reportPath;
                                        rptViewer.LocalReport.DataSources.Add(new ReportDataSource(bind_dsStudentResultSummary.Trim(),report));

                                        byte[] bytes = rptViewer.LocalReport.Render("PDF",null,out mimeType,out encoding,
                                        out extension,out streamIds,out warnings);

                                        string name = deptDepartment.Code.Replace(" ","") + coCourse.Name.Replace(" ","")+ levlevel.Name.Replace(" ","") ;
                                        string path = Server.MapPath("~/Content/temp");
                                        string savelocation = Path.Combine(path,name + ".pdf");
                                        File.WriteAllBytes(savelocation,bytes);
                                    }
                            }
                        }
                    }
                 
               // }
                //using(var zip = new ZipFile())
                //{
                //    string file = Server.MapPath("~/Content/temp/");
                //    zip.AddDirectory(file,"");
                //    string zipFileName = SelectedDepartment.Name;
                //    zip.Save(file + zipFileName + ".zip");
                //    string export = "~/Content/temp/" + zipFileName + ".zip";

                //    //Response.Redirect(export, false);
                //    var urlHelp = new UrlHelper(HttpContext.Current.Request.RequestContext);
                //    Response.Redirect(
                //        urlHelp.Action("DownloadZip",
                //            new
                //            {
                //                controller = "StaffCourseAllocation",
                //                area = "Admin",
                //                downloadName = SelectedDepartment.Name
                //            }),false);
                //}
            }
            catch (Exception ex)
            {
                
                throw ex;
            }
        }

        protected void Display_Button_Click1(object sender,EventArgs e)
        {
            try
            {
                if(InvalidUserInput())
                {
                    lblMessage.Text = "All fields must be selected";
                    return;
                }

                DisplayReportBy(SelectedSession,SelectedSemester,SelectedCourse,SelectedDepartment,SelectedLevel,
                    SelectedProgramme);

                
            }
            catch(Exception ex)
            {
                lblMessage.Text = ex.Message + ex.InnerException.Message;
                ;
            }
        }

        private bool InvalidUserInput()
        {
            try
            {
                if(SelectedSession == null || SelectedSession.Id <= 0 || SelectedDepartment == null ||
                    SelectedDepartment.Id <= 0 || SelectedProgramme == null || SelectedProgramme.Id <= 0)
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

        private void DisplayReportBy(Session session,Semester semester,Course course,Department department,
            Level level,Programme programme)
        {
            try
            {
                var studentResultLogic = new StudentResultLogic();
                List<Model.Model.Result> studeResults = studentResultLogic.GetStudentGSTResultBy(session,semester,level,
                    programme,department,course);
                string bind_dsStudentResultSummary = "dsExamRawScoreSheet";
                string reportPath = null;
                if (course.Name.Contains("ENT"))
                {
                    reportPath = @"Reports\Result\ExamRawScoreSheetENT.rdlc";
                }
                else
                {
                    reportPath = @"Reports\Result\ExamRawScoreSheetGST.rdlc";
                }

                ReportViewer1.Reset();
                ReportViewer1.LocalReport.DisplayName = "Examination Raw Score Sheet ";
                ReportViewer1.LocalReport.ReportPath = reportPath;

                if(studeResults != null)
                {
                    ReportViewer1.ProcessingMode = ProcessingMode.Local;
                    ReportViewer1.LocalReport.DataSources.Add(new ReportDataSource(bind_dsStudentResultSummary.Trim(),
                        studeResults));
                    ReportViewer1.LocalReport.Refresh();
                    ReportViewer1.Visible = true;
                }
            }
            catch(Exception ex)
            {
                lblMessage.Text = ex.Message + ex.InnerException.Message;
            }
        }

        protected void ddlProgramme_SelectedIndexChanged1(object sender,EventArgs e)
        {
            var programme = new Programme { Id = Convert.ToInt32(ddlProgramme.SelectedValue) };
            var departmentLogic = new DepartmentLogic();
            departments = departmentLogic.GetBy(programme);
            departments.Insert(0,new Department { Name = "-- Select Department--" });
            Utility.BindDropdownItem(ddlDepartment,departments,Utility.ID,Utility.NAME);

            ddlDepartment.Visible = true;
        }

        protected void ddlSession_SelectedIndexChanged1(object sender,EventArgs e)
        {
            var session = new Session { Id = Convert.ToInt32(ddlSession.SelectedValue) };
            var sessionSemesterLogic = new SessionSemesterLogic();
            List<SessionSemester> semesterList = sessionSemesterLogic.GetModelsBy(p => p.Session_Id == session.Id);
            foreach(SessionSemester sessionSemester in semesterList)
            {
                semesters.Add(sessionSemester.Semester);
            }
            semesters.Insert(0,new Semester { Name = "-- Select Semester--" });
            Utility.BindDropdownItem(ddlSemester,semesters,Utility.ID,Utility.NAME);
            ddlSemester.Visible = true;
        }

        protected void ddlDepartment_SelectedIndexChanged(object sender,EventArgs e)
        {
            try
            {
                courses = new List<Course>();
                var courseLogic = new CourseLogic();
                var scanCourses = db.GST_SCAN_RESULT.Select(x => x.COURSE_CODE).Distinct().ToList();
                for (int i =0; i < scanCourses.Count; i++)
                {
                    Course course = new Course();
                    course.Id = i;
                    course.Name = scanCourses[i];
                    courses.Add(course);
                }
               
                Utility.BindDropdownItem(ddlCourse,courses,Utility.ID,Utility.NAME);
                ddlCourse.Visible = true;
            }
            catch(Exception ex)
            {
                lblMessage.Text = ex.Message + ex.InnerException.Message;
            }
        }
    }
}
