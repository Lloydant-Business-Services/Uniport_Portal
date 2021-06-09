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

namespace Abundance_Nk.Web.Reports.Presenter.Result
{
    public partial class NotificationOfResultBulk :Page
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
                if(!Page.IsPostBack)
                {
                    Utility.BindDropdownItem(ddlSession,Utility.GetAllSessions(),Utility.ID,Utility.NAME);

                    Utility.BindDropdownItem(ddlProgramme,Utility.GetAllProgrammes(),Utility.ID,Utility.NAME);

                    Utility.BindDropdownItem(ddlLevel,Utility.GetAllLevels(),Utility.ID,Utility.NAME);

                    ddlDepartment.Visible = false;
                    ddlSemester.Visible = false;
                }
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

                if(InvalidUserInput(session,semester,department,level,programme))
                {
                    lblMessage.Text = "All fields must be selected";
                    return;
                }

                if(Directory.Exists(Server.MapPath("~/Content/temp")))
                {
                    Directory.Delete(Server.MapPath("~/Content/temp"),true);
                }
                else
                {
                    DirectoryInfo folder = Directory.CreateDirectory(Server.MapPath("~/Content/temp"));
                    int filesInFolder = folder.GetFiles().Count();
                    if(filesInFolder > 0)
                    {
                        //complete the code
                    }
                }

                List<Model.Model.Result> StudentList = GetResultList(session,semester,department,level,programme);
                foreach(Model.Model.Result item in StudentList)
                {
                    var student = new Student { Id = item.StudentId };

                    List<Model.Model.Result> resultList = GetReportList(semester,session,programme,department,level,
                        student);

                    Warning[] warnings;
                    string[] streamIds;
                    string mimeType = string.Empty;
                    string encoding = string.Empty;
                    string extension = string.Empty;

                    string bind_dsStudentPaymentSummary = "dsNotificationOfResult";
                    string reportPath = @"Reports\Result\NotificationOfResult.rdlc";

                    var rptViewer = new ReportViewer();
                    rptViewer.Visible = false;
                    rptViewer.Reset();
                    rptViewer.LocalReport.DisplayName = "Notification Of Result";
                    rptViewer.ProcessingMode = ProcessingMode.Local;
                    rptViewer.LocalReport.ReportPath = reportPath;
                    rptViewer.LocalReport.EnableExternalImages = true;
                    rptViewer.LocalReport.DataSources.Add(new ReportDataSource(bind_dsStudentPaymentSummary.Trim(),
                        resultList));

                    byte[] bytes = rptViewer.LocalReport.Render("PDF",null,out mimeType,out encoding,out extension,
                        out streamIds,out warnings);

                    string path = Server.MapPath("~/Content/temp");
                    string savelocation = Path.Combine(path,item.Name + ".pdf");
                    File.WriteAllBytes(savelocation,bytes);
                }
                using(var zip = new ZipFile())
                {
                    string file = Server.MapPath("~/Content/temp/");
                    zip.AddDirectory(file,"");
                    string zipFileName = department.Name;
                    zip.Save(file + zipFileName + ".zip");
                    string export = "~/Content/temp/" + zipFileName + ".zip";

                    //Response.Redirect(export, false);
                    var urlHelp = new UrlHelper(HttpContext.Current.Request.RequestContext);
                    Response.Redirect(
                        urlHelp.Action("DownloadZip",
                            new { controller = "Result",area = "Admin",downloadName = department.Name }),false);
                }
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

