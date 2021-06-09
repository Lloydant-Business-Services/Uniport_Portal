using Abundance_Nk.Business;
using Abundance_Nk.Model.Model;
using Abundance_Nk.Web.Models;
using Abundance_Nk.Web.Models.Intefaces;
using Microsoft.Reporting.WebForms;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;

namespace Abundance_Nk.Web.Reports.Presenter.Result
{
    public partial class StatementOfSemesterResult :Page,IReport
    {
        public Student Student { get; set;}
        //public Level selectedLevel
        //{
        //    get
        //    {
        //        return new Level 
        //        { 
        //            Id = Convert.ToInt32(ddlLevel.SelectedValue),
        //            Name = ddlLevel.SelectedItem.Text 
        //        };
        //    }
        //    set { ddlLevel.SelectedValue = value.Id.ToString(); }
        //}
        //public Session selectedSession
        //{
        //    get
        //    {
        //        return new Session 
        //        { 
        //            Id = Convert.ToInt32(ddlSession.SelectedValue),
        //            Name = ddlSession.SelectedItem.Text 
        //        };
        //    }
        //    set { ddlSession.SelectedValue = value.Id.ToString(); }
        //}
        //public Semester selectedSemester
        //{
        //    get
        //    {
        //        return new Semester 
        //        { 
        //            Id = Convert.ToInt32(ddlSemester.SelectedValue),
        //            Name = ddlSemester.SelectedItem.Text 
        //        };
        //    }
        //    set { ddlLevel.SelectedValue = value.Id.ToString(); }
        //}

        //public Programme Programme
        //{
        //    get
        //    {
        //        return new Programme
        //        {
        //            Id = Convert.ToInt32(ddlProgramme.SelectedValue),
        //            Name = ddlProgramme.SelectedItem.Text
        //        };
        //    }
        //    set { ddlProgramme.SelectedValue = value.Id.ToString(); }
        //}

        //public Department Department
        //{
        //    get
        //    {
        //        return new Department
        //        {
        //            Id = Convert.ToInt32(ddlDepartment.SelectedValue),
        //            Name = ddlDepartment.SelectedItem.Text
        //        };
        //    }
        //    set { ddlDepartment.SelectedValue = value.Id.ToString(); }
        //}

        //public Student Student
        //{
        //    get
        //    {
        //        return new Student { Id = Convert.ToInt32(ddlStudent.SelectedValue),Name = ddlStudent.SelectedItem.Text };
        //    }
        //    set { ddlStudent.SelectedValue = value.Id.ToString(); }
        //}

        //public SessionSemester SelectedSession
        //{
        //    get
        //    {
        //        return new SessionSemester
        //        {
        //            Id = Convert.ToInt32(ddlSession.SelectedValue),
        //            Name = ddlSession.SelectedItem.Text
        //        };
        //    }
        //    set { ddlSession.SelectedValue = value.Id.ToString(); }
        //}

        public string Message
        {
            get { return lblMessage.Text; }
            set { lblMessage.Text = value; }
        }

        public ReportViewer Viewer
        {
            get { return rv; }
            set { rv = value; }
        }

        public int ReportType
        {
            get { throw new NotImplementedException(); }
        }

        protected async void Page_Load(object sender,EventArgs e)
        {
            try
            {
                Message = "";

                if (!IsPostBack)
                {
                    //PopulateAllDropDown();
                    //ddlSession.Visible = true;
                    //ddlLevel.Visible = true;
                    //ddlSemester.Visible = true;
                    //btnDisplayReport.Visible = true;

                    long sid = Convert.ToInt64(Request["sid"]);
                    int semesterId = Convert.ToInt32(Request["semid"]);
                    int sessionId = Convert.ToInt32(Request["sesid"]);

                    if (sid > 0 && semesterId > 0 && sessionId > 0)
                    {

                        StudentLogic studentLogic = new StudentLogic();
                        Student = studentLogic.GetBy(sid);
                        if (Student == null || Student.Id <= 0)
                        {
                            lblMessage.Text = "No student record found";
                            return;
                        }

                        var studentLevelLogic = new StudentLevelLogic();
                        var sessionSemesterLogic = new SessionSemesterLogic();
                        SessionSemester sessionSemester = sessionSemesterLogic.GetBySessionSemester(semesterId, sessionId);
                        StudentLevel studentLevel = studentLevelLogic.GetBy(Student.Id);
                        CourseRegistrationLogic courseRegistrationLogic = new CourseRegistrationLogic();

                        if (studentLevel != null)
                        {
                            CourseRegistration courseRegistration = courseRegistrationLogic.GetModelsBy(s =>
                                s.Session_Id == sessionId && s.Person_Id == Student.Id &&
                                s.Department_Id == studentLevel.Department.Id && s.Programme_Id == studentLevel.Programme.Id).LastOrDefault();

                            if (courseRegistration != null)
                                await DisplayReportBy(sessionSemester, courseRegistration.Level, studentLevel.Programme, studentLevel.Department, Student);
                        }
                        else
                        {
                            lblMessage.Text = "No result to display";
                            rv.Visible = false;
                        }
                    }
                    else
                    {
                        lblMessage.Text = "Invalid Parameter";
                        rv.Visible = false;
                    }

                }
            }
            catch(Exception ex)
            {
                lblMessage.Text = ex.Message;
            }
        }

