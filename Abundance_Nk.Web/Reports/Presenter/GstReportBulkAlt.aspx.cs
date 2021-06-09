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
    public partial class GstReportAlt : System.Web.UI.Page
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
                return new Session { Id = Convert.ToInt32(ddlSession.SelectedValue), Name = ddlSession.SelectedItem.Text };
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

        public Faculty SelectedFaculty
        {
            get
            {
                return new Faculty
                {
                    Id = Convert.ToInt32(ddlFaculty.SelectedValue),
                    Name = ddlFaculty.SelectedItem.Text
                };
            }
            set { ddlFaculty.SelectedValue = value.Id.ToString(); }
        }

        public Level SelectedLevel
        {
            get { return new Level { Id = Convert.ToInt32(ddlLevel.SelectedValue), Name = ddlLevel.SelectedItem.Text }; }
            set { ddlLevel.SelectedValue = value.Id.ToString(); }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                lblMessage.Text = "";
                if (!IsPostBack)
                {
                    Utility.BindDropdownItem(ddlSession, Utility.GetAllSessions(), Utility.ID, Utility.NAME);
                    Utility.BindDropdownItem(ddlLevel, Utility.GetAllLevels(), Utility.ID, Utility.NAME);
                    Utility.BindDropdownItem(ddlFaculty, Utility.GetAllFaculties(), Utility.ID, Utility.NAME);

                    Display_Button.Visible = true;
                    ddlSemester.Visible = true;
                }
            }
            catch (Exception ex)
            {
                lblMessage.Text = ex.Message + ex.InnerException.Message;
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

                string directoryName = "gst_" + DateTime.Now.ToString().Replace("/", "").Replace(":", "").Replace(" ", "");
                string directoryPath = "~/Content/" + directoryName;
                if (Directory.Exists(Server.MapPath(directoryPath)))
                {
                    Directory.Delete(Server.MapPath(directoryPath), true);
                }

                Directory.CreateDirectory(Server.MapPath(directoryPath));
                List<Course> courses = new List<Course>();

                var courseLogic = new CourseLogic();
                var programmeLogic = new ProgrammeLogic();
                var departmentLogic = new DepartmentLogic();
                var levelLogic = new LevelLogic();
                Semester seSemester = new Semester() { Id = SelectedSemester.Id, Name = SelectedSemester.Name };
                Session seSession = new Session() { Id = SelectedSession.Id, Name = SelectedSession.Name };
                var scanCourses = db.GST_SCAN_RESULT.Where(x =>x.Semester_Id== seSemester.Id && x.Session_Id== seSession.Id).Select(y=>y.COURSE_CODE).Distinct().ToList();
                //var scanCourses = db.GST_SCAN_RESULT.Select(x => x.COURSE_CODE).Distinct().ToList();


                //var scanCourses = new List<Course>()
                //{
                //new Course { Id = 0, Name = "GST 101" },
                //new Course { Id = 1, Name = "GST 103" },
                //new Course { Id = 2, Name = "GST 105" },
                //new Course { Id = 3, Name = "GST 121" }
                //};
                if (scanCourses == null)
                {
                    lblMessage.Text = "No Course Upload For the selected session and semester";
                    return;
                }
                for (int i = 0; i < scanCourses.Count; i++)
                {
                    Course course = new Course();
                    course.Id = i;
                    course.Name = scanCourses[i];
                    if (course.Name != null && !course.Name.Contains("ENT"))
                    {
                        courses.Add(course);
                    }
                }

                var departments = departmentLogic.GetBy(new Faculty { Id = SelectedFaculty.Id });
                List<Course> courseList = courses;
                
                // Semester seSemester = new Semester() { Id = SelectedSemester.Id, Name = SelectedSemester.Name };
                Faculty faculty = new Faculty() { Id = SelectedFaculty.Id };
                Level level = new Level() { Id = SelectedLevel.Id, Name = SelectedLevel.Name };

                foreach (Department deptDepartment in departments)
                {
                    foreach (Course coCourse in courseList)
                    {
                        var studentResultLogic = new StudentResultLogic();
                        List<Model.Model.Result> report = studentResultLogic.GetStudentGSTResult(seSession, seSemester, level, faculty, deptDepartment, coCourse);
                        if (report.Count > 0)
                        {

                            string bind_dsStudentResultSummary = "dsExamRawScoreSheet";
                            string reportPath = @"Reports\Result\ExamRawScoreSheetGST.rdlc";

                            var rptViewer = new ReportViewer();
                            rptViewer.Visible = false;
                            rptViewer.Reset();
                            rptViewer.LocalReport.DisplayName = "GST Result";
                            rptViewer.ProcessingMode = ProcessingMode.Local;
                            rptViewer.LocalReport.ReportPath = reportPath;
                            rptViewer.LocalReport.DataSources.Add(new ReportDataSource(bind_dsStudentResultSummary.Trim(), report));

                            byte[] bytes = rptViewer.LocalReport.Render("PDF", null, out mimeType, out encoding,
                            out extension, out streamIds, out warnings);

                            string name = deptDepartment.Code.Replace(" ", "") + coCourse.Name.Replace(" ", "").Replace("/", "").Replace("-", "") + level.Name.Replace(" ", "");
                            string path = Server.MapPath(directoryPath);
                            string savelocation = Path.Combine(path, name + ".pdf");
                            File.WriteAllBytes(savelocation, bytes);
                        }
                    }

                }

                using (var zip = new ZipFile())
                {
                    string file = Server.MapPath(directoryPath);
                    zip.AddDirectory(file, "");
                    string zipFileName = SelectedFaculty.Name + "_" + SelectedLevel.Name;
                    zip.Save(file + zipFileName + ".zip");
                    string export = directoryPath + zipFileName + ".zip";

                    Response.Redirect(export, false);
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        protected void Display_Button_Click1(object sender, EventArgs e)
        {
            try
            {
                if (InvalidUserInput())
                {
                    lblMessage.Text = "All fields must be selected";
                    return;
                }

                BulkOperations();
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
                if (SelectedSession == null || SelectedSession.Id <= 0 || SelectedSemester == null || SelectedSemester.Id <= 0 || SelectedFaculty == null || SelectedFaculty.Id <= 0 || SelectedLevel == null || SelectedLevel.Id <= 0)
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

        protected void ddlSession_SelectedIndexChanged1(object sender, EventArgs e)
        {
            var session = new Session { Id = Convert.ToInt32(ddlSession.SelectedValue) };
            var sessionSemesterLogic = new SessionSemesterLogic();

            List<SessionSemester> semesterList = sessionSemesterLogic.GetModelsBy(p => p.Session_Id == session.Id);

            foreach (SessionSemester sessionSemester in semesterList)
            {
                semesters.Add(sessionSemester.Semester);
            }

            semesters.Insert(0, new Semester { Name = "-- Select Semester--" });
            Utility.BindDropdownItem(ddlSemester, semesters, Utility.ID, Utility.NAME);
            ddlSemester.Visible = true;
        }

    }
}
