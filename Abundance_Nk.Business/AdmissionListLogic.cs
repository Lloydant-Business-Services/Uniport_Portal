using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Abundance_Nk.Model.Entity;
using Abundance_Nk.Model.Model;
using Abundance_Nk.Model.Translator;
using System.Threading.Tasks;

namespace Abundance_Nk.Business
{
    public class AdmissionListLogic : BusinessBaseLogic<AdmissionList, ADMISSION_LIST>
    {
        public AdmissionListLogic()
        {
            translator = new AdmissionListTranslator();
        }

        public AdmissionList GetBy(string jambNumber)
        {
            try
            {
                AdmissionList admissionList = null;
                if (jambNumber.Length == 10)
                {
                    admissionList =(from a in repository.GetBy<VW_ADMISSION_LIST_UNDERGRADUATE>( a => a.Applicant_Jamb_Registration_Number == jambNumber)
                                                    select new AdmissionList
                                                    {
                                                        Id = a.Admission_List_Id,
                                                        Form = new ApplicationForm {Id = a.Application_Form_Id, Number = a.Application_Form_Number},
                                                        Deprtment = new Department {Id = a.Department_Id, Name = a.Department_Name},
                                                    }).FirstOrDefault();
                    if (admissionList != null && admissionList.Form.Id > 0)
                    {
                        admissionList = GetBy(admissionList.Form.Id);
                        return admissionList;
                    }

                }
                else
                {
                    admissionList = (from a in repository.GetBy<ADMISSION_LIST>( a => a.APPLICATION_FORM.Application_Form_Number == jambNumber)
                                                select new AdmissionList
                                                {
                                                    Id = a.Admission_List_Id,
                                                    Form = new ApplicationForm {Id = a.Application_Form_Id, Number = a.APPLICATION_FORM.Application_Form_Number},
                                                    Deprtment = new Department {Id = a.Department_Id, Name = a.DEPARTMENT.Department_Name},
                         
                                                }).FirstOrDefault();
                    if (admissionList != null)
                    {
                        admissionList = GetBy(admissionList.Form.Id);
                        return admissionList;
                    }
                    
                }
                return null;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public bool IsAdmittedByJamb(string jambNumber)
        {
            try
            {
                AdmissionList admissionList =
                    (from a in
                        repository.GetBy<VW_ADMISSION_LIST_UNDERGRADUATE>(
                            a => a.Applicant_Jamb_Registration_Number == jambNumber)
                        select new AdmissionList
                        {
                            Id = a.Admission_List_Id,
                            Form = new ApplicationForm {Id = a.Application_Form_Id, Number = a.Application_Form_Number},
                            Deprtment = new Department {Id = a.Department_Id, Name = a.Department_Name},
                         
                        }).FirstOrDefault();
                if (admissionList != null && admissionList.Form.Id > 0)
                {
                    return true;
                }
            }
            catch (Exception)
            {
                throw;
            }
            return false;
        }
        public AdmissionList GetBy(long applicationFormId)
        {
            try
            {
                Expression<Func<ADMISSION_LIST, bool>> selector =a => a.Application_Form_Id == applicationFormId && a.Activated == true;
                AdmissionList admission = GetModelsBy(selector).FirstOrDefault();
                return admission;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public AdmissionList GetBy(Person person)
        {
            try
            {
                Expression<Func<ADMISSION_LIST, bool>> selector = a => a.APPLICATION_FORM.Person_Id == person.Id;
                return GetModelsBy(selector).FirstOrDefault();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<AdmissionList> GetByAsync(Person person)
        {
            try
            {
                Expression<Func<ADMISSION_LIST, bool>> selector = a => a.APPLICATION_FORM.Person_Id == person.Id;
                return await GetModelsByFODAsync(selector);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public bool IsAdmitted(ApplicationForm applicationForm)
        {
            try
            {
                Expression<Func<ADMISSION_LIST, bool>> selector = a => a.Application_Form_Id == applicationForm.Id && a.Activated == true;
                AdmissionList admissionList = GetModelBy(selector);
                if (admissionList != null && admissionList.Id > 0)
                {
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public bool IsAdmitted(string ApplicationNumber)
        {
            try
            {
                Expression<Func<ADMISSION_LIST, bool>> selector =a => a.APPLICATION_FORM.Application_Form_Number == ApplicationNumber && a.Activated == true;
                AdmissionList admissionList = GetModelBy(selector);
                if (admissionList != null && admissionList.Id > 0)
                {
                    return true;
                }

                return false;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public AdmissionList Create(AdmissionList admissionlist, AdmissionListBatch batch, AdmissionListAudit Audit)
        {
            var List = new AdmissionList();
            try
            {
                var auditLogic = new AdmissionListAuditLogic();
                admissionlist.Batch = batch;
                if (!IsAdmitted(admissionlist.Form))
                {
                    List = base.Create(admissionlist);
                    Audit.AdmissionList = admissionlist;
                    Audit.AdmissionList.Id = List.Id;
                    Audit.Form = admissionlist.Form;
                    Audit.Deprtment = admissionlist.Deprtment;
                    Audit.DepartmentOption = admissionlist.DepartmentOption;
                    auditLogic.Create(Audit);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return List;
        }
        public bool IsValidApplicationNumberAndPin(string applicationNumber, string pin)
        {
            try
            {
                Expression<Func<APPLICATION_FORM, bool>> selector =
                    af => af.Application_Form_Number == applicationNumber;
                var applicationForm = new ApplicationForm();
                var formLogic = new ApplicationFormLogic();
                applicationForm = formLogic.GetModelBy(selector);
                if (applicationForm == null)
                {
                    return false;
                }

                if (!IsAdmitted(applicationForm))
                {
                    return false;
                }
                FeeType feetype;
                if (applicationForm.ProgrammeFee.Programme.Id == 1)
                {
                    feetype = new FeeType {Id = 7};
                }
                else
                {
                    feetype = new FeeType {Id = 8};
                }
                var paymentLogic = new ScratchCardLogic();
                ;
                if (!paymentLogic.ValidatePin(pin, feetype))
                {
                    return false;
                }
                bool pinUseStatus = paymentLogic.IsPinUsed(pin, applicationForm.Person.Id);
                if (!pinUseStatus)
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
        public bool HasStudentCheckedStatus(long fid)
        {
            try
            {
                var appForm = new ApplicationForm();
                var appFormLogic = new ApplicationFormLogic();
                appForm = appFormLogic.GetModelBy(a => a.Application_Form_Id == fid);
                if (appForm != null)
                {
                    var paymentLogic = new ScratchCardLogic();
                    var payment = new ScratchCard();
                    payment = paymentLogic.GetModelsBy(s => s.Person_Id == appForm.Person.Id).FirstOrDefault();
                    if (payment != null)
                    {
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return false;
        }
        public bool Update(AdmissionList admissionlist, AdmissionListAudit Audit)
        {
            try
            {
                var auditLogic = new AdmissionListAuditLogic();
                if (IsAdmitted(admissionlist.Form))
                {
                    Expression<Func<ADMISSION_LIST, bool>> selector = a => a.Admission_List_Id == admissionlist.Id;
                    ADMISSION_LIST List = GetEntityBy(selector);
                    if (List != null && List.Admission_List_Id > 0)
                    {
                        List.Department_Id = admissionlist.Deprtment.Id;
                        List.Programme_Id = admissionlist.Programme.Id;
                        
                        List.Activated = admissionlist.Activated;
                        if (admissionlist.DepartmentOption != null && admissionlist.DepartmentOption.Id > 0)
                        {
                            List.Department_Option_Id = admissionlist.DepartmentOption.Id;
                        }
                        if (admissionlist.Activated != List.Activated)
                        {
                            Audit.Action = "Activated toggle" + Audit.Action;
                        }
                        int modifiedRecordCount = Save();
                        if (modifiedRecordCount > 0)
                        {
                            Audit.AdmissionList = admissionlist;
                            Audit.AdmissionList.Id = List.Admission_List_Id;
                            Audit.Form = admissionlist.Form;
                            Audit.Deprtment = admissionlist.Deprtment;
                            Audit.DepartmentOption = admissionlist.DepartmentOption;
                            Audit.Time = DateTime.Now;
                            auditLogic.Create(Audit);
                            return true;
                        }
                    }

                    return false;
                }
            }
            catch (Exception e)
            {
                throw;
            }

            return false;
        }
        public List<AdmissionList> GetByAdmissionLists(Department department, Programme programme, Session session,AdmissionListType admissionListType)
        {
            try
            {
                var applicantJambDetailLogic = new ApplicantJambDetailLogic();

                var admissionLists = new List<AdmissionList>();
                List<AdmissionList> firstAdmissionLists =
                    GetModelsBy(
                        a =>
                            a.Department_Id == department.Id &&
                            a.Programme_Id == programme.Id &&
                            a.APPLICATION_FORM.APPLICATION_FORM_SETTING.SESSION.Session_Id == session.Id &&
                            a.ADMISSION_LIST_BATCH.ADMISSION_LIST_TYPE.Admission_List_Type_Id == admissionListType.Id);
                if (firstAdmissionLists != null && firstAdmissionLists.Count > 0)
                {
                    foreach (AdmissionList firstAdmissionList in firstAdmissionLists)
                    {
                        ApplicantJambDetail applicantJambDetail = applicantJambDetailLogic.GetBy(firstAdmissionList.Form);
                        firstAdmissionList.JambNumber = applicantJambDetail.JambRegistrationNumber;
                        admissionLists.Add(firstAdmissionList);
                    }
                }
                return admissionLists;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public List<AdmissionList> GetByAdmissionLists(Department department, Programme programme, Session session)
        {
            try
            {
                List<AdmissionList> firstAdmissionLists =
                    GetModelsBy(
                        a =>
                            a.Department_Id == department.Id &&
                            a.Programme_Id == programme.Id &&
                            a.APPLICATION_FORM.APPLICATION_FORM_SETTING.SESSION.Session_Id == session.Id);

                return firstAdmissionLists;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public List<AdmittedStudentBreakdownView> GetAdmittedBreakDownBy(Session session)
        {
            try
            {
                List<AdmittedStudentBreakdownView> breakdownViews =
               (from x in repository.GetBy<VW_ADMITTED_STUDENT_BREAKDOWN>
                    (x => x.Session_Id == session.Id)
                select new AdmittedStudentBreakdownView
                {
                    AdmissionListId = x.Admission_List_Id,
                    ProgrammeId = x.Programme_Id,
                    ProgrammeName = x.Programme_Name,
                    DepartmentId = x.Department_Id,
                    DepartmentName = x.Department_Name,
                    SessionId = x.Session_Id,
                    
                }).OrderBy(c => c.DepartmentName).ToList();

                return breakdownViews;
            }
            catch (Exception ex)
            {
                throw ex;
            }


        }
        public List<AdmissionListModel> GetAdmissionListBy(Session session, Programme programme, Department department, DateTime dateFrom, DateTime dateTo)
        {
            List<AdmissionListModel> admissionList = new List<AdmissionListModel>();
            try
            {
                admissionList = (from sr in repository.GetBy<VW_ADMISSION_LIST_ALL>(a => a.Session_Id == session.Id && a.Programme_Id == programme.Id && a.Department_Id == department.Id && a.Date_Uploaded >= dateFrom && a.Date_Uploaded <= dateTo)
                                 select new AdmissionListModel
                                 {
                                     PersonId = sr.Person_Id,
                                     FullName = sr.Full_Name,
                                     ExamNumber = sr.Application_Exam_Number,
                                     ApplicationNumber = sr.Application_Form_Number,
                                     ProgrammeId = sr.Programme_Id,
                                     Programme = sr.Programme_Name,
                                     DepartmentId = sr.Department_Id,
                                     Department = sr.Department_Name,
                                     AdmissionListType = sr.Admission_List_Type_Name,
                                     DateUploaded = sr.Date_Uploaded,
                                     JambNumber = sr.Applicant_Jamb_Registration_Number,
                                     SessionName = sr.Session_Name,
                                     SessionId = sr.Session_Id
                                 }).ToList();



            }
            catch (Exception)
            {
                throw;
            }

            return admissionList.OrderBy(a => a.ApplicationNumber).ToList();
        }

        public List<AdmissionListModel> GetAdmissionListBulk(Session session, DateTime dateFrom, DateTime dateTo)
        {
            List<AdmissionListModel> admissionList = new List<AdmissionListModel>();
            try
            {
                admissionList = (from sr in repository.GetBy<VW_ADMISSION_LIST_ALL>(a => a.Session_Id == session.Id && a.Date_Uploaded >= dateFrom && a.Date_Uploaded <= dateTo)
                                 select new AdmissionListModel
                                 {
                                     PersonId = sr.Person_Id,
                                     FullName = sr.Full_Name,
                                     ExamNumber = sr.Application_Exam_Number,
                                     ApplicationNumber = sr.Application_Form_Number,
                                     ProgrammeId = sr.Programme_Id,
                                     Programme = sr.Programme_Name,
                                     DepartmentId = sr.Department_Id,
                                     Department = sr.Department_Name,
                                     AdmissionListType = sr.Admission_List_Type_Name,
                                     DateUploaded = sr.Date_Uploaded,
                                     JambNumber = sr.Applicant_Jamb_Registration_Number,
                                     SessionName = sr.Session_Name,
                                     SessionId = sr.Session_Id
                                 }).ToList();



            }
            catch (Exception)
            {
                throw;
            }

            return admissionList.OrderBy(a => a.ApplicationNumber).ToList();
        }
    }
}