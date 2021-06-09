using Abundance_Nk.Business;
using Abundance_Nk.Model.Model;
using Abundance_Nk.Web.Models;
using Microsoft.Reporting.WebForms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Abundance_Nk.Web.Reports.Presenter
{
    public partial class AbiaStateIndigeneReportSummary : System.Web.UI.Page
    {
        public Session SelectedSession
        {
            get
            {
                return new Session { Id = Convert.ToInt32(ddlSession.SelectedValue), Name = ddlSession.SelectedItem.Text };
            }
            set { ddlSession.SelectedValue = value.Id.ToString(); }
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

        private void DisplayReportBy(Session session)
        {


            try
            {
                List<Model.Model.Result> DepartmentStudentList = new List<Model.Model.Result>();
                var studentLogic = new StudentLogic();
                var departmentLogic = new DepartmentLogic();
                List<Department> departments = departmentLogic.GetAll();
                foreach(var dept in departments)
                {
                    List<Model.Model.Result> reportByDepartment = studentLogic.GetAbuaStateIndigenesRecord(session,dept.Id);
                    if (reportByDepartment?.Count > 0)
                    {
                        Model.Model.Result deptResult = new Model.Model.Result()
                        {
                            DepartmentName = dept.Name,
                            Count = reportByDepartment.Count,
                            SessionName = session.Name
                        };
                        DepartmentStudentList.Add(deptResult);
                    }
                }
                

                string bind_dsMasterSheet = "dsMasterSheet";
                string reportPath = "";
                reportPath = @"Reports\IndigeneSummaryReport.rdlc";

                rv.Reset();
                rv.LocalReport.DisplayName = "Summary Indigene For" + session.Name + " Session";
                rv.LocalReport.ReportPath = reportPath;

                if (DepartmentStudentList != null)
                {
                    DepartmentStudentList.FirstOrDefault().SessionName = session.Name;
                    rv.ProcessingMode = ProcessingMode.Local;
                    rv.LocalReport.DataSources.Add(new ReportDataSource(bind_dsMasterSheet.Trim(),
                        DepartmentStudentList));
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
                DisplayReportBy(SelectedSession);
            }
            catch (Exception ex)
            {
                lblMessage.Text = ex.Message + ex.InnerException.Message;

            }
        }
    }
}