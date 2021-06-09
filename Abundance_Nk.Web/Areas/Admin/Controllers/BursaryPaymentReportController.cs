using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using System.Web.Script.Services;
using Abundance_Nk.Business;
using Abundance_Nk.Model.Model;
using Abundance_Nk.Web.Controllers;
using Newtonsoft.Json;

namespace Abundance_Nk.Web.Areas.Admin.Controllers
{
    public class BursaryPaymentReportController : BaseController
    {
        // GET: Admin/BursaryPayment
        public ActionResult PaymentReport()
        {
            return View();
        }
        public ActionResult PaymentReportByBreakDown()
        {
            return View();
        }
        public ContentResult GetPaymentReport(string dateFrom, string dateTo, string sortOption)
        {
            List<PaymentReportArray> paymentReportArrayList = new List<PaymentReportArray>();
            PaymentReportArray paymentReportArray = new PaymentReportArray();
            var result = new ContentResult();
            try
            {

                if (string.IsNullOrEmpty(dateFrom) && string.IsNullOrEmpty(dateTo) && string.IsNullOrEmpty(sortOption))
                {
                    return null;
                }
                JavaScriptSerializer serializer = new JavaScriptSerializer() { MaxJsonLength = Int32.MaxValue };
                int sortNumber = Convert.ToInt32(sortOption);
                if (sortNumber == 1)
                {
                    PaymentEtranzactLogic paymentEtranzactLogic = new PaymentEtranzactLogic();
                    List<PaymentEtranzactView> paymentEtranzactView = paymentEtranzactLogic.GetPaymentBy(dateFrom, dateTo);
                    if (paymentEtranzactView != null && paymentEtranzactView.Count > 0)
                    {
                        List<int> feeTypIdList = paymentEtranzactView.Select(s => s.FeeTypeId).Distinct().ToList();
                        for (int i = 0; i < feeTypIdList.Count; i++)
                        {
                            paymentReportArray = new PaymentReportArray();
                            var feeType = paymentEtranzactView.Where(s => s.FeeTypeId == feeTypIdList[i]);
                            decimal totalAmount = Convert.ToDecimal(feeType.Sum(s => s.TransactionAmount));
                            paymentReportArray.FeeTypeId = feeTypIdList[i].ToString();
                            paymentReportArray.FeeTypeName = feeType.FirstOrDefault().FeeTypeName;
                            paymentReportArray.Count = feeType.Count().ToString();
                            paymentReportArray.Amount = String.Format("{0:N}", totalAmount);
                            paymentReportArray.IsError = false;
                            paymentReportArrayList.Add(paymentReportArray);
                        }
                        paymentReportArray.PaymentEtranzactView = serializer.Serialize(paymentEtranzactView);

                    }
                    else
                    {
                        paymentReportArray.IsError = true;
                        paymentReportArray.ErrorMessage = "Error Occured Invalid parameter";
                        paymentReportArrayList.Add(paymentReportArray);
                    }
                }
                else if (sortNumber == 2)
                {
                    PaystackLogic paystackLogic = new PaystackLogic();
                    List<PaymentPaystackView> paymentEtranzactView = paystackLogic.GetPaymentBy(dateFrom, dateTo);
                    if (paymentEtranzactView != null && paymentEtranzactView.Count > 0)
                    {
                        List<int> feeTypIdList = paymentEtranzactView.Select(s => s.FeeTypeId).Distinct().ToList();
                        for (int i = 0; i < feeTypIdList.Count; i++)
                        {
                            paymentReportArray = new PaymentReportArray();
                            var feeType = paymentEtranzactView.Where(s => s.FeeTypeId == feeTypIdList[i]);
                            decimal totalAmount = Convert.ToDecimal(feeType.Sum(s => s.TransactionAmount));
                            paymentReportArray.FeeTypeId = feeTypIdList[i].ToString();
                            paymentReportArray.FeeTypeName = feeType.FirstOrDefault().FeeTypeName;
                            paymentReportArray.Count = feeType.Count().ToString();
                            paymentReportArray.Amount = String.Format("{0:N}", totalAmount);
                            paymentReportArray.IsError = false;
                            paymentReportArrayList.Add(paymentReportArray);
                        }
                        paymentReportArray.PaymentEtranzactView = serializer.Serialize(paymentEtranzactView);

                    }
                    else
                    {
                        paymentReportArray.IsError = true;
                        paymentReportArray.ErrorMessage = "Error Occured No Records found for the selected date range";
                        paymentReportArrayList.Add(paymentReportArray);
                    }
                }

                var serializedList = serializer.Serialize(paymentReportArrayList);
                result = new ContentResult
                {
                    Content = serializedList,
                    ContentType = "application/json"
                };
                return result;
            }
            catch (Exception ex)
            {

                paymentReportArray.IsError = true;
                paymentReportArray.ErrorMessage = "Error Occured" + ex;
                paymentReportArrayList.Add(paymentReportArray);
            }
            return result;
        }

