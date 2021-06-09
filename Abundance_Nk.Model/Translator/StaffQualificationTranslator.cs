using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Abundance_Nk.Model.Entity;
using Abundance_Nk.Model.Model;

namespace Abundance_Nk.Model.Translator
{
    public class StaffQualificationTranslator : TranslatorBase<StaffQualification, STAFF_QUALIFICATION>
    {
        private EducationalQualificationTranslator educationalQualificationTranslator;
        private StaffResultGradeTranslator staffResultGradeTranslator;
        private StaffTranslator staffTranslator;

        public StaffQualificationTranslator()
        {
            educationalQualificationTranslator = new EducationalQualificationTranslator();
            staffResultGradeTranslator = new StaffResultGradeTranslator();
            staffTranslator = new StaffTranslator();
        }
        public override StaffQualification TranslateToModel(STAFF_QUALIFICATION entity)
        {
            try
            {
                StaffQualification model = null;
                if (entity != null)
                {
                    model = new StaffQualification();
                    model.Id = entity.Id;
                    model.CertificateNumber = entity.Certificate_Number;
                    model.EducationalQualification = educationalQualificationTranslator.Translate(entity.EDUCATIONAL_QUALIFICATION);
                    model.StaffResultGrade = staffResultGradeTranslator.Translate(entity.STAFF_RESULT_GRADE);
                    model.FromDate = entity.FromDate;
                    model.InstitutionAttended = entity.Institution_Attended;
                    model.ToDate = entity.ToDate;
                    //model.Staff = staffTranslator.Translate(entity.STAFF);
                }

                return model;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public override STAFF_QUALIFICATION TranslateToEntity(StaffQualification model)
        {
            try
            {
                STAFF_QUALIFICATION entity = null;
                if (model != null)
                {
                    entity = new STAFF_QUALIFICATION();
                    entity.Id = model.Id;
                    entity.Certificate_Number = model.CertificateNumber;

                    if (model.EducationalQualification != null)
                    {
                        entity.Educational_Qualification_Id = model.EducationalQualification.Id;
                    }
                    
                    entity.FromDate = model.FromDate;
                    entity.Institution_Attended = model.InstitutionAttended;
                    entity.ToDate = model.ToDate;

                    if (model.StaffResultGrade != null)
                    {
                        entity.Staff_Result_Grade_Id = model.StaffResultGrade.Id;
                    }
                    if (model.Staff != null)
                    {
                        entity.Staff_Id = model.Staff.Id;
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
