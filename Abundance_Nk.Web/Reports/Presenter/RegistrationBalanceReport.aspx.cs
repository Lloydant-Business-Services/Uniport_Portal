using Abundance_Nk.Business;
using Abundance_Nk.Model.Model;
using Abundance_Nk.Web.Models;
using Microsoft.Reporting.WebForms;
using System;
using System.Collections.Generic;
using System.Web.UI;

namespace Abundance_Nk.Web.Reports.Presenter
{
    public partial class PaymentReport :Page
    {
        private readonly SemesterLogic semesterLogic = new SemesterLogic();
        private List<Semester> semesters = new List<Semester>();

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

        protected void Page_Load(object sender,EventArgs e)
        {
            try
            {
                lblMessage.Text = "";
                if(!IsPostBack)
                {
                    semesters.Add(new Semester { Id = 0,Name = "-- Select--" });
                    semesters = semesterLogic.GetAll();
                    Utility.BindDropdownItem(ddlSession,Utility.GetAllSessions(),Utility.ID,Utility.NAME);
                    Utility.BindDropdownItem(ddlSemester,semesters,Utility.ID,Utility.NAME);
                }
            }
            catch(Exception ex)
            {
                lblMessage.Text = ex.Message + ex.InnerException.Message;
            }
        }

        protected void Button1_Click(object sender,EventArgs e)
        {
            try
            {
                if(InvalidUserInput())
                {
                    lblMessage.Text = "All fields must be selected";
                    return;
                }

                var paymentLogic = new PaymentLogic();

                List<RegistrationBalanceReport> registrationBalanceList =
                    paymentLogic.GetRegistrationBalanceList(SelectedSession,SelectedSemester);

                string bind_dsRegistrationBalanceList = "dsRegistrationBalanceList";

                string reportPath = @"Reports\RegistrationBalanceReport.rdlc";

                ReportViewer1.Reset();
                ReportViewer1.LocalReport.DisplayName = "Payment Report";
                ReportViewer1.LocalReport.ReportPath = reportPath;

                if(registrationBalanceList != null)
                {
                    ReportViewer1.ProcessingMode = ProcessingMode.Local;
                    ReportViewer1.LocalReport.DataSources.Add(new ReportDataSource(
                        bind_dsRegistrationBalanceList.Trim(),registrationBalanceList));

                    ReportViewer1.LocalReport.Refresh();
                }
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
                if(SelectedSession == null || SelectedSession.Id <= 0 || SelectedSemester == null ||
                    SelectedSemester.Id <= 0)
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