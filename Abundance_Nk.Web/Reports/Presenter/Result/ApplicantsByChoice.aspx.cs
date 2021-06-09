using System;
using System.Collections.Generic;
using System.IO;
using System.Web.Mvc;
using System.Web.UI;
using Abundance_Nk.Business;
using Abundance_Nk.Model.Model;
using Abundance_Nk.Web.Models;
using Ionic.Zip;
using Microsoft.Reporting.WebForms;

namespace Abundance_Nk.Web.Reports.Presenter.Result
{
    public partial class ApplicantsByChoice : Page
    {
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

        public Session SelectedSession
        {
            get
            {
                return new Session {Id = Convert.ToInt32(ddlSession.SelectedValue), Name = ddlSession.SelectedItem.Text};
            }
            set { ddlSession.SelectedValue = value.Id.ToString(); }
        }

        public InstitutionChoice SelectedInstitutionChoice
        {
            get
            {
                return new InstitutionChoice
                {
                    Id = Convert.ToInt32(ddlInstitutionChoice.SelectedValue),
                    Name = ddlInstitutionChoice.SelectedItem.Text
                };
            }
            set { ddlInstitutionChoice.SelectedValue = value.Id.ToString(); }
        }
        public ApplicationFormSetting SelectedApplicationFormSetting
        {
            get
            {
                return new ApplicationFormSetting
                {
                    Id = Convert.ToInt32(ddlFormType.SelectedValue),
                    Name = ddlFormType.SelectedItem.Text
                };
            }
            set { ddlFormType.SelectedValue = value.Id.ToString(); }
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

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                Message = "";

                if (!IsPostBack)
                {
                    ddlDepartment.Visible = false;
                    PopulateAllDropDown();
                }
            }
            catch (Exception ex)
            {
                lblMessage.Text = ex.Message;
            }
        }

        private void DisplayReport(Session session, Department department, InstitutionChoice institutionChoice,ApplicationFormSetting applicationFormSetting)
        {
            try
            {
               
                var resultLogic = new ApplicantJambDetailLogic();
                List<Model.Model.ApplicantResult> resultList = resultLogic.GetApplicantsByChoice(department, session,institutionChoice,applicationFormSetting);

                string reportPath = "";
                string bind_ds = "dsScreeningReport";
                reportPath = @"Reports\ScreeningReport.rdlc";
                if (applicationFormSetting.Id == 3)
                {
                     reportPath = @"Reports\ScreeningReportDE.rdlc";
                }
                

                rv.Reset();
                rv.LocalReport.DisplayName = "Applicants Result By Choice";
                rv.LocalReport.ReportPath = reportPath;
                rv.LocalReport.EnableExternalImages = true;

                if (resultList != null)
                {
                    rv.ProcessingMode = ProcessingMode.Local;
                    rv.LocalReport.DataSources.Add(new ReportDataSource(bind_ds.Trim(), resultList));
                    rv.LocalReport.Refresh();
                }
            }
            catch (Exception ex)
            {
                lblMessage.Text = ex.Message;
            }
        }

        private void PopulateAllDropDown()
        {
            try
            {
                var programmeLogic = new ProgrammeLogic();
                List<Programme> programmeList = programmeLogic.GetModelsBy(p => p.Programme_Id == 1);
                programmeList.Insert(0, new Programme {Id = 0, Name = "-- Select Programme --"});

                Utility.BindDropdownItem(ddlSession, Utility.GetAllSessions(), Utility.ID, Utility.NAME);
                Utility.BindDropdownItem(ddlProgramme, programmeList, Utility.ID, Utility.NAME);
                Utility.BindDropdownItem(ddlInstitutionChoice, Utility.GetAllInstitutionChoices(), Utility.ID,Utility.NAME);
                Utility.BindDropdownItem(ddlFormType, Utility.GetAllApplicationFormSetting(), Utility.ID,Utility.NAME);
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
                if (SelectedInstitutionChoice == null || SelectedInstitutionChoice.Id <= 0)
                {
                    lblMessage.Text = "Please select Institution Choice";
                    return true;
                }
                 if (SelectedApplicationFormSetting == null || SelectedApplicationFormSetting.Id <= 0)
                {
                    lblMessage.Text = "Please select Form Type";
                    return true;
                }
                if (Programme == null || Programme.Id <= 0)
                {
                    lblMessage.Text = "Please select Programme";
                    return true;
                }
                if (Department == null || Department.Id <= 0)
                {
                    lblMessage.Text = "Please select Department";
                    return true;
                }

                return false;
            }
            catch (Exception)
            {
                throw;
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

                DisplayReport(SelectedSession, Department, SelectedInstitutionChoice,SelectedApplicationFormSetting);
            }
            catch (Exception ex)
            {
                lblMessage.Text = ex.Message;
            }
        }

