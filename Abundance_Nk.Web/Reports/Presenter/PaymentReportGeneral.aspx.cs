using Abundance_Nk.Business;
using Abundance_Nk.Model.Model;
using Abundance_Nk.Web.Models;
using Microsoft.Reporting.WebForms;
using Microsoft.Reporting.WebForms.Internal.Soap.ReportingServices2005.Execution;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Windows.Forms;

namespace Abundance_Nk.Web.Reports.Presenter
{
    public partial class PaymentReportGeneral : System.Web.UI.Page
    {
        //private List<Department> departments;


        public Programme Programme
        {
            get { return new Programme() { Id = Convert.ToInt32(ddlProgramme.SelectedValue), Name = ddlProgramme.SelectedItem.Text }; }
            set { ddlProgramme.SelectedValue = value.Id.ToString(); }
        }
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
            set { ddlDepartment.SelectedValue = value.Id.ToString(); }

        }

        public FeeType SelectedFeeTypes
        {
            get
            {
                return new FeeType
                {
                    Id = Convert.ToInt32(ddlFeeTypes.SelectedValue),
                    Name = ddlFeeTypes.SelectedItem.Text
                };
            }
            set { ddlFeeTypes.SelectedValue = value.Id.ToString(); }
        }

       

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                lblMessage.Text = "";
                if (!IsPostBack)
                {
                   
                    PopulateAllDropDown();
                    ddlDepartment.Visible = false;
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
                List<FeeType> feeTypes = Utility.GetAllFeeTypes();
                List<Programme> programmes = Utility.GetAllProgrammes();

                feeTypes.Insert(1,new FeeType() { Id = 1001, Name = "All" });
                programmes.Insert(1,new Programme() { Id = 1001, Name = "All" });

                Utility.BindDropdownItem(ddlFeeTypes, feeTypes, Utility.ID, Utility.NAME);
                Utility.BindDropdownItem(ddlProgramme, programmes, Utility.ID, Utility.NAME);
                    
            }
            catch (Exception ex)
            {
                lblMessage.Text = ex.Message;
            }
        }

        private void DisplayReportBy(FeeType Feetype, Programme programme, Department department)
        {
            try
            {
                PaymentLogic paymentLogic = new PaymentLogic();
                var payments = new List<AllPaymentGeneralView>();
                payments = paymentLogic.GetPaymentReportBy(Feetype, department,Programme, txtBoxDateTo.Text, txtBoxDateFrom.Text);
               
                string reportPath = @"Reports\PaymentReports\PaymentReportGeneral.rdlc";
               

                rv.Reset();
                rv.LocalReport.DisplayName = "StudentFees Payment";
                rv.LocalReport.ReportPath = reportPath;

                if (payments != null)
                {
                    rv.ProcessingMode = ProcessingMode.Local;
                    rv.LocalReport.DataSources.Add(new ReportDataSource("PaymentReportGeneralDataSet1", payments));
                    rv.LocalReport.Refresh();
                }
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
                
                if (SelectedDepartment == null || SelectedDepartment.Id <= 0)
                {
                    lblMessage.Text = "Please select Programme";
                    return true;
                }
                if (SelectedFeeTypes == null || SelectedFeeTypes.Id <= 0)
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

                DisplayReportBy(SelectedFeeTypes,Programme,SelectedDepartment);
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
                    ddlProgramme.Visible = false;
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
                List<Department> departments = new List<Department>();
                DepartmentLogic departmentLogic = new DepartmentLogic();

                if (programme.Id == 1001)
                {
                    departments = departmentLogic.GetAll();
                    departments.Insert(0, new Department() { Id = 0, Name = "-- Select Department --" });
                }
                else
                {
                    departments = Utility.GetDepartmentByProgramme(programme);
                }

                if (departments != null && departments.Count > 0)
                {
                    departments = departments.OrderBy(d => d.Name).ToList();
                    departments.Insert(1,new Department() { Id = 1001, Name = "All" });
                    Utility.BindDropdownItem(ddlDepartment, departments, Utility.ID, Utility.NAME);
                    ddlDepartment.Visible = true;

                }

            }
            catch (Exception ex)
            {
                lblMessage.Text = ex.Message;
            }
        }
        //protected void btnDisplayReport_Click(object sender, EventArgs e)
        //{
        //    try
        //    {
        //        if (InvalidUserInput())
        //        {
        //            return;
        //        }

        //        DisplayReportBy(SelectedFeeTypes,Programme,SelectedDepartment ,rblSortOption.SelectedValue);
        //    }
        //    catch (Exception ex)
        //    {
        //        lblMessage.Text = ex.Message;
        //    }
        //} 

    }
}