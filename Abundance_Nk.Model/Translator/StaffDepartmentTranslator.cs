using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Abundance_Nk.Model.Entity;
using Abundance_Nk.Model.Model;

namespace Abundance_Nk.Model.Translator
{
    public class StaffDepartmentTranslator : TranslatorBase<StaffDepartment, STAFF_DEPARTMENT>
    {
        private StaffTranslator staffTranslator;
        private DepartmentTranslator departmentTranslator;
        private UserTranslator userTranslator;
        private DepartmentOptionTranslator departmentOptionTranslator;

        public StaffDepartmentTranslator()
        {
            departmentTranslator = new DepartmentTranslator();
            staffTranslator = new StaffTranslator();
            userTranslator = new UserTranslator();
            departmentOptionTranslator = new DepartmentOptionTranslator();

        }
        public override StaffDepartment TranslateToModel(STAFF_DEPARTMENT entity)
        {
            try
            {
                StaffDepartment model = null;
                if (entity != null)
                {
                    model = new StaffDepartment();
                    model.StaffUserID = entity.Staff_Id;
                    model.DateEntered = entity.Date_Entered;
                    model.Department = departmentTranslator.Translate(entity.DEPARTMENT);
                    model.Id = entity.Staff_Department_Id;
                    model.IsHead = entity.IsHead;
                    model.DepartmentOption = departmentOptionTranslator.Translate(entity.DEPARTMENT_OPTION);
                  
                }

                return model;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public override STAFF_DEPARTMENT TranslateToEntity(StaffDepartment model)
        {
            try
            {
                STAFF_DEPARTMENT entity = null;
                if (model != null)
                {
                    entity = new STAFF_DEPARTMENT();
                    entity.Date_Entered = model.DateEntered;
                    entity.IsHead = model.IsHead;
                  
                    entity.Staff_Department_Id = model.Id;
                    entity.Staff_Id = model.StaffUserID;
                    
                    if (model.Department != null)
                    {
                        entity.Department_Id = model.Department.Id;
                    }
                    if (model.DepartmentOption != null)
                    {
                        entity.Department_Option_Id = model.DepartmentOption.Id;
                    }
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
