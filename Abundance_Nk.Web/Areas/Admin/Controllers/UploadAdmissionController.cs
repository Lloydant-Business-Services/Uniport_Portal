using Abundance_Nk.Business;
using Abundance_Nk.Model.Entity;
using Abundance_Nk.Model.Model;
using Abundance_Nk.Web.Areas.Admin.ViewModels;
using Abundance_Nk.Web.Controllers;
using Abundance_Nk.Web.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.IO;
using System.Linq;
using System.Transactions;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;
using System.Web.UI.WebControls;
using Ionic.Zip;

namespace Abundance_Nk.Web.Areas.Admin.Views
{
    [RoleBasedAttribute]
    public class UploadAdmissionController :BaseController
    {
        private const string ID = "Id";
        private const string NAME = "Name";
        private const string VALUE = "Value";
        private const string TEXT = "Text";
        private Abundance_NkEntities db = new Abundance_NkEntities();
        private UploadAdmissionViewModel viewmodel;

        //
        // GET: /Admin/UploadAdmission/
        public ActionResult UploadAdmission()
        {
            try
            {
                viewmodel = new UploadAdmissionViewModel();
                viewmodel.ProgrammeSelectListItem = Utility.PopulateAllProgrammeSelectListItem();
                viewmodel.SessionSelectListItem = Utility.PopulateSessionSelectListItem();
                viewmodel.AdmissionListTypeSelectListItem = Utility.PopulateAdmissionListTypeSelectListItem();
                ViewBag.ProgrammeId = viewmodel.ProgrammeSelectListItem;
                ViewBag.SessionId = viewmodel.SessionSelectListItem;
                ViewBag.AdmissionListTypeId = viewmodel.AdmissionListTypeSelectListItem;
                ViewBag.DepartmentId = new SelectList(new List<Department>(),ID,NAME);
                ViewBag.DepartmentOptionId = new SelectList(new List<DepartmentOption>(),ID,NAME);
            }
            catch(Exception ex)
            {
                SetMessage("Error Occurred! " + ex.Message,Message.Category.Error);
            }
            return View();
        }