        protected void ddlProgramme_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (Programme != null && Programme.Id > 0)
                {
                    PopulateDepartmentDropdownByProgramme(Programme);
                }
                else
                {
                    ddlDepartment.Visible = false;
                }
            }
            catch (Exception ex)
            {
                lblMessage.Text = ex.Message;
            }
        }

        private void PopulateDepartmentDropdownByProgramme(Programme programme)
        {
            try
            {
                List<Department> departments = Utility.GetDepartmentByProgramme(programme);
                if (departments != null && departments.Count > 0)
                {
                    Utility.BindDropdownItem(ddlDepartment, Utility.GetDepartmentByProgramme(programme), Utility.ID,
                        Utility.NAME);
                    ddlDepartment.Visible = true;
                }
            }
            catch (Exception ex)
            {
                lblMessage.Text = ex.Message;
            }
        }

        protected void Button1_Click(object sender,EventArgs e)
        {
            try
            {
                Warning[] warnings;
                string[] streamIds;
                string mimeType = string.Empty;
                string encoding = string.Empty;
                string extension = string.Empty;

                if (Directory.Exists(Server.MapPath("~/Content/temp")))
                {
                    Directory.Delete(Server.MapPath("~/Content/temp"), true);
                }
                Directory.CreateDirectory(Server.MapPath("~/Content/temp"));

                DepartmentLogic departmentLogic = new DepartmentLogic();
                List<Department> departments = departmentLogic.GetAll();
                foreach (Department department in departments)
                {
                    var resultLogic = new ApplicantJambDetailLogic();
                    List<Model.Model.ApplicantResult> report = resultLogic.GetApplicantsByChoice(department, SelectedSession,SelectedInstitutionChoice,SelectedApplicationFormSetting);

                    if (report.Count > 0)
                    {
                        string reportPath = "";
                        string bind_ds = "dsScreeningReport";
                        reportPath = @"Reports\ScreeningReport.rdlc";
                        if (SelectedApplicationFormSetting.Id == 3)
                        {
                             reportPath = @"Reports\ScreeningReportDE.rdlc";
                        }
                        var rptViewer = new ReportViewer();
                        rptViewer.Visible = false;
                        rptViewer.Reset();
                        rptViewer.LocalReport.DisplayName = "Applicants Result By Choice";
                        rptViewer.ProcessingMode = ProcessingMode.Local;
                        rptViewer.LocalReport.ReportPath = reportPath;
                        rptViewer.LocalReport.DataSources.Add(new ReportDataSource(bind_ds.Trim(), report));

                        byte[] bytes = rptViewer.LocalReport.Render("PDF", null, out mimeType, out encoding,out extension, out streamIds, out warnings);

                       
                        string path = Server.MapPath("~/Content/temp");
                        string savelocation = Path.Combine(path, department.Name.Replace("/","").Replace("\\","") + ".pdf");
                        System.IO.File.WriteAllBytes(savelocation, bytes);
                    }
                }
                
                using (var zip = new ZipFile())
                {
                    string file = Server.MapPath("~/Content/temp/");
                    zip.AddDirectory(file, "");
                    string zipFileName = "ApplicantReport";
                    zip.Save(file + zipFileName + ".zip");
                    string export = "~/Content/temp/" + zipFileName + ".zip";

                    var urlHelp = new UrlHelper(System.Web.HttpContext.Current.Request.RequestContext);

                    Response.Redirect(export);

                }


            }
            catch (Exception)
            {
                    
                throw;
            }
        }
    }
}