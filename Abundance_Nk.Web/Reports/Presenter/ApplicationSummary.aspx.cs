using Abundance_Nk.Business;
using Abundance_Nk.Model.Model;
using Abundance_Nk.Web.Models;
using Microsoft.Reporting.WebForms;
using System;
using System.Collections.Generic;
using System.Web.UI;

namespace Abundance_Nk.Web.Reports.Presenter
{
    public partial class ApplicationSummary :Page
    {
        public Session SelectedSession
        {
            get
            {
                return new Session { Id = Convert.ToInt32(ddlSession.SelectedValue),Name = ddlSession.SelectedItem.Text };
            }
            set { ddlSession.SelectedValue = value.Id.ToString(); }
        }

        protected void Page_Load(object sender,EventArgs e)
        {
            try
            {
                lblMessage.Text = "";

                if(!IsPostBack)
                {
                    PopulateAllDropDown();
                }
            }
            catch(Exception ex)
            {
                lblMessage.Text = ex.Message + ex.InnerException.Message;

                //throw ex;
            }
        }

        private void DisplayReportBy(Session session)
        {
            try
            {
                var applicationFormLogic = new ApplicationFormLogic();
                List<PhotoCard> photoCards = applicationFormLogic.GetPostJAMBApplications(session);

                string bind_dsPostJambApplicant = "dsApplicant";
                string reportPath = @"Reports\ApplicantionSummary.rdlc";

                rv.Reset();
                rv.LocalReport.DisplayName = "Applicantion Summary for" + session.Name;
                rv.LocalReport.ReportPath = reportPath;

                if(photoCards != null)
                {
                    rv.ProcessingMode = ProcessingMode.Local;
                    rv.LocalReport.DataSources.Add(new ReportDataSource(bind_dsPostJambApplicant.Trim(),photoCards));
                    rv.LocalReport.Refresh();
                }
            }
            catch(Exception ex)
            {
                lblMessage.Text = ex.Message + ex.InnerException.Message;
                ;
            }
        }

        private void PopulateAllDropDown()
        {
            try
            {
                Utility.BindDropdownItem(ddlSession,Utility.GetAllSessions(),Utility.ID,Utility.NAME);
                //if (ddlSession.Items.Count > 1)
                //{
                //    ddlSession.SelectedIndex = 1;
                //}
            }
            catch(Exception ex)
            {
                lblMessage.Text = ex.Message + ex.InnerException.Message;
                ;
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

                DisplayReportBy(SelectedSession);
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
                if(SelectedSession == null || SelectedSession.Id <= 0)
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