        public JsonResult GetDepartmentByProgrammeId(string id)
        {
            try
            {
                if(string.IsNullOrEmpty(id))
                {
                    return null;
                }

                var programme = new Programme { Id = Convert.ToInt32(id) };

                UserLogic userLogic = new UserLogic();
                var user = userLogic.GetModelBy(s => s.User_Name == User.Identity.Name);
                FacultyOfficerLogic facultyOfficerLogic = new FacultyOfficerLogic();
                DepartmentLogic departmentLogic = new DepartmentLogic();
                List<Department> departments = new List<Department>();
                if (user.Role.Id == (int)Roles.FacultyOfficer)
                {
                    var getUserFaculty = facultyOfficerLogic.GetModelsBy(u => u.User_Id == user.Id).FirstOrDefault();
                    departments = departmentLogic.GetBy(getUserFaculty.Faculty, programme);

                }
                else
                {
                    departments = departmentLogic.GetBy(programme);

                }

          

                return Json(new SelectList(departments,ID,NAME),JsonRequestBehavior.AllowGet);
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }

        [HttpPost]
        public ActionResult UploadAdmission(UploadAdmissionViewModel vmodel)
        {
            try
            {
                KeepApplicationFormInvoiceGenerationDropDownState(vmodel);

                var applicants = new List<AppliedCourse>();
                string pathForSaving = Server.MapPath("~/Content/ExcelUploads");
                string savedFileName = "";
                foreach(string file in Request.Files)
                {
                    HttpPostedFileBase hpf = Request.Files[file];
                    if(hpf.ContentLength == 0)
                        continue;
                    if(CreateFolderIfNeeded(pathForSaving))
                    {
                        var fileInfo = new FileInfo(hpf.FileName);
                        string fileExtension = fileInfo.Extension;
                        string newFile = "Admission" + "__";
                        string newFileName = newFile +
                                             DateTime.Now.ToString().Replace("/","").Replace(":","").Replace(" ","") +
                                             fileExtension;

                        savedFileName = Path.Combine(pathForSaving,newFileName);
                        hpf.SaveAs(savedFileName);
                    }
                    DataSet dsAdmissionList = ReadExcelFile(savedFileName);
                    if(dsAdmissionList != null && dsAdmissionList.Tables[0].Rows.Count > 0)
                    {
                        string Application_Number = "";

                        var appliedCourseLogic = new AppliedCourseLogic();
                        var applicantJambDetailLogic = new ApplicantJambDetailLogic();

                        for(int i = 0;i < dsAdmissionList.Tables[0].Rows.Count;i++)
                        {
                            Application_Number = dsAdmissionList.Tables[0].Rows[i][0].ToString();
                            ApplicantJambDetail applicantJambDetail = applicantJambDetailLogic.GetModelsBy(a => a.Applicant_Jamb_Registration_Number == Application_Number && a.APPLICATION_FORM != null).LastOrDefault();
                            if(applicantJambDetail != null && applicantJambDetail.ApplicationForm.Id > 0)
                            {
                                var appliedCourse = new AppliedCourse();
                                appliedCourse = appliedCourseLogic.GetModelBy( m => m.APPLICATION_FORM.Application_Form_Id == applicantJambDetail.ApplicationForm.Id);
                                if(appliedCourse != null)
                                {
                                    applicants.Add(appliedCourse);
                                }
                            }
                            else
                            {
                                var appliedCourse = new AppliedCourse();
                                appliedCourse = appliedCourseLogic.GetModelBy(m => m.APPLICATION_FORM.Application_Form_Number == Application_Number);
                                if(appliedCourse != null)
                                {
                                    applicants.Add(appliedCourse);
                                }
                            }
                        }

                        vmodel.AppliedCourseList = applicants;
                        TempData["UploadAdmissionViewModel"] = vmodel;
                        return View(vmodel);
                    }
                }
            }
            catch(Exception)
            {
                throw;
            }
            return View();
        }

        [HttpPost]
        public ActionResult SaveAdmissionList(UploadAdmissionViewModel vmodel)
        {
            try
            {
                string operation = "INSERT";
                string action = "UPLOADING OF ADMISSION LIST";
                string client = Request.LogonUserIdentity.Name + " (" + HttpContext.Request.UserHostAddress + ")";

                vmodel = (UploadAdmissionViewModel)TempData["UploadAdmissionViewModel"];
                if(vmodel.AppliedCourseList != null && vmodel.AppliedCourseList.Count > 0)
                {
                    using(var transaction = new TransactionScope())
                    {
                        var batch = new AdmissionListBatch();
                        var batchLogic = new AdmissionListBatchLogic();
                        var AdmissionType = new AdmissionListType();
                        var AdmissionListAudit = new AdmissionListAudit();
                        var loggeduser = new UserLogic();
                        AdmissionType = vmodel.AdmissionListType;
                        batch.DateUploaded = DateTime.Now;
                        batch.IUploadedFilePath = "NAN";
                        batch.Type = AdmissionType;
                        batch = batchLogic.Create(batch);

                        AdmissionListAudit.Action = action;
                        AdmissionListAudit.Client = client;
                        AdmissionListAudit.Operation = operation;
                        AdmissionListAudit.Time = DateTime.Now;
                        AdmissionListAudit.User = loggeduser.GetModelBy(u => u.User_Name == User.Identity.Name);
                        vmodel.ExistingAdmissions = new List<AdmissionList>();
                        ApplicantJambDetailLogic applicantJambDetailLogic = new ApplicantJambDetailLogic();

                        for (int i = 0;i < vmodel.AppliedCourseList.Count;i++)
                        {
                            var admissionlist = new AdmissionList();
                            var admissionListLogic = new AdmissionListLogic();
                            admissionlist.Form = vmodel.AppliedCourseList[i].ApplicationForm;
                            admissionlist.Deprtment = vmodel.AdmissionListDetail.Deprtment;
                            admissionlist.Programme = vmodel.AdmissionListDetail.Form.ProgrammeFee.Programme;
                            if(vmodel.AdmissionListDetail.DepartmentOption != null &&
                                vmodel.AdmissionListDetail.DepartmentOption.Id > 0)
                            {
                                admissionlist.DepartmentOption = vmodel.AdmissionListDetail.DepartmentOption;
                            }

                            admissionlist.Activated = true;
                            if(!admissionListLogic.IsAdmitted(admissionlist.Form))
                            {
                                admissionListLogic.Create(admissionlist,batch,AdmissionListAudit);
                            }
                            else
                            {
                                var jambDetail = applicantJambDetailLogic.GetBy(admissionlist.Form);
                                var admissionDetail = admissionListLogic.GetBy(admissionlist.Form.Id);
                                admissionDetail.ApplicantJambDetail = new ApplicantJambDetail();
                                admissionDetail.ApplicantJambDetail = jambDetail;
                                vmodel.ExistingAdmissions.Add(admissionDetail);
                            }
                        }
                        transaction.Complete();
                    }
                    if(vmodel.ExistingAdmissions != null  && vmodel.ExistingAdmissions.Count > 0)
                    {
                        TempData["DuplicateModel"] = vmodel;
                        return RedirectToAction("ViewDuplicates");
                    }

                    SetMessage("List was uploaded successfully",Message.Category.Information);
                    return RedirectToAction("UploadAdmission");
                }
            }
            catch(Exception ex)
            {
                throw;
            }
            SetMessage("List was not uploaded successfully, Please check for duplicates and try again",
                Message.Category.Information);

            return View();
        }

        public ActionResult ViewDuplicates()
        {
            try
            {
                UploadAdmissionViewModel vmodel = new UploadAdmissionViewModel();
                if (TempData["DuplicateModel"] != null)
                {
                    vmodel = (UploadAdmissionViewModel) TempData["DuplicateModel"];
                    if (vmodel != null && vmodel.ExistingAdmissions != null)
                    {
                        return View(vmodel);
                    }
                }
                
            }
            catch (Exception ex)
            {
                SetMessage("List was not uploaded successfully", Message.Category.Information);
                
            }
            return RedirectToAction("UploadAdmission");
        }
        public ActionResult ViewAdmission()
        {
            try
            {
                viewmodel = new UploadAdmissionViewModel();
                viewmodel.ProgrammeSelectListItem = Utility.PopulateAllProgrammeSelectListItem();
                viewmodel.SessionSelectListItem = Utility.PopulateSessionSelectListItem();
                viewmodel.AdmissionListTypeSelectListItem = Utility.PopulateAdmissionListTypeSelectListItem();
                ViewBag.ProgrammeId = viewmodel.ProgrammeSelectListItem;
                ViewBag.SessionId = viewmodel.SessionSelectListItem;
                ViewBag.AdmissionListTypeId = viewmodel.AdmissionListTypeSelectListItem;
                ViewBag.DepartmentId = new SelectList(new List<Department>(),ID,NAME);
                ViewBag.DepartmentOptionId = new SelectList(new List<DepartmentOption>(),ID,NAME);
            }
            catch(Exception ex)
            {
                SetMessage("Error Occurred! " + ex.Message,Message.Category.Error);
            }
            return View();
        }

        [HttpPost]
        public ActionResult ViewAdmission(UploadAdmissionViewModel vmodel)
        {
            try
            {
                KeepApplicationFormInvoiceGenerationDropDownState(vmodel);
                if(vmodel.AdmissionListDetail.Deprtment != null &&
                    vmodel.AdmissionListDetail.Form.ProgrammeFee.Programme != null && vmodel.CurrentSession != null &&
                    vmodel.AdmissionListType != null)
                {
                    var list = new List<AdmissionList>();
                    var ListLogic = new AdmissionListLogic();
                    list = ListLogic.GetByAdmissionLists(vmodel.AdmissionListDetail.Deprtment,
                        vmodel.AdmissionListDetail.Form.ProgrammeFee.Programme,vmodel.CurrentSession,
                        vmodel.AdmissionListType);
                    if(list != null)
                    {
                        vmodel.AdmissionList = list;
                        return View(vmodel);
                    }
                }
            }
            catch(Exception ex)
            {
                SetMessage("Error Occurred! " + ex.Message,Message.Category.Error);
            }
            return View();
        }

        private string InvalidFile(decimal uploadedFileSize,string fileExtension)
        {
            try
            {
                string message = null;
                decimal oneKiloByte = 1024;
                decimal maximumFileSize = 50 * oneKiloByte;

                decimal actualFileSizeToUpload = Math.Round(uploadedFileSize / oneKiloByte,1);
                if(InvalidFileType(fileExtension))
                {
                    message = "File type '" + fileExtension +
                              "' is invalid! File type must be any of the following: .jpg, .jpeg, .png or .jif ";
                }
                else if(actualFileSizeToUpload > (maximumFileSize / oneKiloByte))
                {
                    message = "Your file size of " + actualFileSizeToUpload.ToString("0.#") +
                              " Kb is too large, maximum allowed size is " + (maximumFileSize / oneKiloByte) + " Kb";
                }

                return message;
            }
            catch(Exception)
            {
                throw;
            }
        }

        private bool InvalidFileType(string extension)
        {
            extension = extension.ToLower();
            switch(extension)
            {
                case ".xls":
                return false;

                case ".xlsx":
                return false;

                default:
                return true;
            }
        }

        private void DeleteFileIfExist(string folderPath,string fileName)
        {
            try
            {
                string wildCard = fileName + "*.*";
                IEnumerable<string> files = Directory.EnumerateFiles(folderPath,wildCard,SearchOption.TopDirectoryOnly);

                if(files != null && files.Count() > 0)
                {
                    foreach(string file in files)
                    {
                        System.IO.File.Delete(file);
                    }
                }
            }
            catch(Exception)
            {
                throw;
            }
        }

        private bool CreateFolderIfNeeded(string path)
        {
            try
            {
                bool result = true;
                if(!Directory.Exists(path))
                {
                    try
                    {
                        Directory.CreateDirectory(path);
                    }
                    catch(Exception)
                    {
                        /*TODO: You must process this exception.*/
                        result = false;
                    }
                }

                return result;
            }
            catch(Exception)
            {
                throw;
            }
        }

        public static DataSet ReadExcelFile(string filepath)
        {
            DataSet Result = null;
            try
            {
                //string xConnStr = "Provider=Microsoft.Jet.OLEDB.4.0;" + "Data Source=" + filepath + ";" + "Extended Properties=Excel 8.0;";
                string xConnStr = "Provider=Microsoft.ACE.OLEDB.12.0;" + "Data Source=" + filepath + ";" + "Extended Properties=Excel 8.0;";

                var connection = new OleDbConnection(xConnStr);
                var command = new OleDbCommand("Select * FROM [Sheet1$]",connection);
                connection.Open();
                // Create DbDataReader to Data Worksheet

                var MyData = new OleDbDataAdapter();
                MyData.SelectCommand = command;
                var ds = new DataSet();
                ds.Clear();
                MyData.Fill(ds);
                connection.Close();

                Result = ds;
            }
            catch(Exception ex)
            {
                throw ex;
            }
            return Result;
        }
        private DataSet ReadExcel(string filepath)
        {
            DataSet Result = null;
            try
            {
                string xConnStr = "Provider=Microsoft.Jet.OLEDB.4.0;" + "Data Source=" + filepath + ";" +"Extended Properties=Excel 8.0;";
                var connection = new OleDbConnection(xConnStr);
                connection.Open();
                DataTable sheet = connection.GetOleDbSchemaTable(OleDbSchemaGuid.Tables,null);
                foreach(DataRow dataRow in sheet.Rows)
                {
                    string sheetName = dataRow[2].ToString().Replace("'","");
                    var command = new OleDbCommand("Select * FROM [" + sheetName + "]",connection);
                    // Create DbDataReader to Data Worksheet

                    var MyData = new OleDbDataAdapter();
                    MyData.SelectCommand = command;
                    var ds = new DataSet();
                    ds.Clear();
                    MyData.Fill(ds);
                    connection.Close();

                    Result = ds;
                }
            }
            catch(Exception ex)
            {
                throw ex;
            }
            return Result;
        }
        private void KeepApplicationFormInvoiceGenerationDropDownState(UploadAdmissionViewModel viewModel)
        {
            try
            {
                if(viewModel.AdmissionListDetail.Form.ProgrammeFee.Programme.Id != null &&
                    viewModel.AdmissionListDetail.Form.ProgrammeFee.Programme.Id > 0)
                {
                    viewModel.DepartmentSelectListItem = Utility.PopulateDepartmentSelectListItem(viewModel.AdmissionListDetail.Form.ProgrammeFee.Programme);
                    viewModel.DepartmentOpionSelectListItem = Utility.PopulateDepartmentOptionSelectListItem(viewModel.AdmissionListDetail.Deprtment,viewModel.AdmissionListDetail.Form.ProgrammeFee.Programme);
                    ViewBag.ProgrammeId = new SelectList(viewModel.ProgrammeSelectListItem,VALUE,TEXT,viewModel.AdmissionListDetail.Form.ProgrammeFee.Programme.Id);
                    ViewBag.SessionId = Utility.PopulateSessionSelectListItem();
                    ViewBag.AdmissionListTypeId = Utility.PopulateAdmissionListTypeSelectListItem();

                    if(viewModel.AdmissionListDetail.Deprtment != null &&
                        viewModel.AdmissionListDetail.Deprtment.Id > 0)
                    {
                        ViewBag.DepartmentId = new SelectList(viewModel.DepartmentSelectListItem,VALUE,TEXT,
                            viewModel.AdmissionListDetail.Deprtment.Id);
                        ViewBag.DepartmentOptionId = new SelectList(viewModel.DepartmentOpionSelectListItem,VALUE,TEXT);
                    }
                    else
                    {
                        ViewBag.DepartmentId = new SelectList(viewModel.DepartmentSelectListItem,VALUE,TEXT);
                        ViewBag.DepartmentOptionId = new SelectList(viewModel.DepartmentOpionSelectListItem,VALUE,TEXT);
                    }
                }
                else
                {
                    ViewBag.ProgrammeId = new SelectList(viewModel.ProgrammeSelectListItem,VALUE,TEXT);
                    ViewBag.DepartmentId = new SelectList(new List<Department>(),ID,NAME);
                    ViewBag.DepartmentOptionId = new SelectList(new List<DepartmentOption>(),ID,NAME);
                    ViewBag.SessionId = new SelectList(viewmodel.SessionSelectListItem,VALUE,TEXT);
                    ViewBag.AdmissionListTypeId = viewmodel.AdmissionListTypeSelectListItem;
                }
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }

        public JsonResult GetDepartmentOptionByDepartment(string id,string programmeid)
        {
            try
            {
                if(string.IsNullOrEmpty(id))
                {
                    return null;
                }

                var department = new Department { Id = Convert.ToInt32(id) };
                var programme = new Programme { Id = Convert.ToInt32(programmeid) };
                var departmentLogic = new DepartmentOptionLogic();
                List<DepartmentOption> departmentOptions = departmentLogic.GetBy(department,programme);

                return Json(new SelectList(departmentOptions,ID,NAME),JsonRequestBehavior.AllowGet);
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }

        public ActionResult SearchAdmittedStudents()
        {
            return View();
        }

        [HttpPost]
        public ActionResult SearchAdmittedStudents(UploadAdmissionViewModel vModel)
        {
            try
            {
                var admissionList = new List<AdmissionList>();
                var admissionListLogic = new AdmissionListLogic();
                var applicantJambDetailLogic = new ApplicantJambDetailLogic();
                if(vModel.SearchString != null)
                {
                    string search = vModel.SearchString;
                    admissionList = admissionListLogic.GetModelsBy(p => p.APPLICATION_FORM.Application_Exam_Number.Contains(search) || p.APPLICATION_FORM.Application_Form_Number.Contains(search));
                    if(admissionList.Count > 0)
                    {
                        vModel.AdmissionList = admissionList;
                    }
                    else
                    {
                        vModel.AdmissionList = new List<AdmissionList>();
                        List<ApplicantJambDetail> applicantJambDetails = applicantJambDetailLogic.GetModelsBy(a => a.Applicant_Jamb_Registration_Number.Contains(search) && a.APPLICATION_FORM != null);
                        foreach (ApplicantJambDetail applicantJambDetail in applicantJambDetails)
                        {
                            if(applicantJambDetail != null && applicantJambDetail.ApplicationForm != null)
                            {
                                if (admissionListLogic.IsAdmitted(applicantJambDetail.ApplicationForm))
                                {
                                    AdmissionList studeAdmissionList =  admissionListLogic.GetModelBy(f => f.Application_Form_Id == applicantJambDetail.ApplicationForm.Id);
                                    vModel.AdmissionList.Add(studeAdmissionList);
                                }
                   
                            }
                        }


                    }
                }

                return View(vModel);
            }
            catch(Exception)
            {
                throw;
            }
        }

        public ActionResult EditAdmittedStudentDepartment(long id)
        {
            try
            {
                var admissionList = new AdmissionList();
                var admissionListLogic = new AdmissionListLogic();
                admissionList = admissionListLogic.GetModelBy(p => p.Admission_List_Id == id);
                if(admissionList == null)
                {
                    TempData["Action"] = "Student is does not have admission";
                    return RedirectToAction("SearchAdmittedStudents");
                }
                var vModel = new UploadAdmissionViewModel();
                vModel.AdmissionListDetail = admissionList;
                KeepApplicationFormInvoiceGenerationDropDownState(vModel);
                return View(vModel);
            }
            catch(Exception)
            {
                throw;
            }
        }

        [HttpPost]
        public ActionResult EditAdmittedStudentDepartment(UploadAdmissionViewModel vModel)
        {
            try
            {
                if(vModel.AdmissionListDetail.Id > 0)
                {
                    var admissionList = new AdmissionList();
                    var admissionListLogic = new AdmissionListLogic();
                    admissionList = admissionListLogic.GetBy(vModel.AdmissionListDetail.Form.Id);
                    if(admissionList != null)
                    {
                        admissionList.Deprtment.Id = vModel.AdmissionListDetail.Deprtment.Id;
                        admissionList.Programme.Id = vModel.AdmissionListDetail.Programme.Id;
                        admissionList.Activated = vModel.AdmissionListDetail.Activated;
                        if (vModel.AdmissionListDetail.DepartmentOption != null && vModel.AdmissionListDetail.DepartmentOption.Id > 0)
                        {
                            admissionList.DepartmentOption = vModel.AdmissionListDetail.DepartmentOption;
                        }
                        var user = new User();
                        var userLogic = new UserLogic();
                        user = userLogic.GetModelBy(p => p.User_Name == User.Identity.Name);
                        string client = Request.LogonUserIdentity.Name + " (" + HttpContext.Request.UserHostAddress +")";

                        var Audit = new AdmissionListAudit();
                        Audit.Client = client;
                        Audit.Action = "UPDATE";
                        Audit.Operation = "UPDATING ADMISSION LIST";
                        Audit.User = user;

                        bool isUpdate = admissionListLogic.Update(admissionList,Audit);
                        if(isUpdate)
                        {
                            TempData["UpdateSuccess"] = "Student Admission Details Updated Successfully";
                            return RedirectToAction("SearchAdmittedStudents");
                        }
                        TempData["UpdateFailure"] = "Student Admission Details Update Failed";
                        return RedirectToAction("SearchAdmittedStudents");
                    }
                }
                return RedirectToAction("SearchAdmittedStudents");
            }
            catch(Exception)
            {
                throw;
            }
        }
   
        
        public ActionResult UnregisteredStudent()
        {
          try
            {


                viewmodel = new UploadAdmissionViewModel();
                viewmodel.ProgrammeSelectListItem = Utility.PopulateAllProgrammeSelectListItem();
                viewmodel.SessionSelectListItem = Utility.PopulateSessionSelectListItem();
                viewmodel.AdmissionListTypeSelectListItem = Utility.PopulateAdmissionListTypeSelectListItem();
                ViewBag.ProgrammeId = viewmodel.ProgrammeSelectListItem;
                ViewBag.SessionId = viewmodel.SessionSelectListItem;
                ViewBag.AdmissionListTypeId = viewmodel.AdmissionListTypeSelectListItem;
                ViewBag.DepartmentId = new SelectList(new List<Department>(), ID, NAME);

            }
            catch (Exception ex)
            {
                SetMessage("Error Occurred! " + ex.Message, Message.Category.Error);
            }
            return View();
        }
        [HttpPost]
        public ActionResult UnregisteredStudent(UploadAdmissionViewModel vmodel)
        {
            try
            {
                KeepApplicationFormInvoiceGenerationDropDownState(vmodel);
                vmodel.UnregisteredAdmissionList = new List<UnregisteredStudent>();
                List<AppliedCourse> applicants = new List<AppliedCourse>();
                string pathForSaving = Server.MapPath("~/Content/ExcelUploads");
                string savedFileName = "";
                foreach (string file in Request.Files)
                {
                    HttpPostedFileBase hpf = Request.Files[file] as HttpPostedFileBase;
                    if (hpf.ContentLength == 0)
                        continue;
                    if (this.CreateFolderIfNeeded(pathForSaving))
                    {
                        FileInfo fileInfo = new FileInfo(hpf.FileName);
                        string fileExtension = fileInfo.Extension;
                        string newFile = "Admission" + "__";
                        string newFileName = newFile + DateTime.Now.ToString().Replace("/", "").Replace(":", "").Replace(" ", "") + fileExtension;

                        savedFileName = Path.Combine(pathForSaving,newFileName);
                        hpf.SaveAs(savedFileName);
                    }

                    DataSet dsAdmissionList = ReadExcel(savedFileName);
                    if (dsAdmissionList != null && dsAdmissionList.Tables[0].Rows.Count > 0)
                    {
                        for (int i = 0; i < dsAdmissionList.Tables[0].Rows.Count; i++)
                        {
                            if (dsAdmissionList.Tables[0].Rows[i][1].ToString() != "")
                            {
                               UnregisteredStudent student = new UnregisteredStudent();
                               student.Surname = dsAdmissionList.Tables[0].Rows[i][1].ToString();
                               student.Firstname = dsAdmissionList.Tables[0].Rows[i][2].ToString();
                               student.Othername = dsAdmissionList.Tables[0].Rows[i][3].ToString();
                               student.JambNumberNumber = dsAdmissionList.Tables[0].Rows[i][4].ToString();
                               student.Deprtment = new Department() {Id = vmodel.AdmissionListDetail.Deprtment.Id};
                               student.Programme = new Programme(){Id = vmodel.AdmissionListDetail.Form.ProgrammeFee.Programme.Id};
                               student.Session = new Session(){Id = vmodel.CurrentSession.Id};
                               vmodel.UnregisteredAdmissionList.Add(student);
                                
                            }
                        }
                        TempData["UploadAdmissionViewModel"] = vmodel;
                        return View(vmodel);

                    }
                }
            }
            catch (Exception)
            {

                throw;
            }
            return View();


        }
        [HttpPost]
        public ActionResult SaveUnRegisteredAdmissionList(UploadAdmissionViewModel vmodel)
        {
            try
            {
                int UploadCount = 0;
                int UpdateCount = 0;
                string exitingNumbers = "";
                string operation = "INSERT";
                string action = "UPLOADING OF ADMISSION LIST -UNREGISTERED STUDENT- ";
                string client = Request.LogonUserIdentity.Name + " (" + HttpContext.Request.UserHostAddress + ")";
                AdmissionListLogic admissionListLogic = new AdmissionListLogic();
                            
                vmodel = (UploadAdmissionViewModel)TempData["UploadAdmissionViewModel"];
                if (vmodel.UnregisteredAdmissionList != null && vmodel.UnregisteredAdmissionList.Count > 0)
                {
                    using (TransactionScope transaction = new TransactionScope())
                    {
                        AdmissionListBatch batch = new AdmissionListBatch();
                        AdmissionListBatchLogic batchLogic = new AdmissionListBatchLogic();
                        AdmissionListType AdmissionType = new AdmissionListType();
                        AdmissionListAudit AdmissionListAudit = new Model.Model.AdmissionListAudit();
                        UserLogic loggeduser = new UserLogic();
                        AdmissionType = vmodel.AdmissionListType;
                        batch.DateUploaded = DateTime.Now;
                        batch.IUploadedFilePath = "NAN";
                        batch.Type = AdmissionType;
                        batch = batchLogic.Create(batch);

                        AdmissionListAudit.Action = action;
                        AdmissionListAudit.Client = client;
                        AdmissionListAudit.Operation = operation;
                        AdmissionListAudit.Time = DateTime.Now;
                        AdmissionListAudit.User = loggeduser.GetModelBy(u => u.User_Name == User.Identity.Name);

                       
                        for (int i = 0; i < vmodel.UnregisteredAdmissionList.Count; i++)
                        {
                            string jambNo = vmodel.UnregisteredAdmissionList[i].JambNumberNumber;
                            ApplicantJambDetailLogic jambDetailLogic = new ApplicantJambDetailLogic();
                            var jambDetail = jambDetailLogic.GetModelsBy(a => a.Applicant_Jamb_Registration_Number == jambNo && a.Application_Form_Id > 0).FirstOrDefault();
                            if (jambDetail != null)
                            {
                                if (!admissionListLogic.IsAdmittedByJamb(jambDetail.JambRegistrationNumber))
                                {
                                    AdmissionList list = new AdmissionList();
                                    list.Form = jambDetail.ApplicationForm;
                                    list.Deprtment =  new Department(){Id = vmodel.UnregisteredAdmissionList[i].Deprtment.Id};
                                    list.Programme =  new Programme(){Id = vmodel.UnregisteredAdmissionList[i].Programme.Id};
                                    list.Activated = true;
                                    admissionListLogic.Create(list, batch, AdmissionListAudit);
                                    UploadCount++;
                                }
                                else
                                {
                                    var admission = admissionListLogic.GetBy(jambDetail.JambRegistrationNumber);
                                    exitingNumbers = exitingNumbers + jambDetail.JambRegistrationNumber + ", ";
                                    //admission.Deprtment =  new Department(){Id = vmodel.UnregisteredAdmissionList[i].Deprtment.Id};
                                    //admission.Programme =  new Programme(){Id = vmodel.UnregisteredAdmissionList[i].Programme.Id};
                                    //admissionListLogic.Update(admission, AdmissionListAudit);
                                    UpdateCount++;
                                }

                                continue;
                            }
                          
                            //create person
                            var applicantSex = new Sex {Id = 1};
                            string mobile = "NAN";
                            string hometown = "NAN";
                            string permanentAddress = "NAN";
                            string contact_address = "NAN";
                            string email = "NAN";
                            var role = new Role {Id = 6};
                            var personType = new PersonType {Id = 4};
                            var nationality = new Nationality {Id = 1};
                            var religion = new Religion {Id = 1};
                            var applicantPerson = new Person();
                            applicantPerson.DateEntered = DateTime.Now;
                            applicantPerson.MobilePhone = mobile;
                            applicantPerson.Religion = religion;
                            applicantPerson.HomeTown = hometown;
                            applicantPerson.HomeAddress = permanentAddress;
                            applicantPerson.Role = role;
                            applicantPerson.Type = personType;
                            applicantPerson.Nationality = nationality;
                            applicantPerson.LastName = vmodel.UnregisteredAdmissionList[i].Surname;
                            applicantPerson.FirstName = vmodel.UnregisteredAdmissionList[i].Firstname;
                            applicantPerson.OtherName = vmodel.UnregisteredAdmissionList[i].Othername;
                            applicantPerson.State = new State(){Id = "IM"};
                            applicantPerson.LocalGovernment = new LocalGovernment(){Id = 1};
                            applicantPerson.ContactAddress = contact_address;
                            applicantPerson.Sex = applicantSex;
                            applicantPerson.Email = email;
                            var personLogic = new PersonLogic();
                            applicantPerson = personLogic.Create(applicantPerson);

                            //Create Payment
                            PaymentLogic paymentLogic = new PaymentLogic();
                            Payment payment = new Payment();
                            payment.Person = applicantPerson;
                            payment.PaymentMode = new PaymentMode() { Id = 1 };
                            payment.PaymentType = new PaymentType() { Id = 2 };
                            payment.PersonType = new PersonType() { Id = 4 };
                            payment.FeeType = new FeeType() { Id = 1 };
                            payment.DatePaid = DateTime.Now;
                            payment.Session = new Session() { Id = 7};

                            OnlinePayment newOnlinePayment = null;
                            Payment newPayment = paymentLogic.Create(payment);
                            if (newPayment != null)
                            {
                                PaymentChannel channel = new PaymentChannel() { Id = (int)PaymentChannel.Channels.Etranzact };
                                OnlinePaymentLogic onlinePaymentLogic = new OnlinePaymentLogic();
                                OnlinePayment onlinePayment = new OnlinePayment();
                                onlinePayment.Channel = channel;
                                onlinePayment.Payment = newPayment;
                                newOnlinePayment = onlinePaymentLogic.Create(onlinePayment);
                            }

                            //Create Applied Course
                            var applicantAppliedCourse = new AppliedCourse();
                            var appliedCourseLogic = new AppliedCourseLogic();
                            applicantAppliedCourse.Programme = new Programme {Id = vmodel.UnregisteredAdmissionList[i].Programme.Id};
                            applicantAppliedCourse.Department = new Department(){Id = vmodel.UnregisteredAdmissionList[i].Deprtment.Id};
                            applicantAppliedCourse.Person = applicantPerson;
                            applicantAppliedCourse = appliedCourseLogic.Create(applicantAppliedCourse);


                            //create application form
                            var applicationForm = new ApplicationForm();
                            var applicationFormLogic = new ApplicationFormLogic();
                            applicationForm.DateSubmitted = DateTime.Now;
                            applicationForm.Payment = newPayment;
                            applicationForm.Person = applicantPerson;
                            applicationForm.ProgrammeFee = new ApplicationProgrammeFee {Id = 1};
                            applicationForm.RejectReason = "";
                            applicationForm.Remarks = "-UNREGISTERED STUDENT-";
                            applicationForm.Rejected = false;
                            applicationForm.Release = true;
                            applicationForm.Setting = new ApplicationFormSetting {Id = 2};
                            applicationForm = applicationFormLogic.Create(applicationForm);

                            //Update Applied Course
                            applicantAppliedCourse.Programme = new Programme {Id = vmodel.UnregisteredAdmissionList[i].Programme.Id};
                            applicantAppliedCourse.Department = new Department(){Id = vmodel.UnregisteredAdmissionList[i].Deprtment.Id};
                            applicantAppliedCourse.Person = applicantPerson;
                            applicantAppliedCourse.ApplicationForm = applicationForm;
                            appliedCourseLogic.Modify(applicantAppliedCourse);
                           
                            //create Applicant
                            Model.Model.Applicant applicant = new Model.Model.Applicant();
                            ApplicantLogic applicantLogic = new ApplicantLogic();
                            applicant.Ability = new Ability(){Id = 1};
                            applicant.ApplicationForm = applicationForm;
                            applicant.ExtraCurricullarActivities = "None";
                            applicant.OtherAbility = "None";
                            applicant.Person = applicantPerson;
                            applicant.Status = new ApplicantStatus(){Id = 1};
                            applicantLogic.Create(applicant);

                            //create applicant jamb detail
                            if (vmodel.UnregisteredAdmissionList[i].JambNumberNumber != null)
                            {
                                var applicantJambDetail = new ApplicantJambDetail();
                                var applicantJambDetailLogic = new ApplicantJambDetailLogic();
                                applicantJambDetail.Person = applicantPerson;
                                applicantJambDetail.ApplicationForm = applicationForm;
                                applicantJambDetail.JambRegistrationNumber = vmodel.UnregisteredAdmissionList[i].JambNumberNumber;
                                applicantJambDetail = applicantJambDetailLogic.Create(applicantJambDetail);
                            }

                            //add to admissionList
                            AdmissionList admissionlist = new AdmissionList();
                            admissionlist.Form = applicationForm;
                            admissionlist.Deprtment =  new Department(){Id = vmodel.UnregisteredAdmissionList[i].Deprtment.Id};
                            admissionlist.Programme =  new Programme(){Id = vmodel.UnregisteredAdmissionList[i].Programme.Id};
                            admissionlist.Activated = true;
                            admissionListLogic.Create(admissionlist, batch, AdmissionListAudit);
                            UploadCount++;
                        }
                        transaction.Complete();
                    }

                    SetMessage( UploadCount.ToString() + " out of "+ vmodel.UnregisteredAdmissionList.Count.ToString() + " names List was uploaded while "+UpdateCount+ " were found already existing.", Message.Category.Information);
                    return RedirectToAction("UnregisteredStudent");
                }
            }
            catch (Exception ex)
            {
                
                throw;
            }
            SetMessage("List was not uploaded successfully, Please check for duplicates and try again", Message.Category.Information);
                  
            return View();
        }

        public ActionResult ApplicantList()
        {
            try
            {
                viewmodel = new UploadAdmissionViewModel();
                ViewBag.SessionId = viewmodel.AllSessionSelectListItem;
            }
            catch (Exception ex)
            {
                SetMessage("Error Occurred! " + ex.Message, Message.Category.Error);
            }
            return View();
        }

        [HttpPost]
        public ActionResult ApplicantList(UploadAdmissionViewModel viewModel)
        {
            try
            {
                ApplicationFormLogic applicationFormLogic = new ApplicationFormLogic();

                if (viewModel.CurrentSession != null)
                {
                    viewModel.Applicants = applicationFormLogic.GetApplicantList(viewModel.CurrentSession, viewModel.DateFrom, viewModel.DateTo);
                }
            }
            catch (Exception ex)
            {
                SetMessage("Error Occurred! " + ex.Message, Message.Category.Error);
            }

            ViewBag.SessionId = viewModel.AllSessionSelectListItem;

            return View(viewModel);
        }
        public ActionResult AdmissionList()
        {
            UploadAdmissionViewModel viewModel = new UploadAdmissionViewModel();
            try
            {
                ViewBag.Session = viewModel.AllSessionSelectListItem;
                ViewBag.Department = new SelectList(new List<Department>(), ID, NAME);
                ViewBag.Programme = viewModel.ProgrammeSelectListItem;

            }
            catch (Exception ex)
            {
                SetMessage("Error! " + ex.Message, Message.Category.Error);
            }
            return View(viewModel);
        }
        [HttpPost]
        public ActionResult AdmissionList(UploadAdmissionViewModel viewModel)
        {
            try
            {
                AdmissionListLogic admissionListLogic = new AdmissionListLogic();

                if (viewModel.CurrentSession != null && viewModel.CurrentSession.Id > 0 && viewModel.Programme != null && viewModel.Programme.Id > 0 && viewModel.Department != null && viewModel.Department.Id > 0)
                {
                    viewModel.AdmissionModelList = admissionListLogic.GetAdmissionListBy(viewModel.CurrentSession, viewModel.Programme, viewModel.Department, viewModel.DateFrom, viewModel.DateTo);
                }

            }
            catch (Exception ex)
            {
                SetMessage("Error! " + ex.Message, Message.Category.Error);
            }

            ViewBag.Session = viewModel.AllSessionSelectListItem;
            ViewBag.Department = new SelectList(new List<Department>(), ID, NAME);
            ViewBag.Programme = viewModel.ProgrammeSelectListItem;

            return View(viewModel);
        }
        public ActionResult StudentIdData(string fileName = null)
        {
            UploadAdmissionViewModel viewModel = new UploadAdmissionViewModel();
            try
            {
                ViewBag.Session = viewModel.AllSessionSelectListItem;

            }
            catch (Exception ex)
            {
                SetMessage("Error! " + ex.Message, Message.Category.Error);
            }

            if (fileName != null)
            {
                return File(Server.MapPath("~/Content/tempFolder/" + fileName), "application/zip", fileName);
            }

            return View(viewModel);
        }
        [HttpPost]
        public ActionResult StudentIdData(UploadAdmissionViewModel viewModel)
        {
            try
            {
                StudentLogic studentLogic = new StudentLogic();

                if (viewModel.DateTo.Year > 1 && viewModel.DateFrom.Year > 1)
                {
                    viewModel.AdmissionModelList = studentLogic.GetStudentIdData(viewModel.DateFrom, viewModel.DateTo);
                }
                else
                {
                    SetMessage("Error! Please select a date range ", Message.Category.Error);
                }

            }
            catch (Exception ex)
            {
                SetMessage("Error! " + ex.Message, Message.Category.Error);
            }

            TempData["CardDetails"] = viewModel.AdmissionModelList;
            TempData["Session"] = viewModel.CurrentSession;
            
            ViewBag.Session = viewModel.AllSessionSelectListItem;

            return View(viewModel);
        }


        public ActionResult AdmissionListBulk()
        {
            try
            {
                viewmodel = new UploadAdmissionViewModel();
                ViewBag.Session = viewmodel.AllSessionSelectListItem;

            }
            catch (Exception ex)
            {
                SetMessage("Error! " + ex.Message, Message.Category.Error);
            }
            return View();
        }
        [HttpPost]
        public ActionResult AdmissionListBulk(UploadAdmissionViewModel viewModel)
        {
            try
            {
                AdmissionListLogic admissionListLogic = new AdmissionListLogic();

                if (viewModel.CurrentSession != null && viewModel.CurrentSession.Id > 0)
                {
                    viewModel.AdmissionModelList = admissionListLogic.GetAdmissionListBulk(viewModel.CurrentSession, Convert.ToDateTime(viewModel.DateFrom), Convert.ToDateTime(viewModel.DateTo));

                    var gv = new GridView();

                    if (viewModel.AdmissionModelList != null && viewModel.AdmissionModelList.Count > 0)
                    {
                        gv.DataSource = viewModel.AdmissionModelList;

                        gv.Caption = "Admission List";
                        gv.DataBind();
                        string filename = "Admission List";

                        ViewBag.SessionId = viewModel.AllSessionSelectListItem;

                        return new DownloadFileActionResult(gv, filename + ".xls");
                    }

                    Response.Write("No data available for download");
                    Response.End();

                    ViewBag.Session = viewModel.AllSessionSelectListItem;

                    return new JavaScriptResult();
                }

            }
            catch (Exception ex)
            {
                SetMessage("Error! " + ex.Message, Message.Category.Error);
            }

            ViewBag.Session = viewModel.AllSessionSelectListItem;

            return View(viewModel);
        }
        public ActionResult ApplicantListBulk()
        {
            try
            {
                viewmodel = new UploadAdmissionViewModel();
                ViewBag.SessionId = viewmodel.AllSessionSelectListItem;
            }
            catch (Exception ex)
            {
                SetMessage("Error Occurred! " + ex.Message, Message.Category.Error);
            }
            return View();
        }


        [HttpPost]
        public ActionResult ApplicantListBulk(UploadAdmissionViewModel viewModel)
        {
            try
            {
                ApplicationFormLogic applicationFormLogic = new ApplicationFormLogic();

                if (viewModel.CurrentSession != null)
                {
                    viewModel.Applicants = applicationFormLogic.GetApplicantListBulk(viewModel.CurrentSession, Convert.ToDateTime(viewModel.DateFrom), Convert.ToDateTime(viewModel.DateTo));

                    var gv = new GridView();

                    if (viewModel.Applicants != null && viewModel.Applicants.Count > 0)
                    {
                        gv.DataSource = viewModel.Applicants;

                        gv.Caption = "Applicants";
                        gv.DataBind();
                        string filename = "Applicants";

                        ViewBag.SessionId = viewModel.AllSessionSelectListItem;

                        return new DownloadFileActionResult(gv, filename + ".xls");
                    }

                    Response.Write("No data available for download");
                    Response.End();

                    ViewBag.SessionId = viewModel.AllSessionSelectListItem;

                    return new JavaScriptResult();
                }
            }
            catch (Exception ex)
            {
                SetMessage("Error Occurred! " + ex.Message, Message.Category.Error);
            }

            ViewBag.SessionId = viewModel.AllSessionSelectListItem;

            return View(viewModel);
        }
        public ActionResult DownloadIdCardPassport()
        {
            string zipFileName = null;
            List<AdmissionListModel> studentIdDataList = (List<AdmissionListModel>)TempData["CardDetails"];
            try
            {
                if (studentIdDataList != null && studentIdDataList.Count > 0)
                {
                    if (Directory.Exists(Server.MapPath("~/Content/tempFolder")))
                    {
                        Directory.Delete(Server.MapPath("~/Content/tempFolder"), true);
                        Directory.CreateDirectory(Server.MapPath("~/Content/tempFolder"));
                    }
                    else
                    {
                        Directory.CreateDirectory(Server.MapPath("~/Content/tempFolder"));
                    }

                    List<AdmissionListModel> sort = new List<AdmissionListModel>();

                    GridView gv = new GridView();

                    for (int i = 0; i < studentIdDataList.Count; i++)
                    {
                        AdmissionListModel studentIdCard = studentIdDataList[i];
                        string imagePath = studentIdCard.ImageUrl;

                        if (string.IsNullOrEmpty(imagePath))
                        {
                            continue;
                        }

                        string[] splitUrl = imagePath.Split('/');
                        string imageUrl = splitUrl.LastOrDefault();
                        FileInfo fileInfo = new FileInfo(imageUrl);
                        string fileExtension = fileInfo.Extension;
                        string newFileName = imageUrl.Split('.')[0] + fileExtension;

                        if (!System.IO.File.Exists(Server.MapPath("~" + imagePath)))
                        {
                            continue;

                        }


                        System.IO.File.Copy(Server.MapPath(imagePath), Server.MapPath(Path.Combine("~/Content/tempFolder/", imageUrl)), true);

                        studentIdCard.ImageUrl = studentIdCard.ImageUrl;

                        sort.Add(studentIdCard);

                    }

                    gv.DataSource = sort;
                    gv.Caption = studentIdDataList.FirstOrDefault().SessionName + " " + "SESSION";
                    gv.DataBind();
                    SaveStudentDetailsToExcel(gv, "ID Card Data.xls");

                    zipFileName = "ABSU Student Passport " + studentIdDataList.FirstOrDefault().SessionName.Replace("/", "_") + ".zip";

                    using (ZipFile zip = new ZipFile())
                    {
                        string file = Server.MapPath("~/Content/tempFolder/");
                        zip.AddDirectory(file, "");

                        zip.Save(file + zipFileName);
                    }

                    //savedFile = "~/Content/tempFolder/" + zipFileName;

                }
            }
            catch (Exception ex)
            {
                SetMessage("Error Occured!" + ex.Message, Message.Category.Error);
            }

            return RedirectToAction("StudentIdData", "UploadAdmission", new { fileName = zipFileName });
        }
        public void SaveStudentDetailsToExcel(GridView ExcelGridView, string fileName)
        {
            try
            {
                Response.Clear();

                Response.Charset = "";

                Response.Cache.SetCacheability(HttpCacheability.NoCache);

                Response.ContentType = "application/vnd.ms-excel";

                StringWriter sw = new StringWriter();
                HtmlTextWriter htw = new HtmlTextWriter(sw);
                ExcelGridView.RenderControl(htw);

                Response.Write(sw.ToString());
                string renderedGridView = sw.ToString();
                System.IO.File.WriteAllText(Server.MapPath(Path.Combine("~/Content/tempFolder/", fileName)), renderedGridView);

            }
            catch (Exception ex)
            {

                throw;
            }
        }
    }
}