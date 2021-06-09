using Abundance_Nk.Business;
using Abundance_Nk.Model.Model;
using Microsoft.Reporting.WebForms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;

namespace Abundance_Nk.Web.Reports.Presenter.Result
{
    public partial class NotificationOfResult :Page
    {
        private string strDepartmentId = "";
        private string strLevelId = "";
        private string strPersonId = "";
        private string strProgrammeId = "";
        private string strSemesterid = "";
        private string strSessionId = "";

        protected void Page_Load(object sender,EventArgs e)
        {
            try
            {
                lblMessage.Text = "";
                if(!Page.IsPostBack)
                {
                    strPersonId = Request.QueryString["personId"];
                    strSemesterid = Request.QueryString["semesterId"];
                    strSessionId = Request.QueryString["sessionId"];
                    strProgrammeId = Request.QueryString["programmeId"];
                    strDepartmentId = Request.QueryString["departmentId"];
                    strLevelId = Request.QueryString["levelId"];
                    if(!string.IsNullOrEmpty(strPersonId))
                    {
                        long personId = Convert.ToInt64(strPersonId);
                        int semesterId = Convert.ToInt32(strSemesterid);
                        int sessionId = Convert.ToInt32(strSessionId);
                        int programmeId = Convert.ToInt32(strProgrammeId);
                        int departmentId = Convert.ToInt32(strDepartmentId);
                        int levelId = Convert.ToInt32(strLevelId);

                        var student = new Student { Id = personId };
                        var semester = new Semester { Id = semesterId };
                        var session = new Session { Id = sessionId };
                        var programme = new Programme { Id = programmeId };
                        var department = new Department { Id = departmentId };
                        var level = new Level { Id = levelId };
                        DisplayReportBy(session,semester,student,department,level,programme);
                    }
                }
            }
            catch(Exception ex)
            {
                lblMessage.Text = ex.Message + ex.InnerException.Message;
            }
        }

        private void DisplayReportBy(Session session,Semester semester,Student student,Department department,
            Level level,Programme programme)
        {
            try
            {
                List<Model.Model.Result> resultList = null;
                var studentResultLogic = new StudentResultLogic();
                if(semester.Id == 1)
                {
                    List<Model.Model.Result> result = null;
                    result = studentResultLogic.GetStudentProcessedResultBy(session,level,department,student,
                        semester,programme);
                    decimal? firstSemesterGPCUSum = result.Sum(p => p.GPCU);
                    int? firstSemesterTotalSemesterCourseUnit = 0;
                    var studentResultFirstSemester = new Model.Model.Result();
                    studentResultFirstSemester = result.FirstOrDefault();
                    firstSemesterTotalSemesterCourseUnit = studentResultFirstSemester.TotalSemesterCourseUnit;
                    decimal? firstSemesterGPA = firstSemesterGPCUSum / firstSemesterTotalSemesterCourseUnit;
                    studentResultFirstSemester.GPA = firstSemesterGPA;
                    studentResultFirstSemester.CGPA = firstSemesterGPA;
                    studentResultFirstSemester.StudentTypeName =
                        GetGraduatingDegree(studentResultFirstSemester.ProgrammeId);
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
                        studentResultLogic.GetStudentProcessedResultBy(session,level,department,student,
                            secondSemester,programme);
                    decimal? secondSemesterGPCUSum = secondSemesterResultList.Sum(p => p.GPCU);
                    studentResultSecondSemester = secondSemesterResultList.FirstOrDefault();

                    studentResultSecondSemester.GPA =
                        Decimal.Round(
                            (decimal)(secondSemesterGPCUSum / studentResultSecondSemester.TotalSemesterCourseUnit),2);
                    studentResultSecondSemester.CGPA =
                        Decimal.Round(
                            (decimal)
                                ((firstSemesterGPCUSum + secondSemesterGPCUSum) /
                                 (studentResultSecondSemester.TotalSemesterCourseUnit +
                                  firstSemesterTotalSemesterCourseUnit)),2);
                    studentResultSecondSemester.StudentTypeName =
                        GetGraduatingDegree(studentResultSecondSemester.ProgrammeId);
                    studentResultSecondSemester.GraduationStatus = GetGraduationStatus(studentResultSecondSemester.CGPA);
                    resultList = new List<Model.Model.Result>();
                    resultList.Add(studentResultSecondSemester);
                }

                string bind_dsStudentPaymentSummary = "dsNotificationOfResult";
                string reportPath = @"Reports\Result\NotificationOfResult.rdlc";

                ReportViewer1.Reset();
                ReportViewer1.LocalReport.DisplayName = "Notification of Result ";
                ReportViewer1.LocalReport.ReportPath = reportPath;
                ReportViewer1.LocalReport.EnableExternalImages = true;

                if(resultList != null)
                {
                    ReportViewer1.ProcessingMode = ProcessingMode.Local;
                    ReportViewer1.LocalReport.DataSources.Add(new ReportDataSource(bind_dsStudentPaymentSummary.Trim(),
                        resultList));
                    ReportViewer1.LocalReport.Refresh();
                }
            }
            catch(Exception ex)
            {
                lblMessage.Text = ex.Message + ex.InnerException.Message;
            }
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

        private List<Model.Model.Result> GetResultList(Session session,Semester semester,Student student,
            Department department,Level level,Programme programme)
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
    }
}