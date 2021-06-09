using Abundance_Nk.Business;
using Abundance_Nk.Model.Model;
using Abundance_Nk.Web.Areas.Student.ViewModels;
using Abundance_Nk.Web.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Abundance_Nk.Web.Areas.Student.Controllers
{
    public class PaymentClearanceController : BaseController
    {
        private readonly OnlinePaymentLogic onlinePaymentLogic;
        private readonly PaymentEtranzactLogic paymentEtranzactLogic;
        private readonly PaymentLogic paymentLogic;
        private readonly FeeTypeLogic feeTypeLogic;
        private readonly SessionLogic sessionLogic;
        private readonly PersonLogic personLogic;
        private readonly StudentLevelLogic studentLevelLogic;
        private readonly StudentLogic studentLogic;
        private readonly StudentPaymentLogic studentPaymentLogic;
        private readonly PaymentViewModel viewModel;
        private readonly FeeDetailLogic feeDetailLogic;
        public PaymentClearanceController()
        {
            personLogic = new PersonLogic();
            paymentLogic = new PaymentLogic();
            onlinePaymentLogic = new OnlinePaymentLogic();
            studentLevelLogic = new StudentLevelLogic();
            studentLogic = new StudentLogic();
            studentPaymentLogic = new StudentPaymentLogic();
            paymentEtranzactLogic = new PaymentEtranzactLogic();
            feeTypeLogic = new FeeTypeLogic();
            sessionLogic = new SessionLogic();
            viewModel = new PaymentViewModel();
            feeDetailLogic = new FeeDetailLogic();
        }

        // GET: Student/PaymentClearance
        public ActionResult Index()
        {
            try
            {

                viewModel.Student = studentLogic.GetModelsBy(u => u.Matric_Number == User.Identity.Name).FirstOrDefault();
                if (viewModel.Student != null)
                {
                    var studentCurrentLevel = studentLevelLogic.GetModelBy(x => x.Person_Id == viewModel.Student.Id);
                    if (studentCurrentLevel != null)
                    {
                        //viewModel.PaymentClearances=GetAllDuePayment(studentCurrentLevel);
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return View();
        }
        public List<PaymentClearance> GetAllDuePayment(StudentLevel studentLevel)
        {
            PaymentClearance paymentClearance = new PaymentClearance();
            List<PaymentClearance> listofPayment = new List<PaymentClearance>();
            try
            {
                List<long> levelIdList = new List<long>();
                List<int> sessionIdList = new List<int>();
                int levelCount = 1;
                int sessionCount = 1;
                for (int i=0; levelCount <= studentLevel.Level.Id;i++)
                {
                    long lvlId = studentLevel.Level.Id - i;
                    levelIdList.Add(lvlId);
                    levelCount++;
                }
                for (int i = 0; sessionCount <= studentLevel.Session.Id; i++)
                {
                    int sessionId = studentLevel.Session.Id - i;
                    sessionIdList.Add(sessionId);
                    sessionCount++;
                }
                for (int i=0; i<levelIdList.Count; i++)
                {
                    long levelId = levelIdList[i];
                    int sessId = sessionIdList[i];
                    if (levelId == 1)
                    {
                        var paymentDetail = feeDetailLogic.GetModelsBy(x => x.DEPARTMENT.Department_Id == studentLevel.Department.Id && x.LEVEL.Level_Id == levelId && x.PROGRAMME.Programme_Id == studentLevel.Programme.Id && x.SESSION.Session_Id == sessId && x.PAYMENT_MODE.Payment_Mode_Id == 1);
                        if (paymentDetail != null)
                        {
                            paymentClearance.SessionName = paymentDetail.FirstOrDefault().Session.Name;
                            paymentClearance.TotalAmount = paymentDetail.Sum(x => x.Fee.Amount);
                            listofPayment.Add(paymentClearance);
                        }
                    }
                    else
                    {
                        var paymentDetail = feeDetailLogic.GetModelsBy(x => x.DEPARTMENT.Department_Id == studentLevel.Department.Id && x.LEVEL.Level_Id == levelId && x.PROGRAMME.Programme_Id == studentLevel.Programme.Id && x.SESSION.Session_Id == sessId && x.PAYMENT_MODE.Payment_Mode_Id == 1 && !x.FEE.Fee_Name.Contains("Acceptance Fee"));
                        if (paymentDetail != null)
                        {
                            paymentClearance.SessionName = paymentDetail.FirstOrDefault().Session.Name;
                            paymentClearance.TotalAmount = paymentDetail.Sum(x => x.Fee.Amount);
                            listofPayment.Add(paymentClearance);
                        }
                    }

                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
            return listofPayment;
        }
    }
}