        private List<Model.Model.Result> GetReportList(Semester semester,Session session,Programme programme,
            Department department,Level level,Student student)
        {
            List<Model.Model.Result> resultList = null;
            var studentResultLogic = new StudentResultLogic();
            if(semester.Id == 1)
            {
                List<Model.Model.Result> result = null;
                result = studentResultLogic.GetStudentProcessedResultBy(session,level,department,student,semester,
                    programme);
                decimal? firstSemesterGPCUSum = result.Sum(p => p.GPCU);
                int? firstSemesterTotalSemesterCourseUnit = 0;
                var studentResultFirstSemester = new Model.Model.Result();
                studentResultFirstSemester = result.FirstOrDefault();
                firstSemesterTotalSemesterCourseUnit = studentResultFirstSemester.TotalSemesterCourseUnit;
                decimal? firstSemesterGPA = firstSemesterGPCUSum / firstSemesterTotalSemesterCourseUnit;
                studentResultFirstSemester.GPA = firstSemesterGPA;
                studentResultFirstSemester.CGPA = firstSemesterGPA;
                studentResultFirstSemester.StudentTypeName = GetGraduatingDegree(studentResultFirstSemester.ProgrammeId);
                studentResultFirstSemester.GraduationStatus = GetGraduationStatus(studentResultFirstSemester.CGPA);
                resultList = new List<Model.Model.Result>();
                resultList.Add(studentResultFirstSemester);
            }
            else
            {
                List<Model.Model.Result> result = null;
                var firstSemester = new Semester { Id = 1 };
                result = studentResultLogic.GetStudentProcessedResultBy(session,level,department,student,
                    firstSemester,programme);
                decimal? firstSemesterGPCUSum = result.Sum(p => p.GPCU);
                int? firstSemesterTotalSemesterCourseUnit = 0;
                var studentResultFirstSemester = new Model.Model.Result();
                studentResultFirstSemester = result.FirstOrDefault();

                firstSemesterTotalSemesterCourseUnit = studentResultFirstSemester.TotalSemesterCourseUnit;
                decimal? firstSemesterGPA = firstSemesterGPCUSum / firstSemesterTotalSemesterCourseUnit;
                studentResultFirstSemester.GPA = firstSemesterGPA;

                var secondSemester = new Semester { Id = 2 };
                var studentResultSecondSemester = new Model.Model.Result();
                List<Model.Model.Result> secondSemesterResultList =
                    studentResultLogic.GetStudentProcessedResultBy(session,level,department,student,secondSemester,
                        programme);
                decimal? secondSemesterGPCUSum = secondSemesterResultList.Sum(p => p.GPCU);
                studentResultSecondSemester = secondSemesterResultList.FirstOrDefault();

                studentResultSecondSemester.GPA =
                    Decimal.Round(
                        (decimal)(secondSemesterGPCUSum / studentResultSecondSemester.TotalSemesterCourseUnit),2);
                studentResultSecondSemester.CGPA =
                    Decimal.Round(
                        (decimal)
                            ((firstSemesterGPCUSum + secondSemesterGPCUSum) /
                             (studentResultSecondSemester.TotalSemesterCourseUnit + firstSemesterTotalSemesterCourseUnit)),
                        2);
                studentResultSecondSemester.StudentTypeName =
                    GetGraduatingDegree(studentResultSecondSemester.ProgrammeId);
                studentResultSecondSemester.GraduationStatus = GetGraduationStatus(studentResultSecondSemester.CGPA);
                resultList = new List<Model.Model.Result>();
                resultList.Add(studentResultSecondSemester);
            }
            return resultList;
        }

        private string GetGraduationStatus(decimal? CGPA)
        {
            string title = null;
            try
            {
                if(CGPA >= 3.5M && CGPA <= 4.0M)
                {
                    title = "DISTICTION";
                }
                else if(CGPA >= 3.0M && CGPA <= 3.49M)
                {
                    title = "UPPER CREDIT";
                }
                else if(CGPA >= 2.5M && CGPA <= 2.99M)
                {
                    title = "LOWER CREDIT";
                }
                else if(CGPA >= 2.0M && CGPA <= 2.49M)
                {
                    title = "PASS";
                }
                else if(CGPA < 2.0M)
                {
                    title = "POOR";
                }
            }
            catch(Exception)
            {
                throw;
            }
            return title;
        }

        private string GetGraduatingDegree(int? progId)
        {
            try
            {
                if(progId == 1 || progId == 2)
                {
                    return "NATIONAL DIPLOMA";
                }
                return "HIGHER NATIONAL DIPLOMA";
            }
            catch(Exception)
            {
                throw;
            }
        }

        private List<Model.Model.Result> GetResultList(Session session,Semester semester,Department department,
            Level level,Programme programme)
        {
            try
            {
                var filteredResult = new List<Model.Model.Result>();
                var studentResultLogic = new StudentResultLogic();
                List<string> resultList =
                    studentResultLogic.GetProcessedResutBy(session,semester,level,department,programme)
                        .Select(p => p.MatricNumber)
                        .AsParallel()
                        .Distinct()
                        .ToList();
                List<Model.Model.Result> result = studentResultLogic.GetProcessedResutBy(session,semester,level,
                    department,programme);
                foreach(string item in resultList)
                {
                    Model.Model.Result resultItem = result.Where(p => p.MatricNumber == item).FirstOrDefault();
                    filteredResult.Add(resultItem);
                }

                return filteredResult.OrderBy(p => p.Name).ToList();
            }
            catch(Exception)
            {
                throw;
            }
        }

        private bool InvalidUserInput(Session session,Semester semester,Department department,Level level,
            Programme programme)
        {
            try
            {
                if(session == null || session.Id <= 0 || semester == null || semester.Id <= 0 || department == null ||
                    department.Id <= 0 || programme == null || programme.Id <= 0 || level == null || level.Id <= 0)
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
    }
}