        public ContentResult GetFeeTypeDetails(string feeTypeId, string dateFrom, string dateTo, string sortOption, string paymentReportData)
        {
            List<PaymentReportArray> paymentReportArrayList = new List<PaymentReportArray>();
            try
            {
                JavaScriptSerializer serializer = new JavaScriptSerializer() { MaxJsonLength = Int32.MaxValue };
                if (string.IsNullOrEmpty(dateFrom) && string.IsNullOrEmpty(dateTo) && string.IsNullOrEmpty(sortOption) && string.IsNullOrEmpty(feeTypeId))
                {
                    return null;
                }

                int sortNumber = Convert.ToInt32(sortOption);
                int feeTypeid = Convert.ToInt32(feeTypeId);
                if (sortNumber == 1)
                {
                    List<PaymentEtranzactView> paymentEtranzactReportView = serializer.Deserialize<List<PaymentEtranzactView>>(paymentReportData);
                    var feeTypepaymentEtranzactView = paymentEtranzactReportView.Where(s => s.FeeTypeId == feeTypeid).ToList();

                    for (int i = 0; i < feeTypepaymentEtranzactView.Count; i++)
                    {
                        PaymentReportArray paymentReportArray = new PaymentReportArray();
                        paymentReportArray.FullName = feeTypepaymentEtranzactView[i].FullName;
                        paymentReportArray.MatricNumber = feeTypepaymentEtranzactView[i].MatricNumber ?? "-";
                        paymentReportArray.JambNumber = feeTypepaymentEtranzactView[i].JambNumber;
                        paymentReportArray.Amount = String.Format("{0:N}", feeTypepaymentEtranzactView[i].TransactionAmount);
                        paymentReportArray.LevelName = feeTypepaymentEtranzactView[i].LevelName ?? "-";
                        paymentReportArray.DepartmentName = feeTypepaymentEtranzactView[i].DepartmentName ?? "-";
                        paymentReportArray.FacultyName = feeTypepaymentEtranzactView[i].FacultyName ?? "-";
                        paymentReportArray.ProgrammeName = feeTypepaymentEtranzactView[i].ProgrammeName ?? "-";
                        paymentReportArray.SessionName = feeTypepaymentEtranzactView[i].SessionName;
                        paymentReportArray.TransactionDate = feeTypepaymentEtranzactView[i].TransactionDate.ToString();
                        paymentReportArray.ConfirmationNo = feeTypepaymentEtranzactView[i].ConfirmationNo ?? "-";
                        paymentReportArray.InvoiceNumber = feeTypepaymentEtranzactView[i].InvoiceNumber;
                        paymentReportArray.PaymentMode = feeTypepaymentEtranzactView[i].PaymentMode;
                        paymentReportArrayList.Add(paymentReportArray);

                    }
                }
                if (sortNumber == 2)
                {
                    List<PaymentPaystackView> paystackView = serializer.Deserialize<List<PaymentPaystackView>>(paymentReportData);
                    var feeTypepaystackView = paystackView.Where(s => s.FeeTypeId == feeTypeid).ToList().ToList();

                    for (int i = 0; i < feeTypepaystackView.Count; i++)
                    {
                        PaymentReportArray paymentReportArray = new PaymentReportArray();
                        paymentReportArray.FullName = feeTypepaystackView[i].FullName;
                        paymentReportArray.MatricNumber = feeTypepaystackView[i].MatricNumber ?? "-";
                        paymentReportArray.LevelName = feeTypepaystackView[i].LevelName ?? "-";
                        paymentReportArray.FacultyName = feeTypepaystackView[i].FacultyName ?? "-";
                        paymentReportArray.DepartmentName = feeTypepaystackView[i].DepartmentName ?? "-";
                        paymentReportArray.ProgrammeName = feeTypepaystackView[i].ProgrammeName ?? "-";
                        paymentReportArray.ConfirmationNo = feeTypepaystackView[i].ConfirmationNo ?? "-";
                        paymentReportArray.JambNumber = feeTypepaystackView[i].JambNumber;
                        paymentReportArray.Amount = String.Format("{0:N}", feeTypepaystackView[i].TransactionAmount);
                        paymentReportArray.SessionName = feeTypepaystackView[i].SessionName;
                        paymentReportArray.TransactionDate = feeTypepaystackView[i].TransactionDate.ToString();
                        paymentReportArray.InvoiceNumber = feeTypepaystackView[i].InvoiceNumber;
                        paymentReportArray.PaymentMode = feeTypepaystackView[i].PaymentMode;
                        paymentReportArrayList.Add(paymentReportArray);

                    }
                }
                var serializedList = serializer.Serialize(paymentReportArrayList);
                var result = new ContentResult
                {
                    Content = serializedList,
                    ContentType = "application/json"
                };
                return result;

            }
            catch (Exception ex)
            {

                throw;
            }
        }
        public JsonResult GetPaymentSummary(string sortOption, string dateFromString, string dateToString)
        {

            PaymentReportDetail singleResult = new PaymentReportDetail();
            List<PaymentReportDetail> result = new List<PaymentReportDetail>();
            try
            {

                if (!string.IsNullOrEmpty(sortOption) && !string.IsNullOrEmpty(dateFromString) && !string.IsNullOrEmpty(dateToString))
                {
                    DateTime dateFrom = new DateTime();
                    DateTime dateTo = new DateTime();

                    if (!DateTime.TryParse(dateFromString, out dateFrom))
                    {
                        singleResult.IsError = true;
                        singleResult.Message = "Date range is not in the correct format.";

                        return Json(singleResult, JsonRequestBehavior.AllowGet);
                    }
                    if (!DateTime.TryParse(dateToString, out dateTo))
                    {
                        singleResult.IsError = true;
                        singleResult.Message = "Date range is not in the correct format.";

                        return Json(singleResult, JsonRequestBehavior.AllowGet);
                    }

                    int option = Convert.ToInt32(sortOption);
                    PaymentLogic paymentLogic = new PaymentLogic();
                    List<PaymentSummary> paymentSummary = paymentLogic.GetPaymentSummary(dateFrom, dateTo);
                    if (paymentSummary != null && paymentSummary.Count > 0)
                    {
                        if (option == 1)
                        {
                            paymentSummary = paymentSummary.Where(p => p.PaymentEtranzactId != null).ToList();
                        }


                        decimal overallAmount = Convert.ToDecimal(paymentSummary.Sum(p => p.TransactionAmount));
                        int overallCount = paymentSummary.Count();

                        List<int> distinctFeeTypeId = paymentSummary.Select(p => p.FeeTypeId).Distinct().ToList();
                        for (int i = 0; i < distinctFeeTypeId.Count; i++)
                        {
                            int currentFeeTypeId = distinctFeeTypeId[i];

                            List<PaymentSummary> feeTypeSummary = paymentSummary.Where(p => p.FeeTypeId == currentFeeTypeId).ToList();
                            decimal feeTypeTotalAmount = Convert.ToDecimal(feeTypeSummary.Sum(p => p.TransactionAmount));

                            PaymentReportDetail paymentJsonResult = new PaymentReportDetail();
                            paymentJsonResult.FeeTypeId = currentFeeTypeId;
                            paymentJsonResult.FeeTypeName = feeTypeSummary.FirstOrDefault().FeeTypeName;
                            paymentJsonResult.TotalAmount = String.Format("{0:N}", feeTypeTotalAmount);
                            paymentJsonResult.TotalCount = feeTypeSummary.Count();
                            paymentJsonResult.IsError = false;
                            paymentJsonResult.OverallAmount = String.Format("{0:N}", overallAmount);
                            paymentJsonResult.OverallCount = overallCount;

                            result.Add(paymentJsonResult);
                        }
                    }

                    return Json(result, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    singleResult.IsError = true;
                    singleResult.Message = "Invalid parametr! ";
                }
            }
            catch (Exception ex)
            {
                singleResult.IsError = true;
                singleResult.Message = "Error! " + ex.Message;
            }

            return Json(singleResult, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetBreakdownByMode(string sortOption, string dateFromString, string dateToString, int feeTypeId)
        {

            PaymentReportDetail singleResult = new PaymentReportDetail();
            List<PaymentReportDetail> result = new List<PaymentReportDetail>();
            try
            {
                if (!string.IsNullOrEmpty(sortOption) && !string.IsNullOrEmpty(dateFromString) && !string.IsNullOrEmpty(dateToString) && feeTypeId > 0)
                {
                    DateTime dateFrom = new DateTime();
                    DateTime dateTo = new DateTime();

                    if (!DateTime.TryParse(dateFromString, out dateFrom))
                    {
                        singleResult.IsError = true;
                        singleResult.Message = "Date range is not in the correct format.";

                        return Json(singleResult, JsonRequestBehavior.AllowGet);
                    }
                    if (!DateTime.TryParse(dateToString, out dateTo))
                    {
                        singleResult.IsError = true;
                        singleResult.Message = "Date range is not in the correct format.";

                        return Json(singleResult, JsonRequestBehavior.AllowGet);
                    }
                    int option = Convert.ToInt32(sortOption);
                    PaymentLogic paymentLogic = new PaymentLogic();
                    List<PaymentSummary> paymentSummary = paymentLogic.GetPaymentSummary(dateFrom, dateTo);
                    if (paymentSummary != null && paymentSummary.Count > 0)
                    {
                        if (option == 1)
                        {
                            paymentSummary = paymentSummary.Where(p => p.PaymentEtranzactId != null && p.FeeTypeId == feeTypeId).ToList();
                        }

                        decimal overallAmount = Convert.ToDecimal(paymentSummary.Sum(p => p.TransactionAmount));
                        int overallCount = paymentSummary.Count();

                        List<int> distinctPaymentModeId = paymentSummary.Select(p => p.PaymentModeId).Distinct().ToList();
                        for (int i = 0; i < distinctPaymentModeId.Count; i++)
                        {
                            int currentModeId = distinctPaymentModeId[i];

                            List<PaymentSummary> paymentModeSummary = paymentSummary.Where(p => p.PaymentModeId == currentModeId).ToList();
                            decimal modeTotalAmount = Convert.ToDecimal(paymentModeSummary.Sum(p => p.TransactionAmount));

                            PaymentReportDetail paymentJsonResult = new PaymentReportDetail();
                            paymentJsonResult.PaymentModeId = currentModeId;
                            paymentJsonResult.PaymentModeName = paymentModeSummary.FirstOrDefault().PaymentModeName;
                            paymentJsonResult.FeeTypeId = feeTypeId;
                            paymentJsonResult.FeeTypeName = paymentModeSummary.FirstOrDefault().FeeTypeName;
                            paymentJsonResult.TotalAmount = String.Format("{0:N}", modeTotalAmount);
                            paymentJsonResult.TotalCount = paymentModeSummary.Count();
                            paymentJsonResult.IsError = false;
                            paymentJsonResult.OverallAmount = String.Format("{0:N}", overallAmount);
                            paymentJsonResult.OverallCount = overallCount;

                            result.Add(paymentJsonResult);
                        }
                    }

                    return Json(result, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    singleResult.IsError = true;
                    singleResult.Message = "Invalid parametr! ";
                }
            }
            catch (Exception ex)
            {
                singleResult.IsError = true;
                singleResult.Message = "Error! " + ex.Message;
            }

            //string serializedResult2 = serializer.Serialize(singleResult);

            //return Content(serializedResult2, "application/json");
            return Json(singleResult, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetBreakdownByProgramme(string sortOption, string dateFromString, string dateToString, int feeTypeId, int paymentModeId)
        {

            PaymentReportDetail singleResult = new PaymentReportDetail();
            List<PaymentReportDetail> result = new List<PaymentReportDetail>();
            //JavaScriptSerializer serializer = new JavaScriptSerializer();
            try
            {
                if (!string.IsNullOrEmpty(sortOption) && !string.IsNullOrEmpty(dateFromString) && !string.IsNullOrEmpty(dateToString) && feeTypeId > 0)
                {
                    DateTime dateFrom = new DateTime();
                    DateTime dateTo = new DateTime();

                    if (!DateTime.TryParse(dateFromString, out dateFrom))
                    {
                        singleResult.IsError = true;
                        singleResult.Message = "Date range is not in the correct format.";

                        return Json(singleResult, JsonRequestBehavior.AllowGet);
                    }
                    if (!DateTime.TryParse(dateToString, out dateTo))
                    {
                        singleResult.IsError = true;
                        singleResult.Message = "Date range is not in the correct format.";

                        return Json(singleResult, JsonRequestBehavior.AllowGet);
                    }

                    int option = Convert.ToInt32(sortOption);
                    PaymentLogic paymentLogic = new PaymentLogic();
                    List<PaymentSummary> paymentSummary = paymentLogic.GetPaymentSummary(dateFrom, dateTo);
                    if (paymentSummary != null && paymentSummary.Count > 0)
                    {
                        if (option == 1)
                        {
                            paymentSummary = paymentSummary.Where(p => p.RRR == null && p.PaymentEtranzactId != null && p.FeeTypeId == feeTypeId && p.PaymentModeId == paymentModeId).ToList();
                        }
                        else
                        {
                            paymentSummary = paymentSummary.Where(p => p.RRR != null && p.PaymentEtranzactId == null && p.Status.Contains("01") && p.FeeTypeId == feeTypeId && p.PaymentModeId == paymentModeId).ToList();
                        }

                        decimal overallAmount = Convert.ToDecimal(paymentSummary.Sum(p => p.TransactionAmount));
                        int overallCount = paymentSummary.Count();

                        ProgrammeLogic programmeLogic = new ProgrammeLogic();
                        List<Programme> programmes = programmeLogic.GetAll();

                        List<int?> distinctProgrammeId = paymentSummary.Select(p => p.ProgrammeId).Distinct().ToList();

                        for (int i = 0; i < distinctProgrammeId.Count; i++)
                        {
                            int currentProgrammeId = Convert.ToInt32(distinctProgrammeId[i]);

                            List<PaymentSummary> programmeSummary = paymentSummary.Where(p => p.ProgrammeId == currentProgrammeId).ToList();
                            decimal programmeTotalAmount = Convert.ToDecimal(programmeSummary.Sum(p => p.TransactionAmount));

                            PaymentReportDetail paymentJsonResult = new PaymentReportDetail();
                            paymentJsonResult.ProgrammeId = currentProgrammeId;
                            paymentJsonResult.ProgrammeName = programmeSummary.FirstOrDefault().ProgrammeName ?? programmes.LastOrDefault(p => p.Id == currentProgrammeId).Name;
                            paymentJsonResult.PaymentModeId = paymentModeId;
                            paymentJsonResult.PaymentModeName = programmeSummary.FirstOrDefault().PaymentModeName;
                            paymentJsonResult.FeeTypeId = feeTypeId;
                            paymentJsonResult.FeeTypeName = programmeSummary.FirstOrDefault().FeeTypeName;
                            paymentJsonResult.TotalAmount = String.Format("{0:N}", programmeTotalAmount);
                            paymentJsonResult.TotalCount = programmeSummary.Count();
                            paymentJsonResult.IsError = false;
                            paymentJsonResult.OverallAmount = String.Format("{0:N}", overallAmount);
                            paymentJsonResult.OverallCount = overallCount;

                            result.Add(paymentJsonResult);
                        }
                    }

                    return Json(result, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    singleResult.IsError = true;
                    singleResult.Message = "Invalid parametr! ";
                }
            }
            catch (Exception ex)
            {
                singleResult.IsError = true;
                singleResult.Message = "Error! " + ex.Message;
            }
            return Json(singleResult, JsonRequestBehavior.AllowGet);
        }
        public ContentResult GetBreakdownByDepartment(string sortOption, string dateFromString, string dateToString, int feeTypeId, int paymentModeId, int programmeId)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer() { MaxJsonLength = Int32.MaxValue };
            PaymentReportDetail singleResult = new PaymentReportDetail();
            List<PaymentReportDetail> result = new List<PaymentReportDetail>();
            string serializedList;
            try
            {
                if (!string.IsNullOrEmpty(sortOption) && !string.IsNullOrEmpty(dateFromString) && !string.IsNullOrEmpty(dateToString) && feeTypeId > 0)
                {

                    DateTime dateFrom = new DateTime();
                    DateTime dateTo = new DateTime();

                    if (!DateTime.TryParse(dateFromString, out dateFrom))
                    {
                        singleResult.IsError = true;
                        singleResult.Message = "Date range is not in the correct format.";

                        serializedList = serializer.Serialize(singleResult);
                        var singleResultlist = new ContentResult()
                        {
                            Content = serializedList,
                            ContentType = "application/json",
                        };
                        return singleResultlist;
                    }
                    if (!DateTime.TryParse(dateToString, out dateTo))
                    {
                        singleResult.IsError = true;
                        singleResult.Message = "Date range is not in the correct format.";

                        serializedList = serializer.Serialize(singleResult);
                        var singleResultlist = new ContentResult()
                        {
                            Content = serializedList,
                            ContentType = "application/json",
                        };
                        return singleResultlist;
                    }
                    int option = Convert.ToInt32(sortOption);
                    PaymentLogic paymentLogic = new PaymentLogic();
                    List<PaymentSummary> paymentSummary = paymentLogic.GetPaymentSummary(dateFrom, dateTo);
                    if (paymentSummary != null && paymentSummary.Count > 0)
                    {
                        if (option == 1)
                        {
                            paymentSummary = paymentSummary.Where(p => p.PaymentEtranzactId != null && p.FeeTypeId == feeTypeId && p.PaymentModeId == paymentModeId &&
                                                p.ProgrammeId == programmeId).ToList();
                        }

                        decimal overallAmount = Convert.ToDecimal(paymentSummary.Sum(p => p.TransactionAmount));
                        int overallCount = paymentSummary.Count();

                        DepartmentLogic departmentLogic = new DepartmentLogic();
                        ProgrammeLogic programmeLogic = new ProgrammeLogic();
                        PaymentReportDetail paymentJsonResult = new PaymentReportDetail();

                        List<int?> distinctDepartmentId = paymentSummary.Select(p => p.DepartmentId).Distinct().ToList();

                        for (int i = 0; i < distinctDepartmentId.Count; i++)
                        {
                            int currentDepartmentId = Convert.ToInt32(distinctDepartmentId[i]);

                            List<PaymentSummary> departmentSummary = paymentSummary.Where(p => p.DepartmentId == currentDepartmentId).ToList();
                            decimal departmentTotalAmount = Convert.ToDecimal(departmentSummary.Sum(p => p.TransactionAmount));

                            paymentJsonResult = new PaymentReportDetail();
                            paymentJsonResult.DepartmentId = currentDepartmentId;
                            paymentJsonResult.DepartmentName = departmentSummary.FirstOrDefault().DepartmentName ?? departmentLogic.GetModelBy(p => p.Department_Id == currentDepartmentId).Name;
                            paymentJsonResult.ProgrammeId = programmeId;
                            paymentJsonResult.ProgrammeName = departmentSummary.FirstOrDefault().ProgrammeName ?? programmeLogic.GetModelBy(p => p.Programme_Id == programmeId).Name;
                            paymentJsonResult.PaymentModeId = paymentModeId;
                            paymentJsonResult.PaymentModeName = departmentSummary.FirstOrDefault().PaymentModeName;
                            paymentJsonResult.FeeTypeId = feeTypeId;
                            paymentJsonResult.FeeTypeName = departmentSummary.FirstOrDefault().FeeTypeName;
                            paymentJsonResult.TotalAmount = String.Format("{0:N}", departmentTotalAmount);
                            paymentJsonResult.TotalCount = departmentSummary.Count();
                            paymentJsonResult.IsError = false;
                            paymentJsonResult.OverallAmount = String.Format("{0:N}", overallAmount);
                            paymentJsonResult.OverallCount = overallCount;

                            result.Add(paymentJsonResult);
                        }
                        paymentJsonResult.PaymentSummary = serializer.Serialize(paymentSummary);

                    }
                    serializedList = serializer.Serialize(result);
                    var serializedResult = new ContentResult
                    {
                        Content = serializedList,
                        ContentType = "application/json"
                    };
                    return serializedResult;

                }
                else
                {
                    singleResult.IsError = true;
                    singleResult.Message = "Invalid parametr! ";
                }
            }
            catch (Exception ex)
            {
                singleResult.IsError = true;
                singleResult.Message = "Error! " + ex.Message;
            }
            serializedList = serializer.Serialize(result);
            var resultlist = new ContentResult()
            {
                Content = serializedList,
                ContentType = "application/json",
            };
            return resultlist;
        }
        public JsonResult GetPaymentSchoolFeeDetails(string sortOption, string dateFromString, string dateToString, int feeTypeId, int paymentModeId, int programmeId, int departmentId, string reportData)
        {

            PaymentReportDetail singleResult = new PaymentReportDetail();
            List<PaymentReportDetail> result = new List<PaymentReportDetail>();
            try
            {
                if (!string.IsNullOrEmpty(sortOption) && !string.IsNullOrEmpty(dateFromString) && !string.IsNullOrEmpty(dateToString) && feeTypeId > 0)
                {

                    JavaScriptSerializer serializer = new JavaScriptSerializer() { MaxJsonLength = Int32.MaxValue };
                    DateTime dateFrom = new DateTime();
                    DateTime dateTo = new DateTime();

                    if (!DateTime.TryParse(dateFromString, out dateFrom))
                    {
                        singleResult.IsError = true;
                        singleResult.Message = "Date range is not in the correct format.";

                        return Json(singleResult, JsonRequestBehavior.AllowGet);
                    }
                    if (!DateTime.TryParse(dateToString, out dateTo))
                    {
                        singleResult.IsError = true;
                        singleResult.Message = "Date range is not in the correct format.";

                        return Json(singleResult, JsonRequestBehavior.AllowGet);
                    }

                    List<PaymentSummary> paymentEtranzactReportView = serializer.Deserialize<List<PaymentSummary>>(reportData);
                    int option = Convert.ToInt32(sortOption);
                    List<PaymentSummary> paymentSummary = new List<PaymentSummary>();
                    if (option == 1)
                    {
                        paymentSummary = paymentEtranzactReportView.Where(s => s.TransactionDate != null && (s.TransactionDate >= dateFrom && s.TransactionDate <= dateTo) && s.DepartmentId == departmentId && s.ProgrammeId == programmeId && s.PaymentModeId == paymentModeId && s.FeeTypeId == feeTypeId).ToList();
                    }

                    if (paymentSummary.Count > 0)
                    {
                        List<FeeDetail> distinctFeeDetails = new List<FeeDetail>();
                        List<FeeDetail> feeDetailList = new List<FeeDetail>();
                        FeeDetailLogic feeDetailLogic = new FeeDetailLogic();
                        List<int> distinctFeeIds = new List<int>();
                        foreach (var summary in paymentSummary)
                        {
                            int sessionId = summary.SessionId;
                            int levelId = summary.LevelId == null ? 1 : Convert.ToInt32(summary.LevelId);

                            List<FeeDetail> feeDetails = feeDetailLogic.GetModelsBy(fd => fd.Fee_Type_Id == feeTypeId &&
                                                                                          fd.Programme_Id == programmeId &&
                                                                                          fd.Session_Id == sessionId &&
                                                                                          fd.Department_Id ==
                                                                                          departmentId &&
                                                                                          fd.Payment_Mode_Id ==
                                                                                          paymentModeId &&
                                                                                  fd.Level_Id == levelId);

                            feeDetails.ForEach(f =>
                            {
                                if (!distinctFeeIds.Contains(f.Fee.Id))
                                {
                                    distinctFeeDetails.Add(f);
                                    distinctFeeIds.Add(f.Fee.Id);
                                }
                            });
                            feeDetailList.AddRange(feeDetails);
                            //List<int> feeIds = feeDetails.Select(s => s.Fee.Id).Distinct().ToList();
                            //feeDetailList = feeDetailList.Count <= 0 ? feeDetails : feeDetailList.Where(f => !feeIds.Contains(f.Fee.Id)).ToList();

                        }

                        foreach (var feeDetail in distinctFeeDetails)
                        {
                            int count = feeDetailList.Count(s => s.Fee.Id == feeDetail.Fee.Id);
                            PaymentReportDetail paymentJsonResult = new PaymentReportDetail();
                            paymentJsonResult.TotalFeeAmount = feeDetail.Fee.Amount * count;
                            paymentJsonResult.FeeName = feeDetail.Fee.Name;
                            paymentJsonResult.TotalCount = count;
                            paymentJsonResult.FeeAmount = feeDetail.Fee.Amount;
                            paymentJsonResult.DepartmentId = departmentId;
                            paymentJsonResult.DepartmentName = feeDetail.Department.Name;
                            paymentJsonResult.ProgrammeId = programmeId;
                            paymentJsonResult.ProgrammeName = feeDetail.Programme.Name;
                            paymentJsonResult.PaymentModeId = paymentModeId;
                            paymentJsonResult.PaymentModeName = feeDetail.PaymentMode.Name;
                            paymentJsonResult.FeeTypeId = feeTypeId;
                            result.Add(paymentJsonResult);
                        }



                    }
                    return Json(result.OrderBy(p => p.Level).ToList(), JsonRequestBehavior.AllowGet);

                }
                else
                {
                    singleResult.IsError = true;
                    singleResult.Message = "Invalid parametr! ";
                }

            }
            catch (Exception ex)
            {
                singleResult.IsError = true;
                singleResult.Message = "Error! " + ex.Message;
            }
            return Json(singleResult, JsonRequestBehavior.AllowGet);
        }
        public ContentResult GetPaymentBreakdown(string sortOption, string dateFromString, string dateToString, int feeTypeId, int paymentModeId, int programmeId, int departmentId)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer() { MaxJsonLength = Int32.MaxValue };
            PaymentReportDetail singleResult = new PaymentReportDetail();
            List<PaymentReportDetail> result = new List<PaymentReportDetail>();
            string serializedList;
            try
            {
                if (!string.IsNullOrEmpty(sortOption) && !string.IsNullOrEmpty(dateFromString) && !string.IsNullOrEmpty(dateToString) && feeTypeId > 0)
                {
                    DateTime dateFrom = new DateTime();
                    DateTime dateTo = new DateTime();

                    if (!DateTime.TryParse(dateFromString, out dateFrom))
                    {
                        singleResult.IsError = true;
                        singleResult.Message = "Date range is not in the correct format.";

                        serializedList = serializer.Serialize(singleResult);
                        var singleResultlist = new ContentResult()
                        {
                            Content = serializedList,
                            ContentType = "application/json",
                        };
                        return singleResultlist;
                    }
                    if (!DateTime.TryParse(dateToString, out dateTo))
                    {
                        singleResult.IsError = true;
                        singleResult.Message = "Date range is not in the correct format.";
                        serializedList = serializer.Serialize(singleResult);

                        var singleResultlist = new ContentResult()
                        {
                            Content = serializedList,
                            ContentType = "application/json",
                        };
                        return singleResultlist;
                    }

                    int option = Convert.ToInt32(sortOption);
                    PaymentLogic paymentLogic = new PaymentLogic();
                    List<PaymentSummary> paymentSummary = paymentLogic.GetPaymentSummary(dateFrom, dateTo);
                    if (paymentSummary != null && paymentSummary.Count > 0)
                    {
                        if (option == 1)
                        {
                            paymentSummary = paymentSummary.Where(p => p.PaymentEtranzactId != null && p.FeeTypeId == feeTypeId &&
                                                p.PaymentModeId == paymentModeId && p.ProgrammeId == programmeId && p.DepartmentId == departmentId).ToList();
                        }

                        for (int i = 0; i < paymentSummary.Count; i++)
                        {
                            PaymentReportDetail paymentJsonResult = new PaymentReportDetail();
                            paymentJsonResult.TransactionDate = Convert.ToDateTime(paymentSummary[i].TransactionDate).ToShortDateString();
                            paymentJsonResult.MatricNumber = paymentSummary[i].MatricNumber;
                            paymentJsonResult.Name = paymentSummary[i].Name;
                            paymentJsonResult.Level = paymentSummary[i].LevelName ?? "-";
                            paymentJsonResult.Faculty = paymentSummary[i].FacultyName;
                            paymentJsonResult.Session = paymentSummary[i].SessionName;
                            paymentJsonResult.InvoiceNumber = paymentSummary[i].InvoiceNumber;
                            paymentJsonResult.ConfirmationNumber = paymentSummary[i].ConfirmationNumber;
                            paymentJsonResult.FeeTypeName = paymentSummary[i].FeeTypeName;
                            paymentJsonResult.Amount = String.Format("{0:N}", paymentSummary[i].TransactionAmount);
                            paymentJsonResult.DepartmentId = departmentId;
                            paymentJsonResult.DepartmentName = paymentSummary[i].DepartmentName ?? "-";
                            paymentJsonResult.ProgrammeId = programmeId;
                            paymentJsonResult.ProgrammeName = paymentSummary[i].ProgrammeName ?? "-";
                            paymentJsonResult.PaymentModeId = paymentModeId;
                            paymentJsonResult.PaymentModeName = paymentSummary[i].PaymentModeName ?? "-";
                            paymentJsonResult.FeeTypeId = feeTypeId;

                            result.Add(paymentJsonResult);
                        }

                    }
                    serializedList = serializer.Serialize(result);
                    var serializedResult = new ContentResult
                    {
                        Content = serializedList,
                        ContentType = "application/json"
                    };
                    return serializedResult;
                }
                else
                {
                    singleResult.IsError = true;
                    singleResult.Message = "Invalid parametr! ";
                }
            }
            catch (Exception ex)
            {
                singleResult.IsError = true;
                singleResult.Message = "Error! " + ex.Message;
            }
            serializedList = serializer.Serialize(result);
            var resultlist = new ContentResult
            {
                Content = serializedList,
                ContentType = "application/json"
            };
            return resultlist;
        }

        
    }
}