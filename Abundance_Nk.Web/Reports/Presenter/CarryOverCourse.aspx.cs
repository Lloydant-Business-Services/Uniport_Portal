using Abundance_Nk.Business;
using Abundance_Nk.Model.Model;
using Microsoft.Reporting.WebForms;
using System;
using System.Collections.Generic;
using System.Web.UI;

namespace Abundance_Nk.Web.Reports.Presenter
{
    public partial class CarryOverCourse :Page
    {
        private string courseId;
        private string departmentId;
        private string levelId;
        private string programmeId;
        private string semesterId;
        private string sessionId;

        protected void Page_Load(object sender,EventArgs e)
        {
            try
            {
                lblMessage.Text = "";
                if(Request.QueryString["departmentId"] != null && Request.QueryString["programmeId"] != null &&
                    Request.QueryString["sessionId"] != null && Request.QueryString["levelId"] != null &&
                    Request.QueryString["semesterId"] != null && Request.QueryString["courseId"] != null)
                {
                    departmentId = Request.QueryString["departmentId"];
                    programmeId = Request.QueryString["programmeId"];
                    sessionId = Request.QueryString["sessionId"];
                    levelId = Request.QueryString["levelId"];
                    semesterId = Request.QueryString["semesterId"];
                    courseId = Request.QueryString["courseId"];
                }
            }
            catch(Exception ex)
            {
                lblMessage.Text = ex.Message + ex.InnerException.Message;
            }
        }

        protected void Button1_Click(object sender,EventArgs e)
        {
            int departmentIdNew = Convert.ToInt32(departmentId);
            int programmeIdNew = Convert.ToInt32(programmeId);
            int sessionIdNew = Convert.ToInt32(sessionId);
            int levelIdNew = Convert.ToInt32(levelId);
            int semesterIdNew = Convert.ToInt32(semesterId);
            int courseIdNew = Convert.ToInt32(courseId);
            BuildCarryOverCourseList(departmentIdNew,programmeIdNew,levelIdNew,sessionIdNew,courseIdNew,
                semesterIdNew);
        }

        private void BuildCarryOverCourseList(int departmentId,int programmeId,int levelId,int sessionId,
            int courseId,int semesterId)
        {
            try
            {
                var department = new Department { Id = departmentId };
                var programme = new Programme { Id = programmeId };
                var level = new Level { Id = levelId };
                var session = new Session { Id = sessionId };
                var course = new Course { Id = courseId };
                var semester = new Semester { Id = semesterId };
                var courseRegistrationLogic = new CourseRegistrationLogic();

                List<CarryOverReportModel> carryOverCourseList = courseRegistrationLogic.GetCarryOverCourseList(
                    session,semester,programme,department,level,course);

                string bind_dsCarryOverCourseList = "dsCarryOverCourseList";

                string reportPath = @"Reports\CarryOverCourseReport.rdlc";

                ReportViewer1.Reset();
                ReportViewer1.LocalReport.DisplayName = "Carry Over Report";
                ReportViewer1.LocalReport.ReportPath = reportPath;

                if(carryOverCourseList != null)
                {
                    ReportViewer1.ProcessingMode = ProcessingMode.Local;
                    ReportViewer1.LocalReport.DataSources.Add(new ReportDataSource(bind_dsCarryOverCourseList.Trim(),
                        carryOverCourseList));

                    ReportViewer1.LocalReport.Refresh();
                }
            }
            catch(Exception ex)
            {
                lblMessage.Text = ex.Message + ex.InnerException.Message;
            }
        }
    }
}