        private async Task DisplayReportBy(SessionSemester session,Level level,Programme programme,Department department,Student student)
        {
            try
            {
                var scoreGradeLogic = new ScoreGradeLogic();
                var resultLogic = new StudentResultLogic();
                var academicStandingLogic = new AcademicStandingLogic();
                List<Model.Model.Result> results = await resultLogic.GetStudentResultByAsync(session,level,programme,department,student);
                List<StatementOfResultSummary> resultSummaries = new List<StatementOfResultSummary>();

                 //List<StatementOfResultSummary> resultSummaries = resultLogic.GetStatementOfResultSummaryBy(session,
                 //   level,programme,department,student);


                string bind_ds = "dsMasterSheet";
                string bind_resultSummary = "dsResultSummary";
                string reportPath = @"Reports\Result\StatementOfResult.rdlc";

                if(results != null && results.Count > 0)
                {
                    string appRoot = ConfigurationManager.AppSettings["AppRoot"];
                    foreach(Model.Model.Result result in results)
                    {
                        if(!string.IsNullOrWhiteSpace(result.PassportUrl))
                        {
                            result.PassportUrl = appRoot + result.PassportUrl;
                        }
                        else
                        {
                            result.PassportUrl = appRoot + Utility.DEFAULT_AVATAR;
                        }
                    }
                }

                rv.Reset();
                rv.LocalReport.DisplayName = "Statement of Result";
                rv.LocalReport.ReportPath = reportPath;
                rv.LocalReport.EnableExternalImages = true;
                
                if(results != null && results.Count > 0)
                {
                    rv.ProcessingMode = ProcessingMode.Local;
                    rv.LocalReport.DataSources.Add(new ReportDataSource(bind_ds.Trim(),results));
                    rv.LocalReport.DataSources.Add(new ReportDataSource(bind_resultSummary.Trim(),resultSummaries));

                    rv.LocalReport.Refresh();
                    rv.Visible = true;
                }
                else
                {
                    lblMessage.Text = "No course registration found!";
                    rv.Visible = false;
                }
            }
            catch(Exception ex)
            {
                lblMessage.Text = ex.Message + ex.InnerException.Message;
            }
        }

        //private void PopulateAllDropDown()
        //{
        //    try
        //    {
        //        Utility.BindDropdownItem(ddlSession,Utility.GetAllSessions(),Utility.ID,Utility.NAME);
        //        Utility.BindDropdownItem(ddlLevel,Utility.GetAllLevels(),Utility.ID,Utility.NAME);
        //        Utility.BindDropdownItem(ddlSemester,Utility.GetAllSemesters(),Utility.ID,Utility.NAME);
        //    }
        //    catch(Exception ex)
        //    {
        //        lblMessage.Text = ex.Message;
        //    }
        //}

        //private bool InvalidUserInput()
        //{
        //    try
        //    {
        //        if(selectedLevel == null || selectedLevel.Id <= 0)
        //        {
        //            lblMessage.Text = "Please select Level";
        //            return true;
        //        }

        //        if(SelectedSession == null || SelectedSession.Id <= 0)
        //        {
        //            lblMessage.Text = " Session not set! Please contact your system administrator.";
        //            return true;
        //        }

        //         if(selectedSemester == null || selectedSemester.Id <= 0)
        //        {
        //            lblMessage.Text = " Semester not set! Please contact your system administrator.";
        //            return true;
        //        }

        //        return false;
        //    }
        //    catch(Exception)
        //    {
        //        throw;
        //    }
        //}

        //protected async void btnDisplayReport_Click(object sender,EventArgs e)
        //{
        //    try
        //    {
        //        //if(InvalidUserInput())
        //        //{
        //        //    return;
        //        //}

        //        long sid = Convert.ToInt64(Request["sid"]);
        //        int semesterId = Convert.ToInt32(Request["semid"]);
        //        int sessionId = Convert.ToInt32(Request["sesid"]);
                
                
        //        StudentLogic studentLogic = new StudentLogic();
        //        Student = studentLogic.GetBy(sid);
        //        if(Student == null || Student.Id <= 0)
        //        {
        //            lblMessage.Text = "No student record found";
        //            return;
        //        }

        //        var studentLevelLogic = new StudentLevelLogic();
        //        var sessionSemesterLogic = new SessionSemesterLogic();
        //        SessionSemester sessionSemester = sessionSemesterLogic.GetBySessionSemester(semesterId, sessionId);
        //        StudentLevel studentLevel = studentLevelLogic.GetBy(Student.Id);
        //        CourseRegistrationLogic courseRegistrationLogic = new CourseRegistrationLogic();

        //        if (studentLevel != null)
        //        {

        //            CourseRegistration courseRegistration = await courseRegistrationLogic.GetModelByAsync(s =>
        //                     s.Session_Id == sessionId && s.Person_Id == Student.Id &&
        //                     s.Department_Id == studentLevel.Department.Id && s.Programme_Id == studentLevel.Programme.Id);

        //            await DisplayReportBy(sessionSemester, courseRegistration.Level, studentLevel.Programme, studentLevel.Department, Student);

        //        }
        //        else
        //        {
        //            lblMessage.Text = "No result to display";
        //            rv.Visible = false;
        //        }
        //    }
        //    catch(Exception ex)
        //    {
        //        lblMessage.Text = ex.Message;
        //    }
        //}

    }
}