using System;
using System.Collections.Generic;
using System.IO;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;
using Abundance_Nk.Business;
using Abundance_Nk.Model.Model;
using Abundance_Nk.Web.Models;
using DocumentFormat.OpenXml.Office2010.ExcelAc;
using Ionic.Zip;
using Microsoft.Reporting.WebForms;

namespace Abundance_Nk.Web.Reports.Presenter
{
    public partial class AdmittedStudentDataReport : Page
    {
        private List<Department> departments;
        public Session SelectedSession
        {
            get
            {
                return new Session { Id = Convert.ToInt32(ddlSession.SelectedValue), Name = ddlSession.SelectedItem.Text };
            }
            set { ddlSession.SelectedValue = value.Id.ToString(); }
        }
        public Department SelectedDepartment
        {
            get
            {
                return new Department { Id = Convert.ToInt32(ddlDepartment.SelectedValue), Name = ddlDepartment.SelectedItem.Text };
            }
            set { ddlDepartment.SelectedValue = value.Id.ToString(); }
        }
        public Programme SelectedProgramme
        {
            get
            {
                return new Programme { Id = Convert.ToInt32(ddlProgramme.SelectedValue), Name = ddlProgramme.SelectedItem.Text };
            }
            set { ddlProgramme.SelectedValue = value.Id.ToString(); }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                lblMessage.Text = "";

                if (!IsPostBack)
                {
                    Utility.BindDropdownItem(ddlSession, Utility.GetAllSessions(), Utility.ID, Utility.NAME);
                    Utility.BindDropdownItem(ddlProgramme, Utility.GetAllProgrammes(), Utility.ID, Utility.NAME);
                    ddlDepartment.Visible = false;
                }
            }
            catch (Exception ex)
            {
                lblMessage.Text = ex.Message + ex.InnerException.Message;
            }
        }

        private void DisplayReportBy(Department department, Programme programme, Session session)
        {
            string folderName = @"~/Content/StudentDataReport/";
            try
            {
                Warning[] warnings;
                string[] streamIds;
                string mimeType = string.Empty;
                string encoding = string.Empty;
                string extension = string.Empty;

                AdmissionListLogic admissionListLogic = new AdmissionListLogic();
                StudentLogic studentLogic = new StudentLogic();
                OLevelResultLogic oLevelResultLogic = new OLevelResultLogic();
                List<AdmissionList> admissionLists = admissionListLogic.GetByAdmissionLists(department, programme, session);

                if (Directory.Exists(Server.MapPath(folderName)))
                {
                    Directory.Delete(Server.MapPath(folderName), true);
                    Directory.CreateDirectory(Server.MapPath(folderName));
                }
                else
                {
                    Directory.CreateDirectory(Server.MapPath(folderName));
                }
               
                for (int i = 0; i < admissionLists.Count; i++)
                {
                    var person = admissionLists[i].Form.Person;

                    List<AdmittedStudentDataView> report = studentLogic.GetAdmittedStudentDataBy(department, programme, session, person);
                    List<OlevelSitting> firstSittingOlevelResult = oLevelResultLogic.GetOlevelFirstSettingsBy(report);
                    List<OlevelSitting> secondSittingOlevelResult = oLevelResultLogic.GetOlevelSecondSettingsBy(report);

                    string bind_dsStudentDataSummary = "dsStudentData";
                    string bind_dsStudentFirstSitting = "dsFirstSittingResult";
                    string bind_dsStudentSecondSitting = "dsSecondSittingResult";
                    string reportPath = @"Reports\AdmittedStudentDataReport.rdlc";

                    rv.Reset();
                    rv.LocalReport.DisplayName = "Student Data Report ";
                    rv.LocalReport.ReportPath = reportPath;
                    rv.LocalReport.EnableExternalImages = true;

                    if (report != null)
                    {
                        rv.ProcessingMode = ProcessingMode.Local;
                        rv.LocalReport.DataSources.Add(new ReportDataSource(bind_dsStudentDataSummary.Trim(), report));
                        rv.LocalReport.DataSources.Add(new ReportDataSource(bind_dsStudentFirstSitting.Trim(), firstSittingOlevelResult));
                        rv.LocalReport.DataSources.Add(new ReportDataSource(bind_dsStudentSecondSitting.Trim(), secondSittingOlevelResult));
                        rv.LocalReport.Refresh();
                        
                        byte[] bytes = rv.LocalReport.Render("PDF", null, out mimeType, out encoding, out extension, out streamIds, out warnings);
                        string name = "studentData_" + person.Id;
                        string path = Server.MapPath(folderName);
                        string savelocation = Path.Combine(path, name + ".pdf");
                        File.WriteAllBytes(savelocation, bytes);

                    }
                }
                using (var zip = new ZipFile())
                {
                    string file = Server.MapPath(folderName);
                    zip.AddDirectory(file, "");
                    string zipFileName = SelectedProgramme.Name + "_" + SelectedDepartment.Name;
                    zip.Save(file + zipFileName + ".zip");
                    string export = folderName + zipFileName + ".zip";
                    Response.Redirect(export, false);

                   
                }
            }
            catch (Exception ex)
            {
                lblMessage.Text = ex.Message + ex.InnerException.Message;
                ;
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

                DisplayReportBy(SelectedDepartment, SelectedProgramme, SelectedSession);
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
                if (SelectedDepartment == null || SelectedDepartment.Id <= 0 || SelectedProgramme == null ||
                     SelectedProgramme.Id <= 0 || SelectedSession.Id <= 0 || SelectedSession == null)
                {
                    return true;
                }

                return false;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        protected void ddlProgramme_SelectedIndexChanged(object sender, EventArgs e)
        {
            var programme = new Programme { Id = Convert.ToInt32(ddlProgramme.SelectedValue) };
            var departmentLogic = new DepartmentLogic();
            departments = departmentLogic.GetBy(programme);
            Utility.BindDropdownItem(ddlDepartment, departments, Utility.ID, Utility.NAME);
            ddlDepartment.Visible = true;
        }
    }
}