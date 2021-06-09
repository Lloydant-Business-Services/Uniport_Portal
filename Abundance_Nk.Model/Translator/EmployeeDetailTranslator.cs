using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Abundance_Nk.Model.Entity;
using Abundance_Nk.Model.Model;

namespace Abundance_Nk.Model.Translator
{
    public class EmployeeDetailTranslator : TranslatorBase<EmployeeDetail, EMPLOYEE_DETAIL>
    {
        private DepartmentTranslator departmentTranslator;
        private DesignationTranslator designationTranslator;
        private GradeLevelTranslator gradeLevelTranslator;
        private UnitTranslator unitTranslator;
        private PersonTranslator personTranslator;

        public EmployeeDetailTranslator()
        {
            departmentTranslator = new DepartmentTranslator();
            designationTranslator = new DesignationTranslator();
            gradeLevelTranslator = new GradeLevelTranslator();
            unitTranslator = new UnitTranslator();
            personTranslator = new PersonTranslator();
        }
        public override EmployeeDetail TranslateToModel(EMPLOYEE_DETAIL entity)
        {
            try
            {
                EmployeeDetail model = null;
                if (entity != null)
                {
                    model = new EmployeeDetail();
                    model.Department = departmentTranslator.Translate(entity.DEPARTMENT);
                    model.Designation = designationTranslator.Translate(entity.DESIGNATION);
                    model.Person = personTranslator.Translate(entity.STAFF.PERSON);
                    model.GradeLevel = gradeLevelTranslator.Translate(entity.GRADE_LEVEL);
                    model.Id = entity.Staff_Id;
                    model.StaffNumber = entity.Staff_Number;
                    model.UnitId = entity.Unit_Id;
                    model.YearOfEmployment = entity.Year_Of_Employment;
                    model.DateOfPresentAppointment = entity.Date_Of_Present_Appointment;
                    model.DateOfPreviousApointment = entity.Date_Of_Previous_Apointment;
                    model.DateOfRetirement = entity.Date_Of_Retirement;
                    model.EmploymentLocation = entity.Employment_Location;

                }

                return model;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public override EMPLOYEE_DETAIL TranslateToEntity(EmployeeDetail model)
        {
            try
            {
                EMPLOYEE_DETAIL entity = null;
                if (model != null)
                {
                    entity = new EMPLOYEE_DETAIL();
                    entity.Staff_Number = model.StaffNumber;
                    entity.Year_Of_Employment = model.YearOfEmployment;
                    if (model.Department != null)
                    {
                        entity.Department_Id = model.Department.Id;
                    }
                    if (model.Designation != null)
                    {
                        entity.Designation_Id = model.Designation.Id;
                    }
                    if (model.GradeLevel != null)
                    {
                        entity.Grade_Level_Id = model.GradeLevel.Id;
                    }
                    
                    if (model.Unit != null)
                    {
                        entity.Unit_Id = model.Unit.Id;
                    }
                    entity.Staff_Id = model.Id;
                    entity.Date_Of_Present_Appointment = model.DateOfPresentAppointment;
                    entity.Date_Of_Previous_Apointment = model.DateOfPreviousApointment;
                    entity.Date_Of_Retirement = model.DateOfRetirement;
                    entity.Employment_Location = model.EmploymentLocation;
                }

                return entity;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
