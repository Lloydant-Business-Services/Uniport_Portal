using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Transactions;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using Abundance_Nk.Business;
using Abundance_Nk.Model.Entity;
using Abundance_Nk.Model.Model;
using Abundance_Nk.Web.Areas.Admin.ViewModels;
using Abundance_Nk.Web.Controllers;
using System.IO;
using System.Configuration;
using Abundance_Nk.Web.Areas.Applicant.Controllers;

using System.Data.Entity.Validation;
using System.Net;
using System.Text;
using GridMvc;
using Microsoft.Ajax.Utilities;
using Microsoft.Reporting.WebForms.Internal.Soap.ReportingServices2005.Execution;



namespace Abundance_Nk.Web.Areas.Admin.Controllers
{
    public class StaffProfileController : BaseController
    {
        private const string ID = "Id";
        private const string NAME = "Name";
        private const string VALUE = "Value";
        private const string TEXT = "Text";
        private const string DEFAULT_PASSPORT = "/Content/Images/default_avatar.png";

        private string appRoot = ConfigurationManager.AppSettings["AppRoot"];

       
        public ActionResult Index()
        {
           StaffDataViewModel viewModel = new StaffDataViewModel();
            try
            {
                UserLogic userLogic = new UserLogic();
                StaffLogic staffLogic = new StaffLogic();
                PersonLogic personLogic = new PersonLogic();
                StaffDepartmentLogic staffDepartmentLogic = new StaffDepartmentLogic();
                StaffQualificationLogic staffQualificationLogic = new StaffQualificationLogic();
                EmployeeDetailLogic employeeDetailLogic = new EmployeeDetailLogic();


                Model.Model.User user = userLogic.GetModelsBy(u => u.User_Name == User.Identity.Name).LastOrDefault();

                if (user != null)
                {
                var getStaff = staffLogic.GetModelBy(s => s.User_Id == user.Id);
                    if (getStaff != null)
                    {
                        if(getStaff.BloodGroup == null)
                        {
                            BloodGroup bloodGroup = new BloodGroup();
                            getStaff.BloodGroup = bloodGroup;
                        }

                        if(getStaff.Genotype == null)
                        {
                            Genotype genotype = new Genotype();
                            getStaff.Genotype = genotype;
                        }

                        if (getStaff.MaritalStatus == null)
                        {
                            MaritalStatus maritalStatus = new MaritalStatus();
                            getStaff.MaritalStatus = maritalStatus;
                        }
                        if(getStaff.Department == null)
                        {
                            Department department = new Department();
                            getStaff.Department = department;
                        }
                        if(getStaff.Type == null)
                        {
                            StaffType staffType = new StaffType();
                            getStaff.Type = staffType;
                        }
                        if(getStaff.Person == null)
                        {
                            Person person = new Person();
                            getStaff.Person = person;
                        }
                        viewModel.Staff = getStaff;
                        
                        var getEmployee = employeeDetailLogic.GetModelBy(s => s.Staff_Id == getStaff.Id);
                        if(getEmployee != null)
                        {
                            viewModel.EmployeeDetail = getEmployee;
                        }

                        var getQualification = staffQualificationLogic.GetModelBy(s => s.Staff_Id == getStaff.Id);
                        if(getQualification != null)
                        {
                            viewModel.StaffQualification = getQualification;
                        }
                    }
                    else
                    {
                        SetMessage("This user does not have a staff account please contact the system administrator for setup ", Message.Category.Error);
                        return RedirectToAction("Security/Account/Home");
                    }
                
                }

                ViewBag.Sex = viewModel.SexSelectList;
                ViewBag.State = viewModel.StateSelectList;
                ViewBag.LGA = new SelectList(new List<LocalGovernment>(), "Id", "Name");
                ViewBag.Religion = viewModel.ReligionSelectList;
                ViewBag.MaritalStatus = viewModel.MaritalStatusSelectList;
                ViewBag.StaffType = viewModel.StaffTypeSelectList;
                ViewBag.Genotype = viewModel.GenotypeSelectList;
                ViewBag.BloodGroup = viewModel.BloodGroupSelectList;
                ViewBag.Department = viewModel.DepartmentSelectList;
                ViewBag.Designation = viewModel.DesignationSelectList;
                ViewBag.GradeLevel = viewModel.GradeLevelSelectList;
                ViewBag.Unit = viewModel.UnitSelectList;
                ViewBag.EducationalQualification = viewModel.EducationalQualificationSelectList;
                ViewBag.StaffResultGrade = viewModel.StaffResultGradeSelectList;
            }
            catch (Exception ex)
            {
                SetMessage("Error! " + ex.Message, Message.Category.Error);
            }

            return View(viewModel);
        }
        public JsonResult GetLocalGovernmentsByState(string id)
        {
            try
            {
                LocalGovernmentLogic lgaLogic = new LocalGovernmentLogic();

                Expression<Func<LOCAL_GOVERNMENT, bool>> selector = l => l.State_Id == id;
                List<LocalGovernment> lgas = lgaLogic.GetModelsBy(selector);

                return Json(new SelectList(lgas, "Id", "Name"), JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public JsonResult SavePersonalDetails(string myRecordArray)
        {
            JsonResultModel result = new JsonResultModel();
            try
            {
                JavaScriptSerializer serializer = new JavaScriptSerializer();
                PersonModel personDetails = serializer.Deserialize<PersonModel>(myRecordArray);

                Person person = new Person();
                PersonLogic personLogic = new PersonLogic();
                StaffLogic staffLogic = new StaffLogic();

                using (TransactionScope scope = new TransactionScope())
                {
                    person.FirstName = personDetails.FirstName;
                    person.LastName = personDetails.LastName;
                    person.OtherName = personDetails.OtherName;
                    person.MobilePhone = personDetails.MobilePhone;
                    person.ContactAddress = personDetails.ContactAddress;
                    person.Email = personDetails.Email;
                    long staffId = Convert.ToInt64(personDetails.Id); //converting my staff id to person
                    var getStaff = staffLogic.GetModelBy(s => s.Staff_Id == staffId);
                    if(getStaff.Person != null && getStaff.Person.Id > 0)
                    {
                        person.Id = getStaff.Person.Id;
                    }
                    else
                    {
                        person.Id = 0;
                    }
                    DateTime dob = new DateTime();

                    if (!DateTime.TryParse(personDetails.DOBString, out dob))
                    {
                        dob = DateTime.UtcNow;
                    }

                    person.DateOfBirth = dob;
                    person.LocalGovernment = new LocalGovernment() { Id = Convert.ToInt32(personDetails.LGAId) };
                    person.State = new State() { Id = personDetails.StateId };
                    person.Nationality = new Nationality() { Id = 1 };
                    person.Religion = new Religion() { Id = Convert.ToInt32(personDetails.ReligionId) };
                    
                    person.Sex = new Sex() { Id = Convert.ToByte(personDetails.SexId) };
                    person.Type = new PersonType() { Id = 1 };
                    person.Role = new Role() { Id = 7 };
                    if (person.Id <= 0)
                    {
                        person.DateEntered = DateTime.Now;
                        person = personLogic.Create(person);
                    }
                    else
                    {
                        personLogic.Modify(person);
                    }

                    if (person != null && person.Id > 0)
                    {
                        
                        UserLogic userLogic = new UserLogic();
                        Staff staff = new Staff();

                        staff.BloodGroup = new BloodGroup() { Id = Convert.ToInt32(personDetails.BloodGroupId) };
                       // staff.Religion = new Religion() { Id = Convert.ToInt32(personDetails.ReligionId) };
                        staff.Genotype = new Genotype() { Id = Convert.ToInt32(personDetails.GenotypeId) };
                       // staff.ProfileDescription = personDetails.ProfileDescription;
                        staff.Type = new StaffType() { Id = Convert.ToInt32(personDetails.StaffTypeId) };
                        staff.User = userLogic.GetModelsBy(u => u.User_Name == User.Identity.Name).LastOrDefault();
                        staff.isManagement = false;
                        staff.MaritalStatus = new MaritalStatus() { Id = Convert.ToInt32(personDetails.MaritalStatusId) };
                        staff.ProfileDescription = personDetails.ProfileDescription;
                        staff.Id = staffId;
                        staff.Person = person;



                        if (staffLogic.GetModelBy(s => s.Staff_Id == staffId) == null)
                        {
                            staff = staffLogic.Create(staff);
                        }
                        else
                        {
                            staffLogic.Modify(staff);
                        }
                    }

                    scope.Complete();
                }

                result.Id = person.Id.ToString();
                result.IsError = false;
                result.Message = "Operation Successful!";
            }
            catch (Exception ex)
            {
                result.IsError = true;
                result.Message = "Error! " + ex.Message;
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public JsonResult SaveEmploymentInformation(string myRecordArray)
        {
            JsonResultModel result = new JsonResultModel();
            try
            {
                JavaScriptSerializer serializer = new JavaScriptSerializer();
                PersonModel personDetails = serializer.Deserialize<PersonModel>(myRecordArray);

                long staffId = !string.IsNullOrEmpty(personDetails.Id) ? Convert.ToInt32(personDetails.Id) : 0;
                UserLogic userLogic = new UserLogic();
                Model.Model.User user = userLogic.GetModelsBy(u => u.User_Name == User.Identity.Name).LastOrDefault();

                StaffDepartment staffDepartment = new StaffDepartment();
                StaffDepartmentLogic staffDepartmentLogic = new StaffDepartmentLogic();

                using (TransactionScope scope = new TransactionScope())
                {
                    if (user != null && user.Id > 0)
                    {
                        staffDepartment.StaffUserID = user.Id;
                        staffDepartment.DateEntered = DateTime.Now;
                        staffDepartment.Department = new Department() { Id = Convert.ToInt32(personDetails.DepartmentId) };
                        staffDepartment.IsHead = false;

                        StaffDepartment existingStaffDepartment = staffDepartmentLogic.GetModelsBy(s => s.Staff_Id == user.Id).LastOrDefault();

                        if (existingStaffDepartment == null)
                        {
                            staffDepartment = staffDepartmentLogic.Create(staffDepartment);
                        }
                        else
                        {
                            staffDepartment.Id = existingStaffDepartment.Id;
                            staffDepartmentLogic.Modify(staffDepartment);
                        }

                        EmployeeDetailLogic employeeDetailLogic = new EmployeeDetailLogic();
                        EmployeeDetail employeeDetail = new EmployeeDetail();

                        employeeDetail.Department = Convert.ToInt32(personDetails.DepartmentId) != 0 ? new Department() { Id = Convert.ToInt32(personDetails.DepartmentId) } : null;
                        employeeDetail.Unit = Convert.ToInt32(personDetails.UnitId) != 0 ? new Model.Model.Unit() { Id = Convert.ToInt32(personDetails.UnitId) } : null;
                        employeeDetail.GradeLevel = Convert.ToInt32(personDetails.GradeLevelId) != 0 ? new GradeLevel() { Id = Convert.ToInt32(personDetails.GradeLevelId) } : null;
                        employeeDetail.Designation = Convert.ToInt32(personDetails.DesignationId) != 0 ? new Designation() { Id = Convert.ToInt32(personDetails.DesignationId) } : null;
                        if (!string.IsNullOrEmpty(personDetails.DateOfPresentAppointment))
                        {
                            DateTime presentAppointmentDate = new DateTime();

                            if (DateTime.TryParse(personDetails.DateOfPresentAppointment, out presentAppointmentDate))
                            {
                                employeeDetail.DateOfPresentAppointment = presentAppointmentDate;
                                employeeDetail.YearOfEmployment = presentAppointmentDate.Year;
                            }
                        }
                        if (!string.IsNullOrEmpty(personDetails.DateOfPreviousAppointment))
                        {
                            DateTime previousAppointmentDate = new DateTime();

                            if (DateTime.TryParse(personDetails.DateOfPreviousAppointment, out previousAppointmentDate))
                            {
                                employeeDetail.DateOfPreviousApointment = previousAppointmentDate;
                            }
                        }
                        if (!string.IsNullOrEmpty(personDetails.DateOfRetirement))
                        {
                            DateTime retirementDate = new DateTime();

                            if (DateTime.TryParse(personDetails.DateOfRetirement, out retirementDate))
                            {
                                employeeDetail.DateOfRetirement = retirementDate;
                            }
                        }
                        employeeDetail.StaffNumber = personDetails.StaffNumber;
                        employeeDetail.EmploymentLocation = "Abia";
                        employeeDetail.Id = staffId;

                        EmployeeDetail existingEmployeeDetail = employeeDetailLogic.GetModelsBy(s => s.Staff_Id == user.Id).LastOrDefault();
                        if (existingEmployeeDetail == null)
                        {
                            employeeDetailLogic.Create(employeeDetail);
                        }
                        else
                        {
                            employeeDetail.Id = existingEmployeeDetail.Id;
                            employeeDetailLogic.Modify(employeeDetail);
                        }
                    }

                    scope.Complete();
                }

                result.Id = staffId.ToString();
                result.IsError = false;
                result.Message = "Operation Successful!";
            }
            catch (Exception ex)
            {
                result.IsError = true;
                result.Message = "Error! " + ex.Message;
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult SaveQualificationInformation(string myRecordArray)
        {
            JsonResultModel result = new JsonResultModel();
            try
            {
                JavaScriptSerializer serializer = new JavaScriptSerializer();
                PersonModel personDetails = serializer.Deserialize<PersonModel>(myRecordArray);

                long staffId = !string.IsNullOrEmpty(personDetails.Id) ? Convert.ToInt32(personDetails.Id) : 0;

                StaffQualificationLogic staffQualificationLogic = new StaffQualificationLogic();
                StaffQualification staffQualification = new StaffQualification();

                if (staffId > 0)
                {
                    staffQualification.Staff = new Staff() { Id = staffId };
                    staffQualification.InstitutionAttended = personDetails.InstitutionAttended;
                    staffQualification.EducationalQualification = new EducationalQualification() { Id = Convert.ToInt32(personDetails.EducationalQualificationId) };
                    staffQualification.StaffResultGrade = new StaffResultGrade() { Id = Convert.ToInt32(personDetails.StaffResultGradeId) };
                    staffQualification.CertificateNumber = personDetails.CertificateNumber;
                    if (!string.IsNullOrEmpty(personDetails.FromDate))
                    {
                        DateTime fromDate = new DateTime();

                        if (DateTime.TryParse(personDetails.FromDate, out fromDate))
                        {
                            staffQualification.FromDate = fromDate;
                        }
                    }
                    if (!string.IsNullOrEmpty(personDetails.ToDate))
                    {
                        DateTime toDate = new DateTime();

                        if (DateTime.TryParse(personDetails.ToDate, out toDate))
                        {
                            staffQualification.ToDate = toDate;
                        }
                    }

                    StaffQualification existingStaffQualification = staffQualificationLogic.GetModelsBy(s => s.Staff_Id == staffId).LastOrDefault();

                    if (existingStaffQualification == null)
                    {
                        staffQualification = staffQualificationLogic.Create(staffQualification);
                    }
                    else
                    {
                        staffQualification.Id = existingStaffQualification.Id;
                        staffQualificationLogic.Modify(staffQualification);
                    }
                }

                result.Id = staffId.ToString();
                result.IsError = false;
                result.Message = "Operation Successful!";
            }
            catch (Exception ex)
            {
                result.IsError = true;
                result.Message = "Error! " + ex.Message;
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public virtual ActionResult UploadFile(FormCollection form)
        {
            HttpPostedFileBase file = Request.Files["MyFile"];

            bool isUploaded = false;
            //string passportUrl = form["Staff.SignatureFileUrl"].ToString();

            StaffLogic staffLogic = new StaffLogic();
            string personId = staffLogic.GetModelBy(p => p.USER.User_Name == User.Identity.Name).Person.Id.ToString();
            string message = "File upload failed";

            string path = null;
            string imageUrl = null;
            string imageUrlDisplay = null;

            try
            {
                if (file != null && file.ContentLength != 0)
                {
                    FileInfo fileInfo = new FileInfo(file.FileName);
                    string fileExtension = fileInfo.Extension;
                    string newFile = personId + "__";
                    string newFileName = newFile + DateTime.Now.ToString().Replace("/", "").Replace(":", "").Replace(" ", "") + fileExtension;

                    string invalidFileMessage = InvalidFile(file.ContentLength, fileExtension);
                    if (!string.IsNullOrEmpty(invalidFileMessage))
                    {
                        isUploaded = false;
                        TempData["imageUrl"] = null;
                        string passportUrl = newFileName;
                        return Json(new { isUploaded = isUploaded, message = invalidFileMessage, imageUrl = passportUrl }, "text/html", JsonRequestBehavior.AllowGet);
                    }

                    string pathForSaving = Server.MapPath("~/Content/Junk");
                    if (this.CreateFolderIfNeeded(pathForSaving))
                    {
                        DeleteFileIfExist(pathForSaving, personId);

                        file.SaveAs(Path.Combine(pathForSaving, newFileName));

                        isUploaded = true;
                        message = "File uploaded successfully!";

                        path = Path.Combine(pathForSaving, newFileName);
                        if (path != null)
                        {
                            imageUrl = "/Content/Junk/" + newFileName;
                            imageUrlDisplay = appRoot + imageUrl + "?t=" + DateTime.Now;

                            TempData["imageUrl"] = imageUrl;

                            PersonLogic personLogic = new PersonLogic();
                            long personID = Convert.ToInt64(personId);
                            Person person = personLogic.GetModelBy(p => p.Person_Id == personID);
                            person.SignatureFileUrl = imageUrl;
                            personLogic.Modify(person);

                            //imageUrlDisplay = imageUrl + "?t=" + DateTime.Now;

                            //imageUrl = "/Content/Junk/" + newFileName;
                            ////imageUrlDisplay = "/Nekedepoly" + imageUrl + "?t=" + DateTime.Now;
                            //TempData["imageUrl"] = imageUrl;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                message = string.Format("File upload failed: {0}", ex.Message);
            }

            return Json(new { isUploaded = isUploaded, message = message, imageUrl = imageUrlDisplay }, "text/html", JsonRequestBehavior.AllowGet);
        }

        private string InvalidFile(decimal uploadedFileSize, string fileExtension)
        {
            try
            {
                string message = null;
                decimal oneKiloByte = 1024;
                decimal maximumFileSize = 50 * oneKiloByte;

                decimal actualFileSizeToUpload = Math.Round(uploadedFileSize / oneKiloByte, 1);
                if (InvalidFileType(fileExtension))
                {
                    message = "File type '" + fileExtension + "' is invalid! File type must be any of the following: .jpg, .jpeg, .png or .jif ";
                }
                else if (actualFileSizeToUpload > (maximumFileSize / oneKiloByte))
                {
                    message = "Your file size of " + actualFileSizeToUpload.ToString("0.#") + " Kb is too large, maximum allowed size is " + (maximumFileSize / oneKiloByte) + " Kb";
                }

                return message;
            }
            catch (Exception)
            {
                throw;
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
                        /*TODO: You must process this exception.*/
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
        private bool InvalidFileType(string extension)
        {
            extension = extension.ToLower();
            switch (extension)
            {
                case ".jpg":
                    return false;
                case ".png":
                    return false;
                case ".gif":
                    return false;
                case ".jpeg":
                    return false;
                default:
                    return true;
            }
        }
        private void DeleteFileIfExist(string folderPath, string fileName)
        {
            try
            {
                string wildCard = fileName + "*.*";
                IEnumerable<string> files = Directory.EnumerateFiles(folderPath, wildCard, SearchOption.TopDirectoryOnly);

                if (files != null && files.Count() > 0)
                {
                    foreach (string file in files)
                    {
                        System.IO.File.Delete(file);
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
        }


        public ActionResult Report()
        {
            StaffDataViewModel  viewModel = new StaffDataViewModel();
            ViewBag.Department = viewModel.DepartmentSelectList;

            return View(viewModel);
        }

        [HttpPost]
        public ActionResult Report(StaffDataViewModel viewModel)
        {
            try
            {
                if (viewModel != null && viewModel.Staff.Department != null && viewModel.Staff.Department.Id > 0)
                {
                    StaffLogic staffLogic = new StaffLogic();
                    EmployeeDetailLogic employeeDetailLogic = new EmployeeDetailLogic();
                    var getStaffList = employeeDetailLogic.GetModelsBy(s => s.Department_Id == viewModel.Staff.Department.Id && s.Staff_Id > 0);

                    if (getStaffList != null && getStaffList.Count > 0)
                    {

                        viewModel.StaffList = getStaffList;
                    }
                    else
                    {
                        SetMessage("No Staff in selected department, make sure the staff in this department has setup their profiles ", Message.Category.Error);

                    }
                }
                else
                {
                    SetMessage("Please select a department before submitting ", Message.Category.Error);

                }
            }
            catch (Exception ex)
            {
                SetMessage("Error! " + ex.Message, Message.Category.Error);
            }
            ViewBag.Department = viewModel.DepartmentSelectList;
            return View(viewModel);

        }
    }
}