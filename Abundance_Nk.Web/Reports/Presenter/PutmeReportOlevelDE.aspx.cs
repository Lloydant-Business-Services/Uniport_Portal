﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;
using System.Web.UI.WebControls;
using Abundance_Nk.Business;
using Abundance_Nk.Model.Model;
using Abundance_Nk.Web.Models;
using Ionic.Zip;
using Microsoft.Reporting.WebForms;

namespace Abundance_Nk.Web.Reports.Presenter
{
    public partial class PutmeReportOlevelDE : System.Web.UI.Page
    {
        private List<Department> departments;
        private List<InstitutionChoice> choices;


        public Department SelectedDepartment
        {
            get
            {
                return new Department
                {
                    Id = Convert.ToInt32(ddlDepartment.SelectedValue),
                    Name = ddlDepartment.SelectedItem.Text
                };
            }
            set { ddlDepartment.SelectedValue = value.Name.ToString(); }
        }

        public InstitutionChoice SelectedChoice
        {
            get
            {
                return new InstitutionChoice
                {
                    Id = Convert.ToInt32(ddlChoice.SelectedValue),
                    Name = ddlChoice.SelectedItem.Text
                };
            }
            set { ddlChoice.SelectedValue = value.Name.ToString(); }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                lblMessage.Text = "";

                if (!IsPostBack)
                {
                    DepartmentLogic departmentLogic = new DepartmentLogic();
                    departments = departmentLogic.GetBy(new Programme() { Id = 1 });
                    Utility.BindDropdownItem(ddlDepartment, departments, Utility.ID, Utility.NAME);

                    InstitutionChoiceLogic choiceLogic = new InstitutionChoiceLogic();
                    choices = choiceLogic.GetAll();
                    Utility.BindDropdownItem(ddlChoice, choices, Utility.ID, Utility.NAME);

                    ddlDepartment.Visible = true;
                    ddlChoice.Visible = true;
                    //  BulkOperations();
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
                if (SelectedDepartment == null || SelectedDepartment.Id <= 0)
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

        private void DisplayReportBy(Department department, InstitutionChoice institutionChoice)
        {
            try
            {
                var putmeResultLogic = new PutmeResultLogic();
                List<PutmeSlip> report = putmeResultLogic.GetPUTMEReportOlevelByChoice(department, 18, institutionChoice);
                string bind_dsStudentPaymentSummary = "dsReport";
                string reportPath = @"Reports\PUTME_OLEVEL_DE.rdlc";

                ReportViewer1.Reset();
                ReportViewer1.LocalReport.DisplayName = "PUTME Report ";
                ReportViewer1.LocalReport.ReportPath = reportPath;

                if (report != null)
                {
                    ReportViewer1.ProcessingMode = ProcessingMode.Local;
                    ReportViewer1.LocalReport.DataSources.Add(new ReportDataSource(bind_dsStudentPaymentSummary.Trim(),
                        report));
                    ReportViewer1.LocalReport.Refresh();
                }
            }
            catch (Exception ex)
            {
                lblMessage.Text = ex.Message + ex.InnerException.Message;
                ;
            }
        }



        protected void Display_Button_Click1(object sender, EventArgs e)
        {
            try
            {
                if (InvalidUserInput())
                {
                    lblMessage.Text = "All fields must be selected";
                    return;
                }

                DisplayReportBy(SelectedDepartment,SelectedChoice);
            }
            catch (Exception ex)
            {
                lblMessage.Text = ex.Message + ex.InnerException.Message;
                ;
            }
        }

        public void BulkOperations()
        {
            try
            {
                Warning[] warnings;
                string[] streamIds;
                string mimeType = string.Empty;
                string encoding = string.Empty;
                string extension = string.Empty;

                string downloadPath = "/Content/tempPUTME/" + DateTime.Now.Hour + DateTime.Now.Minute + DateTime.Now.Second + DateTime.Now.Millisecond + "/";

                if (Directory.Exists(Server.MapPath(downloadPath)))
                {
                    Directory.Delete(Server.MapPath(downloadPath), true);
                    Directory.CreateDirectory(Server.MapPath(downloadPath));
                }
                else
                {
                    Directory.CreateDirectory(Server.MapPath(downloadPath));
                }
                List<Course> courses = new List<Course>();

                var departmentLogic = new DepartmentLogic();
                var choice = new InstitutionChoiceLogic();

                List<Department> departments = departmentLogic.GetAll();
                List<InstitutionChoice> choices = choice.GetAll();

                foreach (Department deptDepartment in departments)
                {
                    foreach (InstitutionChoice institutionChoice in choices)
                    {
                        var putmeResultLogic = new PutmeResultLogic();
                        //List<PutmeSlip> report = putmeResultLogic.GetPUTMEReport(deptDepartment,5);
                        List<PutmeSlip> report = putmeResultLogic.GetPUTMEReportOlevelByChoice(deptDepartment, 12, institutionChoice);

                        if (report.Count > 0)
                        {
                            string bind_dsStudentPaymentSummary = "dsReport";
                            string reportPath = @"Reports\PUTME_OLEVEL_DE.rdlc";

                            var rptViewer = new ReportViewer();
                            rptViewer.Visible = false;
                            rptViewer.Reset();
                            rptViewer.LocalReport.DisplayName = "PUTME Report";
                            rptViewer.ProcessingMode = ProcessingMode.Local;
                            rptViewer.LocalReport.ReportPath = reportPath;
                            rptViewer.LocalReport.DataSources.Add(new ReportDataSource(bind_dsStudentPaymentSummary.Trim(), report));

                            byte[] bytes = rptViewer.LocalReport.Render("PDF", null, out mimeType, out encoding,
                                out extension, out streamIds, out warnings);

                            string name = deptDepartment.Code.Replace(" ", "") + "_" + institutionChoice.Name.Replace(" ", "");
                            string path = Server.MapPath(downloadPath);
                            string savelocation = Path.Combine(path, name + ".pdf");
                            File.WriteAllBytes(savelocation, bytes);
                        }
                    }

                   

                }

                using (var zip = new ZipFile())
                {
                    string file = Server.MapPath(downloadPath);
                    zip.AddDirectory(file, "");
                    string zipFileName = SelectedDepartment.Name;
                    zip.Save(file + zipFileName + ".zip");
                    string export = downloadPath + zipFileName + ".zip";

                    //Response.Redirect(export, false);
                    var urlHelp = new UrlHelper(HttpContext.Current.Request.RequestContext);
                    Response.Redirect(
                        urlHelp.Action("DownloadZip",
                            new
                            {
                                controller = "StaffCourseAllocation",
                                area = "Admin",
                                downloadName = SelectedDepartment.Name
                            }), false);
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
     
    }
}