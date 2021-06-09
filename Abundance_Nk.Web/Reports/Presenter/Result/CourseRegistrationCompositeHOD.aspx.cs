using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Abundance_Nk.Business;
using Abundance_Nk.Model.Model;
using Abundance_Nk.Web.Models;
using Abundance_Nk.Web.Models.Intefaces;
using Microsoft.Reporting.WebForms;

namespace Abundance_Nk.Web.Reports.Presenter
{
    public partial class CourseRegistrationCompositeHOD :Page,IReport
    {
        
        public SessionSemester SelectedSession
        {
            get
            {
                return new SessionSemester
                {
                    Id = Convert.ToInt32(ddlSession.SelectedValue),
                    Name = ddlSession.SelectedItem.Text
                };
            }
            set { ddlSession.SelectedValue = value.Id.ToString(); }
        }

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
                   ddlDepartment.Visible = false;
                    PopulateAllDropDown();
                }
            }
            catch(Exception ex)
            {
                lblMessage.Text = ex.Message;
            }
        }

        private void DisplayReportBy(SessionSemester session,Level level,Programme programme,Department department,
            int type)
        {
            try
            {
                var resultLogic = new StudentResultLogic();
                List<Model.Model.Result> resultList = resultLogic.GetMaterSheetCourseDetailsBy(session,level,programme,
                    department);
                string reportPath = "";
                string bind_ds = "dsMasterSheet";
                reportPath = @"Reports\Result\CourseRegistrationComposite.rdlc";
                

                rv.Reset();
                rv.LocalReport.DisplayName = "Student Master Sheet";
                rv.LocalReport.ReportPath = reportPath;
                rv.LocalReport.EnableExternalImages = true;

                string programmeName = programme.Id > 2 ? "Undergraduate" : "Undergraduate ";
                var programmeParam = new ReportParameter("Programme",programmeName);
                var departmentParam = new ReportParameter("Department",department.Name);
                var sessionSemesterParam = new ReportParameter("SessionSemester",session.Name);
                ReportParameter[] reportParams = { departmentParam,programmeParam,sessionSemesterParam };
                rv.LocalReport.SetParameters(reportParams);

                if(resultList != null)
                {
                    rv.ProcessingMode = ProcessingMode.Local;
                    rv.LocalReport.DataSources.Add(new ReportDataSource(bind_ds.Trim(),resultList));
                    rv.LocalReport.Refresh();
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
                Utility.BindDropdownItem(ddlSession,Utility.GetAllSessionSemesters(),Utility.ID,Utility.NAME);
                Utility.BindDropdownItem(ddlLevel,Utility.GetAllLevels(),Utility.ID,Utility.NAME);
                Utility.BindDropdownItem(ddlProgramme,Utility.GetAllProgrammes(),Utility.ID,Utility.NAME);

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
                if(SelectedSession == null || SelectedSession.Id <= 0)
                {
                    lblMessage.Text = "Please select Session";
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

                //SortOption sortOption = Option == 2 ? SortOption.ExamNo : Option == 3 ? SortOption.ApplicationNo : SortOption.Name;
                DisplayReportBy(SelectedSession,Level,Programme,Department,1);
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
                var loggeduser = new UserLogic();
                var user = loggeduser.GetModelBy(u => u.User_Name == User.Identity.Name);
                if (user != null && user.Id > 0)
                {
                    StaffLogic staffLogic = new StaffLogic();
                    Staff staff = staffLogic.GetBy(user.Id);
                    if (staff != null && staff.Id > 0)
                    {

                        List<Department> departments;
                        if ((bool)staff.isManagement)
                        {
                            departments = Utility.PopulateDepartmentByFacultyId(staff.Department.Faculty.Id);
                        }
                        else
                        {
                            departments = Utility.PopulateDepartmentById(staff.Department.Id);
                        }

                        //List<Department> departments = Utility.GetDepartmentByProgramme(programme);
                        if (departments != null && departments.Count > 0)
                        {
                            Utility.BindDropdownItem(ddlDepartment, departments, Utility.ID, Utility.NAME);
                            ddlDepartment.Visible = true;
                        }
                    }

                }


            }
            catch (Exception ex)
            {
                lblMessage.Text = ex.Message;
            }
        }

    }
}