using System;
using Abundance_Nk.Model.Entity;
using Abundance_Nk.Model.Model;

namespace Abundance_Nk.Model.Translator
{
    public class StaffTranslator : TranslatorBase<Staff, STAFF>
    {
        private readonly PersonTranslator personTranslator;
        private readonly UserTranslator userTranslator;
        private readonly DepartmentTranslator departmentTranslator;
        private readonly StaffTypeTranslator staffTypeTranslator;
        private readonly BloodGroupTranslator bloodGroupTranslator;
        private readonly GenotypeTranslator genotypeTranslator;
        private readonly MaritalStatusTranslator maritalStatusTranslator;
        private readonly DepartmentOptionTranslator departmentOptionTranslator;

        public StaffTranslator()
        {
            staffTypeTranslator = new StaffTypeTranslator();
            personTranslator = new PersonTranslator();
            userTranslator = new UserTranslator();
            departmentTranslator = new DepartmentTranslator();
            staffTypeTranslator = new StaffTypeTranslator();
            bloodGroupTranslator = new BloodGroupTranslator();
            genotypeTranslator = new GenotypeTranslator();
            maritalStatusTranslator = new MaritalStatusTranslator();
            departmentOptionTranslator = new DepartmentOptionTranslator();
        }

        public override Staff TranslateToModel(STAFF entity)
        {
            try
            {
                Staff model = null;
                if (entity != null)
                {
                    model = new Staff();
                    model.Id = entity.Staff_Id;
                    model.Type = staffTypeTranslator.Translate(entity.STAFF_TYPE);
                    model.Person = personTranslator.Translate(entity.PERSON);
                    model.User = userTranslator.Translate(entity.USER);
                    model.Department = departmentTranslator.Translate(entity.DEPARTMENT);
                    model.isHead = entity.IsHead;
                    model.isManagement = (bool)entity.Is_Management;
                    model.ProfileDescription = entity.Profile_Description;

                    model.BloodGroup = bloodGroupTranslator.Translate(entity.BLOOD_GROUP);
                    model.Genotype = genotypeTranslator.Translate(entity.GENOTYPE);
                    model.MaritalStatus = maritalStatusTranslator.Translate(entity.MARITAL_STATUS);
                    model.DepartmentOption = departmentOptionTranslator.Translate(entity.DEPARTMENT_OPTION);
                }

                return model;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public override STAFF TranslateToEntity(Staff model)
        {
            try
            {
                STAFF entity = null;
                if (model != null)
                {
                    entity = new STAFF();
                    entity.Staff_Id = model.Id;
                    entity.Staff_Type_Id = model.Type.Id;
                    entity.User_Id = model.User.Id;
                    entity.Department_Id = model.Department.Id;
                    entity.Profile_Description = model.ProfileDescription;
                    entity.IsHead = model.isHead;
                    entity.Is_Management = model.isManagement;

                    if (model.Person != null)
                    {
                        entity.Person_Id = model.Person.Id;
                    }
                    if (model.MaritalStatus != null)
                    {
                        entity.Marital_Status_Id = model.MaritalStatus.Id;
                    }
                    if (model.BloodGroup != null)
                    {
                        entity.Blood_Group_Id = model.BloodGroup.Id;
                    }
                    if (model.Genotype != null)
                    {
                        entity.Genotype_Id = model.Genotype.Id;
                    }
                    if (model.DepartmentOption?.Id > 0)
                    {
                        entity.DepartmentOption_Id = model.DepartmentOption.Id;
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