using Abundance_Nk.Business;
using Abundance_Nk.Model.Model;
using Abundance_Nk.Web.Models;
using Abundance_Nk.Web.Models.Intefaces;
using Microsoft.Reporting.WebForms;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Web.UI;

namespace Abundance_Nk.Web.Reports.Presenter.Result
{
    public partial class Transcript :Page,IReport
    {
        public Level Level
        {
            get { return new Level { Id = Convert.ToInt32(ddlLevel.SelectedValue),Name = ddlLevel.SelectedItem.Text }; }
            set { ddlLevel.SelectedValue = value.Id.ToString(); }
        }

        public Programme Programme
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

        public Department Department
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

        public Student Student
        {
            get
            {
                return new Student { Id = Convert.ToInt32(ddlStudent.SelectedValue),Name = ddlStudent.SelectedItem.Text };
            }
            set { ddlStudent.SelectedValue = value.Id.ToString(); }
        }

        public Session CurrentSession
        {
            get { return new Session { Id = Convert.ToInt32(hfSession.Value) }; }
            set { hfSession.Value = value.Id.ToString(); }
        }

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

        protected void Page_Load(object sender,EventArgs e)
        {
            try
            {
                Message = "";

                if(!IsPostBack)
                {
                    PopulateAllDropDown();

                    ddlStudent.Visible = false;
                    ddlDepartment.Visible = false;
                    SetCurrentSession();
                }
            }
            catch(Exception ex)
            {
                lblMessage.Text = ex.Message;
            }
        }

        private void SetCurrentSession()
        {
            try
            {
                var currentSessionSemesterLogic = new CurrentSessionSemesterLogic();
                CurrentSessionSemester currentSessionSemester = currentSessionSemesterLogic.GetCurrentSessionSemester();
                CurrentSession = currentSessionSemester.SessionSemester.Session;
            }
            catch(Exception)
            {
                throw;
            }
        }

        private void DisplayReportBy(Student student,Programme programme,Department department,Faculty faculty)
        {
            try
            {
                var scoreGradeLogic = new ScoreGradeLogic();
                var resultLogic = new StudentResultLogic();
                var academicStandingLogic = new AcademicStandingLogic();
                List<Model.Model.Result> results = resultLogic.GetTranscriptBy(student);

                string abbreviations = academicStandingLogic.GetAbbreviations();
                string scoreGradingKeys = scoreGradeLogic.GetScoreGradingKey();

                string reportPath = @"Reports\Result\ResultTranscript.rdlc";
                //string reportPath = @"Reports\Transcript1.rdlc";
                string bind_ds = "dsMasterSheet";

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
                rv.LocalReport.ReportPath = reportPath;
                rv.LocalReport.EnableExternalImages = true;
                rv.LocalReport.DisplayName = "Statement of Result";

                string programmeName = "Undergraduate";
                //ReportParameter programmeParam = new ReportParameter("Programme", programmeName);
                //ReportParameter departmentParam = new ReportParameter("Department", department.Name);
                //ReportParameter facultyParam = new ReportParameter("Faculty", faculty.Name);
                //ReportParameter abbreviationsParam = new ReportParameter("Abbreviations", abbreviations);
                //ReportParameter scoreGradingKeysParam = new ReportParameter("ScoreGradingKeys", scoreGradingKeys);
                //ReportParameter[] reportParams = new ReportParameter[] { departmentParam, facultyParam, programmeParam, abbreviationsParam, scoreGradingKeysParam };

                //rv.LocalReport.SetParameters(reportParams);
                if(results != null && results.Count > 0)
                {
                    rv.ProcessingMode = ProcessingMode.Local;
                    rv.LocalReport.DataSources.Add(new ReportDataSource(bind_ds.Trim(),results));
                    rv.LocalReport.Refresh();
                    //rv.Visible = true;
                }
                else
                {
                    lblMessage.Text = "No result to display";
                    rv.Visible = false;
                }
            }
            catch(Exception ex)
            {
                lblMessage.Text = ex.Message;
            }
        }

        private void PopulateAllDropDown()
        {
            try
            {
                Utility.BindDropdownItem(ddlLevel,Utility.GetAllLevels(),Utility.ID,Utility.NAME);
                Utility.BindDropdownItem(ddlProgramme,Utility.GetAllProgrammes(),Utility.ID,Utility.NAME);
            }
            catch(Exception ex)
            {
                lblMessage.Text = ex.Message;
            }
        }

        protected void ddlDepartment_SelectedIndexChanged(object sender,EventArgs e)
        {
            try
            {
                if(InvalidUserInput())
                {
                    return;
                }

                rv.LocalReport.DataSources.Clear();
                List<Student> students = Utility.GetStudentsBy(Level,Programme,Department,CurrentSession);
                if(students != null && students.Count > 0)
                {
                    Utility.BindDropdownItem(ddlStudent,students,Utility.ID,"FirstName");
                    ddlStudent.Visible = true;
                    rv.Visible = true;
                }
                else
                {
                    ddlStudent.Visible = false;
                    rv.Visible = false;
                }
            }
            catch(Exception ex)
            {
                lblMessage.Text = ex.Message;
            }
        }

        private bool InvalidUserInput()
        {
            try
            {
                if(Level == null || Level.Id <= 0)
                {
                    lblMessage.Text = "Please select Level";
                    return true;
                }
                if(Programme == null || Programme.Id <= 0)
                {
                    lblMessage.Text = "Please select Programme";
                    return true;
                }
                if(Department == null || Department.Id <= 0)
                {
                    lblMessage.Text = "Please select Department";
                    return true;
                }
                if(CurrentSession == null || CurrentSession.Id <= 0)
                {
                    lblMessage.Text = "Current Session not set! Please contact your system administrator.";
                    return true;
                }

                return false;
            }
            catch(Exception)
            {
                throw;
            }
        }

        protected void btnDisplayReport_Click(object sender,EventArgs e)
        {
            try
            {
                if(InvalidUserInput())
                {
                    return;
                }
                if(Student == null || Student.Id <= 0)
                {
                    lblMessage.Text = "Please select Student";
                    return;
                }

                var studentLevelLogic = new StudentLevelLogic();
                StudentLevel studentLevel = studentLevelLogic.GetBy(Student,CurrentSession);

                DisplayReportBy(Student,Programme,Department,studentLevel.Department.Faculty);
            }
            catch(Exception ex)
            {
                lblMessage.Text = ex.Message;
            }
        }

        protected void ddlProgramme_SelectedIndexChanged(object sender,EventArgs e)
        {
            try
            {
                if(Programme != null && Programme.Id > 0)
                {
                    PopulateDepartmentDropdownByProgramme(Programme);
                }
                else
                {
                    ddlDepartment.Visible = false;
                }
            }
            catch(Exception ex)
            {
                lblMessage.Text = ex.Message;
            }
        }

        private void PopulateDepartmentDropdownByProgramme(Programme programme)
        {
            try
            {
                List<Department> departments = Utility.GetDepartmentByProgramme(programme);
                if(departments != null && departments.Count > 0)
                {
                    Utility.BindDropdownItem(ddlDepartment,Utility.GetDepartmentByProgramme(programme),Utility.ID,
                        Utility.NAME);
                    ddlDepartment.Visible = true;
                }
            }
            catch(Exception ex)
            {
                lblMessage.Text = ex.Message;
            }
        }
    }
}