using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.IO;
using System.Linq;
using System.Transactions;
using System.Web;
using System.Web.Mvc;
using Abundance_Nk.Business;
using Abundance_Nk.Model.Model;
using Abundance_Nk.Web.Areas.Admin.ViewModels;
using Abundance_Nk.Web.Controllers;
using Abundance_Nk.Web.Models;

namespace Abundance_Nk.Web.Areas.Admin.Controllers
{

    public class UploadGstScanResultController : BaseController
    {
        private const string VALUE = "Value";
        private const string TEXT = "Text";
        private const string ID = "Id";
        private const string NAME = "Name";
        GstScanLogic gstScanLogic = new GstScanLogic();
        // GET: Admin/UploadGstScanResult
        public ActionResult UploadGstScanResult()
        {
            try
            {

                ViewBag.SemesterId = Utility.PopulateSemesterSelectListItem();
                ViewBag.DepartmentId = Utility.PopulateAllDepartmentSelectListItem();
                ViewBag.SessionId = Utility.PopulateAllSessionSelectListItem();
            }
            catch (Exception)
            {

                throw;
            }
            return View();
        }

        [HttpPost]
        public ActionResult UploadGstScanResult(GstViewModel viewModel)
        {
            try
            {
                string pathForSaving = Server.MapPath("~/Content/ExcelUploads");
                string savedFileName = "";

                List<GstScan> gstScans = new List<GstScan>();
                gstScanLogic = new GstScanLogic();


                foreach (string file in Request.Files)
                {
                    HttpPostedFileBase hpf = Request.Files[file];
                    if (hpf.ContentLength == 0)
                    {
                        continue;
                    }

                    if (CreateFolderIfNeeded(pathForSaving))
                    {
                        FileInfo fileInfo = new FileInfo(hpf.FileName);
                        string fileExtension = fileInfo.Extension;
                        string newFile = "GSTUpload" + "__";
                        string newFileName = newFile + DateTime.Now.ToString().Replace("/", "").Replace(":", "").Replace(" ", "") + fileExtension;

                        savedFileName = Path.Combine(pathForSaving, newFileName);
                        hpf.SaveAs(savedFileName);
                        IExcelServiceManager excelServiceManager = new ExcelServiceManager();
                        DataSet dsGstScanResultList = excelServiceManager.UploadExcel(savedFileName);
                        //DataSet dsGstScanResultList = ReadExcelFile(savedFileName);
                        if (dsGstScanResultList != null && dsGstScanResultList.Tables[0].Rows.Count > 0)
                        {
                            int count = 0;
                            for (int i = 0; i < dsGstScanResultList.Tables[0].Rows.Count; i++)
                            {
                                if (!string.IsNullOrEmpty(dsGstScanResultList.Tables[0].Rows[i][0].ToString().Trim()))
                                {
                                    viewModel.GstScan = new GstScan();
                                    viewModel.GstScan.SN = Convert.ToInt32(dsGstScanResultList.Tables[0].Rows[i][0].ToString().Trim());
                                    viewModel.GstScan.CourseCode = Convert.ToString(dsGstScanResultList.Tables[0].Rows[i][1].ToString().Trim());
                                    viewModel.GstScan.CourseTitle = Convert.ToString(dsGstScanResultList.Tables[0].Rows[i][2].ToString().Trim());
                                    viewModel.GstScan.MatricNumber = Convert.ToString(dsGstScanResultList.Tables[0].Rows[i][3].ToString().Trim());
                                    viewModel.GstScan.Name = Convert.ToString(dsGstScanResultList.Tables[0].Rows[i][4].ToString().Trim());
                                    viewModel.GstScan.RawScore = Convert.ToDecimal(dsGstScanResultList.Tables[0].Rows[i][5].ToString().Trim());
                                    viewModel.GstScan.Ca = Convert.ToDecimal(dsGstScanResultList.Tables[0].Rows[i][6].ToString().Trim());

                                    if (viewModel.GstScan.Ca <= 0)
                                    {
                                        viewModel.GstScan.Ca = 0;
                                    }
                                    viewModel.GstScan.Total = Convert.ToDecimal(dsGstScanResultList.Tables[0].Rows[i][7].ToString().Trim());
                                    viewModel.GstScan.SemesterId = viewModel.Semester.Id;
                                    viewModel.GstScan.SessionId = viewModel.CurrentSession.Id;

                                    if (viewModel.GstScan.MatricNumber != null && viewModel.Semester.Id > 0)
                                    {

                                        SemesterLogic semesterLogic = new SemesterLogic();
                                        DepartmentLogic departmentLogic = new DepartmentLogic();
                                        var semester = semesterLogic.GetEntityBy(s => s.Semester_Id == viewModel.Semester.Id).Semester_Name.ToUpper();
                                        var department = departmentLogic.GetEntityBy(d => d.Department_Id == viewModel.Department.Id).Department_Name.Trim().ToUpper();

                                        var listAdd = new GstScan()
                                        {
                                            SN = viewModel.GstScan.SN,
                                            CourseCode = viewModel.GstScan.CourseCode,
                                            CourseTitle = viewModel.GstScan.CourseTitle,
                                            MatricNumber = viewModel.GstScan.MatricNumber,
                                            Name = viewModel.GstScan.Name,
                                            DepartmentName = department.ToUpper(),
                                            RawScore = viewModel.GstScan.RawScore,
                                            Ca = viewModel.GstScan.Ca,
                                            Total = viewModel.GstScan.Total,
                                            SemesterName = semester.ToUpper(),
                                            SessionId = viewModel.GstScan.SessionId,
                                            SemesterId = viewModel.GstScan.SemesterId


                                        };
                                        gstScans.Add(listAdd);
                                    }
                                    else
                                    {
                                        SetMessage("Error Occured Some Values are Missing From the Excel", Message.Category.Error);
                                        break;
                                    }
                                }
                            
                            }
                            //if (count > 0)
                            //{

                            //    SetMessage(count + " " + "Result could not be added because they alreay exist", Message.Category.Error);
                            //}

                            viewModel.GstScanList = gstScans;
                            TempData["viewModel"] = viewModel;

                        }
                    }
                }
                KeepDropDownState(viewModel);
            }
            catch (Exception ex)
            {
                SetMessage("Error occured." + ex.Message, Message.Category.Error);
            }
            return View(viewModel);
        }
        public void KeepDropDownState(GstViewModel keepDropdownState)
        {
            try
            {

                keepDropdownState.DepartmentSelectListItem = Utility.PopulateAllDepartmentSelectListItem();
                keepDropdownState.SemesterSelectListItem = Utility.PopulateSemesterSelectListItem();
                keepDropdownState.SessionSelectListItem = Utility.PopulateAllSessionSelectListItem();



                if (keepDropdownState.Department != null && keepDropdownState.Department.Id > 0)
                {
                    ViewBag.DepartmentId = new SelectList(keepDropdownState.DepartmentSelectListItem, VALUE, TEXT, keepDropdownState.Department.Id);
                }
                else
                {
                    ViewBag.DepartmentId = new SelectList(keepDropdownState.DepartmentSelectListItem, VALUE, TEXT);
                }
                if (keepDropdownState.Semester != null && keepDropdownState.Semester.Id > 0)
                {
                    ViewBag.SemesterId = new SelectList(keepDropdownState.SemesterSelectListItem, VALUE, TEXT, keepDropdownState.Semester.Id);
                }
                else
                {
                    ViewBag.SemesterId = new SelectList(keepDropdownState.SemesterSelectListItem, VALUE, TEXT);
                }
                if (keepDropdownState.CurrentSession != null && keepDropdownState.CurrentSession.Id > 0)
                {
                    ViewBag.SessionId = new SelectList(keepDropdownState.SessionSelectListItem, VALUE, TEXT, keepDropdownState.CurrentSession.Id);
                }
                else
                {
                    ViewBag.SessionId = new SelectList(keepDropdownState.SessionSelectListItem, VALUE, TEXT);
                }
                if (keepDropdownState.Course != null && keepDropdownState.Course.Id > 0)
                {
                    ViewBag.CourseCode = new SelectList(Utility.PopulateAllCourseCodeSelectListItem(), VALUE, TEXT, keepDropdownState.Course.Id);
                }
                else
                {
                    ViewBag.CourseCode = new SelectList(keepDropdownState.SemesterSelectListItem, VALUE, TEXT);
                }

            }
            catch (Exception)
            {

                SetMessage("Error Occured", Message.Category.Error);
            }
        }
        private bool CreateFolderIfNeeded(string path)
        {
            try
            {
                bool result = true;
                if (!Directory.Exists(path))
                {
                    try
                    {
                        Directory.CreateDirectory(path);
                    }
                    catch (Exception)
                    {

                        result = false;
                    }
                }

                return result;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public static DataSet ReadExcelFile(string filepath)
        {
            DataSet result = null;
            try
            {
                string xConnStr = "Provider=Microsoft.Jet.OLEDB.4.0;" + "Data Source=" + filepath + ";" + "Extended Properties=Excel 8.0;";
                OleDbConnection connection = new OleDbConnection(xConnStr);
                OleDbCommand command = new OleDbCommand("Select * FROM [Sheet1$]", connection);
                connection.Open();
                // Create DbDataReader to Data Worksheet

                OleDbDataAdapter MyData = new OleDbDataAdapter();
                MyData.SelectCommand = command;
                DataSet ds = new DataSet();
                ds.Clear();
                MyData.Fill(ds);
                connection.Close();

                result = ds;

            }
            catch (OleDbException ex)
            {
                throw ex;
            }
            return result;
        }
        [HttpPost]
        public ActionResult SaveGstScanResult(GstViewModel viewModel)
        {
           
                int UploadCount = 0;

                GstScanResultAuditLogic gstScanResultAuditLogic = new GstScanResultAuditLogic();
                gstScanLogic = new GstScanLogic();

                viewModel = (GstViewModel)TempData["viewModel"];
                if (viewModel.GstScanList != null && viewModel.GstScanList.Count > 0)
                {
                    using (TransactionScope transactionScope = new TransactionScope(TransactionScopeOption.Required,
                        new System.TimeSpan(0, 15, 0)))
                    {
                        try
                        {
                            if (viewModel.GstScanList != null)
                            {
                                for (int i = 0; i < viewModel.GstScanList.Count; i++)
                                {
                                    GstScan gstScan = new GstScan
                                    {
                                        CourseCode = viewModel.GstScanList[i].CourseCode,
                                        CourseTitle = viewModel.GstScanList[i].CourseTitle,
                                        MatricNumber = viewModel.GstScanList[i].MatricNumber,
                                        Name = viewModel.GstScanList[i].Name,
                                        DepartmentName = viewModel.GstScanList[i].DepartmentName.ToUpper(),
                                        RawScore = viewModel.GstScanList[i].RawScore,
                                        Ca = viewModel.GstScanList[i].Ca,
                                        Total = viewModel.GstScanList[i].Total,
                                        SemesterName = viewModel.GstScanList[i].SemesterName.ToUpper(),
                                        SemesterId = viewModel.GstScanList[i].SemesterId,
                                        SessionId = viewModel.GstScanList[i].SessionId
                                    };
                                    GstScan existingScan = gstScanLogic.GetBy(gstScan);
                                    if (existingScan != null && existingScan.Id > 0)
                                    {
                                        gstScan.Id = existingScan.Id;
                                        gstScanLogic.Modify(gstScan);
                                        UploadCount++;
                                    }
                                    else
                                    {
                                        GstScan createdGstScanResult = gstScanLogic.Create(gstScan);
                                        if (createdGstScanResult != null)
                                        {
                                            CreateAudit(createdGstScanResult);

                                        }

                                        UploadCount++;
                                    }
                                   
                                }
                            }

                            transactionScope.Complete();
                        }
                        catch (Exception ex)
                        {
                            transactionScope.Dispose();
                            SetMessage("Error Occured" + ex.Message, Message.Category.Error);
                            return RedirectToAction("UploadGstScanResult");
                        }
                    }
                    SetMessage(UploadCount + " " + "Out of" + " " + viewModel.GstScanList.Count + " " + "Uploaded Successfully", Message.Category.Information);
                    return RedirectToAction("UploadGstScanResult");
                }
            
           
            return HttpNotFound("No resourse found");
        }

        public ActionResult ViewUpload()
        {
            GstViewModel viewModel = new GstViewModel();
            try
            {
                viewModel = (GstViewModel)TempData["ScanList"];

                if (viewModel != null)
                {
                    List<GstScan> gstScan = gstScanLogic.GetModelsBy(s => s.DEPARTMENT.Trim().Contains(viewModel.GstScan.DepartmentName) && s.COURSE_CODE.Trim().Contains(viewModel.GstScan.CourseCode) && s.SESSION_NAME.Trim().Contains(viewModel.GstScan.SessionName));
                    viewModel.GstScanList = gstScan;

                    KeepDropDownState(viewModel);
                    return View(viewModel);
                }

                ViewBag.SessionId = Utility.PopulateAllSessionSelectListItem();
                ViewBag.SemesterId = Utility.PopulateSemesterSelectListItem();
                ViewBag.DepartmentId = Utility.PopulateAllDepartmentSelectListItem();
                ViewBag.CourseCode = new SelectList(new List<Course>(), ID, NAME);


            }
            catch (Exception ex)
            {
                SetMessage(ex.Message, Message.Category.Error);
            }
            return View();
        }

        [HttpPost]
        public ActionResult ViewUpload(GstViewModel viewModel)
        {

            try
            {
                gstScanLogic = new GstScanLogic();
                DepartmentLogic departmentLogic = new DepartmentLogic();
                CourseLogic courseLogic = new CourseLogic();

                if (viewModel.Department.Id <= 0 && viewModel.Course.Id <= 0 && viewModel.Semester.Id <= 0 && viewModel.CurrentSession.Id <= 0)
                {
                    SetMessage("Error Occurred one or more select options not filled", Message.Category.Error);
                    return View();
                }
                string department = departmentLogic.GetModelBy(d => d.Department_Id == viewModel.Department.Id).Name.Trim().ToUpper();
                string course = courseLogic.GetModelBy(c => c.Course_Id == viewModel.Course.Id).Code.Trim().ToUpper();
                string sessionName = (viewModel.Semester.Id == 1 ? "FIRST SEMESTER" : "SECOND SEMESTER").Trim().ToUpper();

                List<GstScan> gstScan = gstScanLogic.GetModelsBy(s => s.DEPARTMENT.Trim().Contains(department) && s.COURSE_CODE.Trim().Contains(course) && s.SESSION_NAME.Trim().Contains(sessionName) && s.Session_Id == viewModel.CurrentSession.Id && s.Semester_Id == viewModel.Semester.Id);
                viewModel.GstScanList = gstScan;

                viewModel.GstScan = new GstScan
                {
                    DepartmentName = department,
                    CourseCode = course,
                    SessionName = sessionName
                };

                TempData["ScanList"] = viewModel;
            }
            catch (Exception)
            {

                throw;
            }
            KeepDropDownState(viewModel);
            return View(viewModel);
        }
        public JsonResult GetCourseCodeByDepartment(string id, string semesterId)
        {
            try
            {
                if (string.IsNullOrEmpty(id) && string.IsNullOrEmpty(semesterId))
                {
                    return null;
                }

                var department = new Department() { Id = Convert.ToInt32(id) };
                var semester = new Semester() { Id = Convert.ToInt32(semesterId) };
                var programme = new Programme() { Id = 1 };
                var courseLogic = new CourseLogic();
                List<Course> courses = courseLogic.GetModelsBy(c => c.Department_Id == department.Id && c.Semester_Id == semester.Id && c.Programme_Id == programme.Id).Where(c => c.Code.Contains("GST")).Distinct().ToList();

                for (int i = 0; i < courses.Count; i++)
                {
                    courses[i].Name = courses[i].Code;
                }

                return Json(new SelectList(courses, ID, NAME), JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public ActionResult EditGstScanResult(string resultId)
        {
            GstScan gstScan = new GstScan();
            try
            {
                long id = Convert.ToInt64(resultId);
                if (string.IsNullOrEmpty(resultId))
                {
                    return RedirectToAction("ViewUpload");
                }
                gstScanLogic = new GstScanLogic();
                gstScan = gstScanLogic.GetModelBy(gs => gs.ID == id);


            }
            catch (Exception)
            {

                throw;
            }

            return View(gstScan);

        }

        public ActionResult SaveEditedGstResult(GstScan Scan)
        {
            GstScan gstScan = Scan;
            try
            {
                gstScanLogic = new GstScanLogic();
                var gstModified = gstScanLogic.Modify(gstScan);

                if (gstModified != null)
                {

                    CreateAudit(gstModified);
                    return RedirectToAction("ViewUpload");
                }
            }
            catch (Exception)
            {

                throw;
            }
            return RedirectToAction("EditGstScanResult", gstScan.Id);
        }

        public GstScanResultAudit CreateAudit(GstScan gstScanResultAdded)
        {
            GstScanResultAudit gstScanAudit = new GstScanResultAudit();
            try
            {

                GstScanResultAuditLogic gstScanResultAuditLogic = new GstScanResultAuditLogic();
                var addedScanResult = gstScanResultAdded;
                string client = Request.LogonUserIdentity.Name + " (" + HttpContext.Request.UserHostAddress + ")";
                gstScanAudit = gstScanResultAuditLogic.GetModelBy(gs => gs.Gst_Scan_Result_Id == addedScanResult.Id);
                if (gstScanAudit != null)
                {
                    if (client == gstScanAudit.Client)
                    {
                        gstScanAudit.Operation = "MODIFY";
                        gstScanAudit.Action = "MODIFIED RESULT ";
                        gstScanAudit.FullName = addedScanResult.Name;
                        gstScanAudit.ExamNo = addedScanResult.MatricNumber;
                        gstScanAudit.RawScore = (decimal)addedScanResult.RawScore;
                        gstScanAudit.CA = (decimal)addedScanResult.Ca;

                        var modified = gstScanResultAuditLogic.Create(gstScanAudit);
                        if (modified != null)
                        {
                            SetMessage("operation successful", Message.Category.Information);
                        }
                    }
                }
                else
                {

                    if (addedScanResult.DepartmentName == null && addedScanResult.DepartmentName == null)
                    {
                        GstScanLogic scanlogic = new GstScanLogic();
                        var gstScan = scanlogic.GetModelBy(s => s.ID == addedScanResult.Id);

                        addedScanResult.CourseCode = gstScan.CourseCode;
                        addedScanResult.CourseTitle = gstScan.CourseTitle;
                        addedScanResult.DepartmentName = gstScan.DepartmentName;
                        addedScanResult.SemesterName = gstScan.SemesterName;
                    }
                    string action = "UPLOADING GST RESULT";
                    UserLogic userLogic = new UserLogic();
                    var user = userLogic.GetModelsBy(u => u.User_Name == User.Identity.Name).LastOrDefault();
                    gstScanAudit = new GstScanResultAudit
                    {
                        Operation = "INSERT",
                        Action = action,
                        Client = client,
                        DateUploaded = DateTime.Now,
                        User = user

                    };
                    gstScanAudit.GstScan = addedScanResult;
                    gstScanAudit.CourseCode = addedScanResult.CourseCode;
                    gstScanAudit.CourseTitle = addedScanResult.CourseTitle;
                    gstScanAudit.ExamNo = addedScanResult.MatricNumber;
                    gstScanAudit.FullName = addedScanResult.Name;
                    gstScanAudit.DepartmentName = addedScanResult.DepartmentName;
                    gstScanAudit.RawScore = (decimal)addedScanResult.RawScore;
                    gstScanAudit.CA = (decimal)addedScanResult.Ca;
                    gstScanAudit.Total = (decimal)addedScanResult.Total;
                    gstScanAudit.SemesterName = addedScanResult.SemesterName.ToUpper();
                    gstScanAudit.SemesterId = addedScanResult.SemesterId;
                    gstScanAudit.SessionId = addedScanResult.SessionId;

                    var modified = gstScanResultAuditLogic.Create(gstScanAudit);

                    if (modified != null)
                    {
                        SetMessage("Operation Successful", Message.Category.Information);
                    }
                }

            }
            catch (Exception)
            {

                throw;
            }
            return gstScanAudit;
        }
    }
}