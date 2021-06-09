using Abundance_Nk.Business;
using Abundance_Nk.Model.Model;
using Abundance_Nk.Web.Areas.Student.Models;
using Abundance_Nk.Web.Areas.Student.ViewModels;
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
    public partial class StudentPaymentClearanceReport : System.Web.UI.Page
    {
        public Session SelectedSession
        {
            get
            {
                return new Session { Id = Convert.ToInt32(ddlSession.SelectedValue), Name = ddlSession.SelectedItem.Text };
            }
            set { ddlSession.SelectedValue = value.Id.ToString(); }
        }
        public Level SelectedLevel
        {
            get { return new Level { Id = Convert.ToInt32(ddlLevel.SelectedValue), Name = ddlLevel.SelectedItem.Text }; }
            set { ddlLevel.SelectedValue = value.Id.ToString(); }
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                lblMessage.Text = "";

                if (!IsPostBack)
                {
                    Utility.BindDropdownItem(ddlSession, Utility.GetAllSessions(), Utility.ID, Utility.NAME);


                    Utility.BindDropdownItem(ddlLevel, Utility.GetAllLevels(), Utility.ID, Utility.NAME);

                }
            }
            catch (Exception ex)
            {
                lblMessage.Text = ex.Message + ex.InnerException.Message;
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
                StudentLogic studentLogic = new StudentLogic();
                StudentLevelLogic studentLevelLogic = new StudentLevelLogic();
                StudentLevel studentLevel = new StudentLevel();
                Student student = new Student();
                student = studentLogic.GetModelsBy(u => u.Matric_Number == User.Identity.Name).FirstOrDefault();
                if (student == null)
                {
                    ReportViewer1.Reset();
                    lblMessage.Text = "Please, login to continue";
                    return;
                }
                if (student != null)
                {
                    studentLevel = studentLevelLogic.GetModelBy(x => x.Person_Id == student.Id);
                    if (studentLevel != null)
                    {
                        var validated = ValidateSessionLevel(SelectedSession, SelectedLevel, student, studentLevel);
                        if (!validated)
                        {
                            ReportViewer1.Reset();
                            lblMessage.Text = "The Session-Level Selected does not Exist ";
                            return;
                        }
                        DisplayReportBy(student,SelectedSession, studentLevel.Department, studentLevel.Programme, SelectedLevel);
                    }
                }
                
            }
            catch (Exception ex)
            {
                lblMessage.Text = ex.Message + ex.InnerException.Message;
                return;
            }
        }

        private bool InvalidUserInput()
        {
            try
            {
                if (SelectedSession == null || SelectedSession.Id <= 0 || SelectedLevel==null || SelectedLevel.Id<=0)
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
        private void DisplayReportBy( Student student,Session session, Department department, Programme programme, Level level)
        {
            try
            {
                List<PersonReportModel> listPersonReportModel = new List<PersonReportModel>();
                PersonReportModel personReportModel = new PersonReportModel();
                
                var duePaymentList=AllDuePayment(session, department, programme, level);
                var madePaymentList = PaymentMade(student, session);
                personReportModel.LevelName = level.Name;
                personReportModel.SessionName = session.Name;
                personReportModel.FullName = student.FullName.ToUpper();
                personReportModel.Department = department.Name;
                personReportModel.Programme = programme.Name;
                personReportModel.MatricNo = student.MatricNumber;
                personReportModel.Date = DateTime.Now.Date.ToShortDateString();
                listPersonReportModel.Add(personReportModel);
                string reportPath = Server.MapPath("~/Reports/PaymentClearance.rdlc");
                ReportViewer1.Reset();
                ReportViewer1.LocalReport.DisplayName = "Student Payment Clearance Report ";
                ReportViewer1.LocalReport.ReportPath = reportPath;
                ReportDataSource rdc2 = new ReportDataSource("dsPaymentclearanceDue", duePaymentList);
                ReportDataSource rdc3 = new ReportDataSource("dsPaymentclearancePaid", madePaymentList);
                ReportDataSource rdc1 = new ReportDataSource("dsStudentInfo", listPersonReportModel);
                ReportViewer1.LocalReport.DataSources.Add(rdc1);
                ReportViewer1.LocalReport.DataSources.Add(rdc2);
                ReportViewer1.LocalReport.DataSources.Add(rdc3);
                ReportViewer1.LocalReport.Refresh();
                ReportViewer1.DataBind();
            }
            catch (Exception ex)
            {
                lblMessage.Text = ex.Message + ex.InnerException.Message;
                ;
            }
        }
        private List<PaymentClearance> AllDuePayment(Session session, Department department, Programme programme, Level level)
        {
            
            List<PaymentClearance> listofPayment = new List<PaymentClearance>();
            try
            {
                FeeDetailLogic feeDetailLogic = new FeeDetailLogic();
                FeeDetail feeDetail = new FeeDetail();
                if (level.Id == 1)
                {
                    var groupedPaymentDetails = feeDetailLogic.GetModelsBy(x => x.DEPARTMENT.Department_Id == department.Id && x.LEVEL.Level_Id == level.Id && x.PROGRAMME.Programme_Id == programme.Id && x.SESSION.Session_Id == session.Id && x.PAYMENT_MODE.Payment_Mode_Id == 1 && x.FEE_TYPE.Fee_Type_Id==2|| x.FEE_TYPE.Fee_Type_Id == 3).OrderBy(x => x.FeeType.Id).GroupBy(x=>x.FeeType.Id);
                    if (groupedPaymentDetails != null)
                    {
                        foreach(var groupedPaymentDetail in groupedPaymentDetails)
                        {
                            var specificFee=feeDetailLogic.GetModelsBy(x => x.DEPARTMENT.Department_Id == department.Id && x.LEVEL.Level_Id == level.Id && x.PROGRAMME.Programme_Id == programme.Id && x.SESSION.Session_Id == session.Id && x.PAYMENT_MODE.Payment_Mode_Id == 1 && x.FEE_TYPE.Fee_Type_Id==groupedPaymentDetail.Key);
                            if (specificFee != null)
                            {
                                PaymentClearance paymentClearance = new PaymentClearance();
                                paymentClearance.SessionName = specificFee.FirstOrDefault().Session.Name;
                                paymentClearance.TotalAmount = specificFee.Sum(x => x.Fee.Amount);
                                paymentClearance.Feetype = specificFee.FirstOrDefault().FeeType.Name.ToUpper();
                                listofPayment.Add(paymentClearance);
                            }
                        }
                    }
                }
                else
                {
                    var groupedPaymentDetails = feeDetailLogic.GetModelsBy(x => x.DEPARTMENT.Department_Id == department.Id && x.LEVEL.Level_Id == level.Id && x.PROGRAMME.Programme_Id == programme.Id && x.SESSION.Session_Id == session.Id && x.PAYMENT_MODE.Payment_Mode_Id == 1 && !x.FEE.Fee_Name.Contains("Acceptance Fee") && x.FEE_TYPE.Fee_Type_Id == 3).OrderBy(x => x.FeeType.Id).GroupBy(x => x.FeeType.Id);
                    if (groupedPaymentDetails != null)
                    {
                        foreach (var groupedPaymentDetail in groupedPaymentDetails)
                        {
                            var specificFee = feeDetailLogic.GetModelsBy(x => x.DEPARTMENT.Department_Id == department.Id && x.LEVEL.Level_Id == level.Id && x.PROGRAMME.Programme_Id == programme.Id && x.SESSION.Session_Id == session.Id && x.PAYMENT_MODE.Payment_Mode_Id == 1 && x.FEE_TYPE.Fee_Type_Id == groupedPaymentDetail.Key);
                            if (specificFee != null)
                            {
                                PaymentClearance paymentClearance = new PaymentClearance();
                                paymentClearance.SessionName = specificFee.FirstOrDefault().Session.Name;
                                paymentClearance.TotalAmount = specificFee.Sum(x => x.Fee.Amount);
                                paymentClearance.Feetype = specificFee.FirstOrDefault().FeeType.Name.ToUpper();
                                listofPayment.Add(paymentClearance);
                            }
                        }
                    }
                }


            }
            catch(Exception ex)
            {
                throw ex;
            }
            return listofPayment;
        }
        private List<PaymentClearance> PaymentMade(Student student, Session session)
        {
            
            List<PaymentClearance> listofPayment = new List<PaymentClearance>();
            try
            {
                PaymentLogic paymentLogic = new PaymentLogic();
                var paystackPayment=paymentLogic.GetPaystackBy(student, session);
                if (paystackPayment != null && paystackPayment.Count>0)
                {
                    var groupByFeetypes= paystackPayment.OrderBy(x=>x.FeeTypeId).GroupBy(x => x.FeeTypeId);
                    foreach(var groupByFeetype in groupByFeetypes)
                    {
                        var remitaSpecific= paystackPayment.Where(x => x.FeeTypeId == groupByFeetype.Key);
                        PaymentClearance paymentClearance = new PaymentClearance();
                        paymentClearance.SessionName = remitaSpecific.FirstOrDefault().SessionName;
                        paymentClearance.Feetype = remitaSpecific.FirstOrDefault().FeeTypeName.ToUpper();
                        var amount = (decimal)remitaSpecific.Sum(x => x.Amount);
                        paymentClearance.TotalAmount = Math.Round(amount, 2);
                        listofPayment.Add(paymentClearance);

                    }
                }
                var etranzactPayment=paymentLogic.GetEtranzactBy(student, session);
                if (etranzactPayment != null && etranzactPayment.Count>0)
                {
                    var groupEtranzactPaymentByFeetypes = etranzactPayment.GroupBy(x => x.FeeTypeId);
                    foreach(var groupEtranzactPaymentByFeetype in groupEtranzactPaymentByFeetypes)
                    {
                        var etranzactSpecific = etranzactPayment.Where(x => x.FeeTypeId == groupEtranzactPaymentByFeetype.Key);
                        PaymentClearance paymentClearance = new PaymentClearance();
                        paymentClearance.SessionName = etranzactSpecific.FirstOrDefault().SessionName;
                        paymentClearance.Feetype = etranzactSpecific.FirstOrDefault().FeeTypeName.ToUpper();
                        var amount= (decimal)etranzactSpecific.Sum(x => x.Amount);
                        paymentClearance.TotalAmount = Math.Round(amount, 2);
                        if(paymentClearance.TotalAmount>0)
                        {
                            listofPayment.Add(paymentClearance);
                        }
                        
                    }
                }
            }
            catch(Exception ex)
            {
                throw ex;
            }
            return listofPayment;
        }
        private bool ValidateSessionLevel(Session session, Level level, Student student, StudentLevel studentLevel)
        {
            try
            {
                CourseRegistrationLogic courseRegistrationLogic = new CourseRegistrationLogic();
                var exist=courseRegistrationLogic.GetModelsBy(x => x.Session_Id == session.Id && x.Level_Id == level.Id && x.Person_Id == student.Id && x.Department_Id == studentLevel.Department.Id && x.Programme_Id == studentLevel.Programme.Id).LastOrDefault();
                if (exist != null)
                {
                    return true;
                }
            }
            catch(Exception ex)
            {
                throw ex;
            }
            return false;
        }
    }
}