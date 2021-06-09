using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Abundance_Nk.Business;
using Abundance_Nk.Model.Model;
using Abundance_Nk.Web.Models;
using Microsoft.Reporting.WebForms;

namespace Abundance_Nk.Web.Reports.Presenter
{
    public partial class AdmittedStudentBreakdown : System.Web.UI.Page
    {

        public Session SelectedSession
        {
            get
            {
                return new Session
                {
                    Id = Convert.ToInt32(ddlSession.SelectedValue),
                    Name = ddlSession.SelectedItem.Text
                };
            }
            set { ddlSession.SelectedValue = value.Id.ToString(); }

        }
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                lblMessage.Text = "";
                if (!IsPostBack)
                {
                    PopulateAllDropDown();
                }

            }
            catch (Exception)
            {

                throw;
            }

        }
        private void PopulateAllDropDown()
        {
            try
            {
                List<Session> session = Utility.GetAllSessions();
                Utility.BindDropdownItem(ddlSession, session, Utility.ID, Utility.NAME);

            }
            catch (Exception ex)
            {
                lblMessage.Text = ex.Message;
            }
        }
        private void DisplayReportBy(Session session)
        {
            try
            {
                AdmissionListLogic admissionListLogic = new AdmissionListLogic();
                var admittedBreakDowns = new List<AdmittedStudentBreakdownView>();
                admittedBreakDowns = admissionListLogic.GetAdmittedBreakDownBy(session);

                string reportPath = @"Reports\AdmittedStudentBreakdown.rdlc";


                rv.Reset();
                rv.LocalReport.DisplayName = "Admitted Student Breakdown";
                rv.LocalReport.ReportPath = reportPath;

                if (admittedBreakDowns != null)
                {
                    rv.ProcessingMode = ProcessingMode.Local;
                    rv.LocalReport.DataSources.Add(new ReportDataSource("DataSet", admittedBreakDowns));
                    rv.LocalReport.Refresh();
                }
            }
            catch (Exception ex)
            {
                lblMessage.Text = ex.Message;
            }
        }
        protected void btnDisplayReport_Click(object sender, EventArgs e)
        {
            try
            {
                if (InvalidUserInput())
                {
                    return;
                }

                DisplayReportBy(SelectedSession);
            }
            catch (Exception ex)
            {
                lblMessage.Text = ex.Message;
            }
        }
        private bool InvalidUserInput()
        {
            try
            {

                if (SelectedSession == null || SelectedSession.Id <= 0)
                {
                    lblMessage.Text = "Please select Session";
                    return true;
                }
                
                return false;
            }
            catch (Exception)
            {
                throw;
            }
        }



    }
}