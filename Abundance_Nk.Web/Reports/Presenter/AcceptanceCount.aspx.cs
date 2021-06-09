﻿using Abundance_Nk.Business;
using Abundance_Nk.Model.Model;
using Abundance_Nk.Web.Models;
using Microsoft.Reporting.WebForms;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Ionic.Zip;

namespace Abundance_Nk.Web.Reports.Presenter
{

    public partial class AcceptanceCount : System.Web.UI.Page
    {
        List<Department> departments;
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
        public Session SelectedSession
        {
            get { return new Session() { Id = Convert.ToInt32(ddlSession.SelectedValue), Name = ddlSession.SelectedItem.Text }; }
            set { ddlSession.SelectedValue = value.Id.ToString(); }
        }

        private void DisplayReport(Session session)
        {
            try
            {
                PaymentLogic paymentLogic = new PaymentLogic();
                var report = paymentLogic.GetAcceptanceCount(session, txtBoxDateFrom.Text, txtBoxDateTo.Text);
                string bind_dsStudentPaymentSummary = "DataSet";
                string reportPath = @"Reports\AcceptanceCountReport.rdlc";


                ReportViewer1.Reset();
                ReportViewer1.LocalReport.DisplayName = "Acceptance Report ";
                ReportViewer1.LocalReport.ReportPath = reportPath;

                if (report != null)
                {
                    ReportViewer1.ProcessingMode = ProcessingMode.Local;
                    ReportViewer1.LocalReport.DataSources.Add(new ReportDataSource(bind_dsStudentPaymentSummary.Trim(), report));
                    ReportViewer1.LocalReport.Refresh();
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
        protected void Display_Button_Click(object sender, EventArgs e)
        {
            if (InvalidUserInput())
            {
                lblMessage.Text = "All fields must be selected";
                return;
            }


            DisplayReport(SelectedSession);
        
        }
    }
}