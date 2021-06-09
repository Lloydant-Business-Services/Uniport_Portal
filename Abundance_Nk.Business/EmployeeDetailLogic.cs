using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Abundance_Nk.Model.Entity;
using Abundance_Nk.Model.Model;
using Abundance_Nk.Model.Translator;

namespace Abundance_Nk.Business
{
    public class EmployeeDetailLogic : BusinessBaseLogic<EmployeeDetail, EMPLOYEE_DETAIL>
    {
        public EmployeeDetailLogic()
        {
            translator = new EmployeeDetailTranslator();
        }

        public bool Modify(EmployeeDetail model)
        {
            try
            {
                Expression<Func<EMPLOYEE_DETAIL, bool>> selector = p => p.Staff_Id == model.Id;
                EMPLOYEE_DETAIL entity = GetEntityBy(selector);

                if (entity == null || entity.Staff_Id <= 0)
                {
                    throw new Exception(NoItemFound);
                }

                if (model.YearOfEmployment > 0)
                {
                    entity.Year_Of_Employment = model.YearOfEmployment;
                }

                if (model.StaffNumber != null)
                {
                    entity.Staff_Number = model.StaffNumber;
                }

                if (model.Designation != null && model.Designation.Id > 0)
                {
                    entity.Designation_Id = model.Designation.Id;
                }

                if (model.GradeLevel != null && model.GradeLevel.Id > 0)
                {
                    entity.Grade_Level_Id = model.GradeLevel.Id;
                }
                if (model.Department != null && model.Department.Id > 0)
                {
                    entity.Department_Id = model.Department.Id;
                }
                if (model.Unit != null && model.Unit.Id > 0)
                {
                    entity.Unit_Id = model.Unit.Id;
                }

                entity.Date_Of_Previous_Apointment = model.DateOfPreviousApointment;
                entity.Date_Of_Present_Appointment = model.DateOfPresentAppointment;
                entity.Date_Of_Retirement = model.DateOfRetirement;

                if (model.EmploymentLocation != null)
                {
                    entity.Employment_Location = model.EmploymentLocation;
                }

                int modifiedRecordCount = Save();
                if (modifiedRecordCount <= 0)
                {
                    return false;
                }
                return true;
            }
            catch (Exception)
            {
                throw;
            }
        }

        //public List<ReportModel> GetFormDetail(long personId)
        //{
        //    List<ReportModel> reportModels = null;
        //    try
        //    {
        //        reportModels = (from a in repository.GetBy<VW_FORM_REPORT>(a => a.Person_Id == personId)
        //                      select new ReportModel
        //                                {
        //                                    Surname = a.Surname,
        //                                    PersonId = a.Person_Id,
        //                                    BloodGroup = a.BloodGroup,
        //                                    Firstname = a.Firstname,
        //                                    Department = a.Department,
        //                                    MiddleName = a.Othername,
        //                                    PassportUrl = a.Passport_Url,
        //                                    MobileNumber = a.Mobile_Number,
        //                                    ResidentialAddress = a.Contact_Address,
        //                                    HomeTown = a.Home_Town,
        //                                    HomePostalAddress = a.Home_Postal_Address,
        //                                    DateOfBirth = a.Date_Of_Birth != null ? a.Date_Of_Birth.Value.ToShortDateString() : "",
        //                                    YearOfEmployment = a.Year_Of_Employment,
        //                                    StaffNumber = a.Staff_Number,
        //                                    DateOfPreviousApointment = a.Date_Of_Previous_Apointment != null ? a.Date_Of_Previous_Apointment.Value.ToShortDateString() : "",
        //                                    DateOfPresentAppointment = a.Date_Of_Present_Appointment != null ? a.Date_Of_Present_Appointment.Value.ToShortDateString() : "",
        //                                    DateOfRetirement = a.Date_Of_Retirement != null ? a.Date_Of_Retirement.Value.ToShortDateString() : "",
        //                                    EmploymentLocation = a.Employment_Location,
        //                                    Designation = a.Designation,
        //                                    InstitutionAttended1 = a.Institution_Attended,
        //                                    FromDate1 = a.FromDate != null ? a.FromDate.Value.ToShortDateString() : "",
        //                                    ToDate1 = a.ToDate != null ? a.ToDate.Value.ToShortDateString() : "",
        //                                    CertificateNumber1 = a.Certificate_Number,
        //                                    EducationalQualification1 = a.Employment_Location,
        //                                    GradeLevel = a.Grade_Level,
        //                                    LocalGovernment = a.Local_Government_Name,
        //                                    MaritalStatus = a.Marital_Status_Name,
        //                                    State = a.State_Name,
        //                                    NameOfSpouse = a.Name_Of_Spouse,
        //                                    NextOfKinName = a.Next_Of_Kin_Name,
        //                                    NextOfKinMobilePhone = a.Next_Of_Kin_Mobile_Phone,
        //                                    NextOfKinPostalAddress = a.Next_Of_Kin_Postal_Address,
        //                                    NumberOfDependentChildren = Convert.ToInt32(a.Number_Of_Dependent_Children),
        //                                    NumberOfDependentRelative = Convert.ToInt32(a.Number_Of_Dependent_Relatives),
        //                                    RelationshipWithNextOfKin = a.Relationship_Name,
        //                                    Religion = a.Religion_Name,
        //                                    Sex = a.State_Name,
        //                                    Title = a.Title_Name,
        //                                    Unit = a.Unit
        //                                }).ToList();
        //    }
        //    catch (Exception)
        //    {
        //        throw;
        //    }

        //    return reportModels;
        //}
    }
}
