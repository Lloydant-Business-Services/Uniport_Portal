using System;
using Abundance_Nk.Model.Entity;
using Abundance_Nk.Model.Model;

namespace Abundance_Nk.Model.Translator
{
    public class StudentTranslator : TranslatorBase<Student, STUDENT>
    {
        private readonly ApplicationFormTranslator applicationFormTranslator;
        private readonly BloodGroupTranslator bloodGroupTranslator;
        private readonly NationalityTranslator countryTranslator = new NationalityTranslator();
        private readonly GenotypeTranslator genotypeTranslator;
        private readonly LocalGovernmentTranslator localGovernmentTranslator = new LocalGovernmentTranslator();
        private readonly MaritalStatusTranslator maritalStatusTranslator;
        private readonly SexTranslator sexTranslator = new SexTranslator();
        private readonly StateTranslator stateTranslator = new StateTranslator();
        private readonly StudentCategoryTranslator studentCategoryTranslator;
        private readonly StudentStatusTranslator studentStatusTranslator;
        private readonly StudentTypeTranslator studentTypeTranslator;
        private readonly TitleTranslator titleTranslator;
        private PersonTranslator personTranslator;

        public StudentTranslator()
        {
            titleTranslator = new TitleTranslator();
            studentTypeTranslator = new StudentTypeTranslator();
            studentCategoryTranslator = new StudentCategoryTranslator();
            applicationFormTranslator = new ApplicationFormTranslator();
            studentStatusTranslator = new StudentStatusTranslator();
            maritalStatusTranslator = new MaritalStatusTranslator();
            bloodGroupTranslator = new BloodGroupTranslator();
            genotypeTranslator = new GenotypeTranslator();
            personTranslator = new PersonTranslator();
        }

        public override Student TranslateToModel(STUDENT studentEntity)
        {
            try
            {
                Student student = null;
                if (studentEntity != null)
                {
                    student = new Student();
                    student.Id = studentEntity.Person_Id;
                    student.ApplicationForm = applicationFormTranslator.Translate(studentEntity.APPLICATION_FORM);
                    student.Type = studentTypeTranslator.TranslateToModel(studentEntity.STUDENT_TYPE);
                    student.Category = studentCategoryTranslator.TranslateToModel(studentEntity.STUDENT_CATEGORY);
                    student.Status = studentStatusTranslator.TranslateToModel(studentEntity.STUDENT_STATUS);
                    student.Number = studentEntity.Student_Number;
                    student.MatricNumber = studentEntity.Matric_Number;
                    student.Title = titleTranslator.Translate(studentEntity.TITLE);
                    student.MaritalStatus = maritalStatusTranslator.Translate(studentEntity.MARITAL_STATUS);
                    student.BloodGroup = bloodGroupTranslator.Translate(studentEntity.BLOOD_GROUP);
                    student.Genotype = genotypeTranslator.Translate(studentEntity.GENOTYPE);
                    student.SchoolContactAddress = studentEntity.School_Contact_Address;
                    student.Activated = studentEntity.Activated;
                    student.Reason = studentEntity.Reason;
                    student.RejectCategory = studentEntity.Reject_Category;
                    student.PasswordHash = studentEntity.Password_hash;

                    if (studentEntity.PERSON != null)
                    {
                        student.FirstName = studentEntity.PERSON.First_Name;
                        student.LastName = studentEntity.PERSON.Last_Name;
                        student.OtherName = studentEntity.PERSON.Other_Name;
                        student.ImageFileUrl = studentEntity.PERSON.Image_File_Url;
                        student.MobilePhone = studentEntity.PERSON.Mobile_Phone;
                        student.LocalGovernment =
                            localGovernmentTranslator.Translate(studentEntity.PERSON.LOCAL_GOVERNMENT);
                        student.State = stateTranslator.Translate(studentEntity.PERSON.STATE);
                        student.HomeAddress = studentEntity.PERSON.Home_Address;
                        student.HomeTown = studentEntity.PERSON.Home_Town;
                        student.Nationality = countryTranslator.Translate(studentEntity.PERSON.NATIONALITY);
                        student.Sex = sexTranslator.Translate(studentEntity.PERSON.SEX);
                    }
                }

                return student;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public override STUDENT TranslateToEntity(Student student)
        {
            try
            {
                STUDENT studentEntity = null;
                if (student != null)
                {
                    studentEntity = new STUDENT();
                    studentEntity.Person_Id = student.Id;

                    studentEntity.Student_Number = student.Number;
                    studentEntity.Matric_Number = student.MatricNumber;
                    studentEntity.School_Contact_Address = student.SchoolContactAddress;
                    studentEntity.Activated = student.Activated;
                    studentEntity.Reason = student.Reason;
                    studentEntity.Reject_Category = student.RejectCategory;
                    if (student.Type != null && student.Type.Id > 0)
                    {
                        studentEntity.Student_Type_Id = student.Type.Id;
                    }
                    if (student.Category != null && student.Category.Id > 0)
                    {
                        studentEntity.Student_Category_Id = student.Category.Id;
                    }
                    if (student.Status != null && student.Status.Id > 0)
                    {
                        studentEntity.Student_Status_Id = student.Status.Id;
                    }

                    if (student.PasswordHash != null)
                    {
                        studentEntity.Password_hash = student.PasswordHash;
                    }

                    if (student.Title != null && student.Title.Id > 0)
                    {
                        studentEntity.Title_Id = student.Title.Id;
                    }

                    if (student.MaritalStatus != null && student.MaritalStatus.Id > 0)
                    {
                        studentEntity.Marital_Status_Id = student.MaritalStatus.Id;
                    }

                    if (student.BloodGroup != null && student.BloodGroup.Id > 0)
                    {
                        studentEntity.Blood_Group_Id = student.BloodGroup.Id;
                    }

                    if (student.Genotype != null && student.Genotype.Id > 0)
                    {
                        studentEntity.Genotype_Id = student.Genotype.Id;
                    }

                    if (student.ApplicationForm != null && student.ApplicationForm.Id > 0)
                    {
                        studentEntity.Application_Form_Id = student.ApplicationForm.Id;
                    }
                }

                return studentEntity;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}