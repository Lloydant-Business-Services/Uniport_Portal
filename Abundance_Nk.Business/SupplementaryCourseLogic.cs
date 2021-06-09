using System;
using System.Linq.Expressions;
using Abundance_Nk.Model.Entity;
using Abundance_Nk.Model.Model;
using Abundance_Nk.Model.Translator;

namespace Abundance_Nk.Business
{
    public class SupplementaryCourseLogic : BusinessBaseLogic<SupplementaryCourse, APPLICANT_SUPPLEMENTARY_COURSE>
    {
        public SupplementaryCourseLogic()
        {
            translator = new SupplementaryCourseTranslator();
        }

        public SupplementaryCourse GetBy(Person person)
        {
            try
            {
                Expression<Func<APPLICANT_SUPPLEMENTARY_COURSE, bool>> selector = s => s.Person_Id == person.Id;
                return GetModelBy(selector);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public bool Modify(SupplementaryCourse supplementaryCourse)
        {
            try
            {
                APPLICANT_SUPPLEMENTARY_COURSE entity = GetEntityBy(supplementaryCourse.Person);

                if (entity == null)
                {
                    throw new Exception(NoItemFound);
                }

                entity.Department_Id = supplementaryCourse.Department.Id;
                entity.Avergae_Score = supplementaryCourse.Average_Score;
                entity.PUTME_Score = supplementaryCourse.Putme_Score;

                if (supplementaryCourse.ApplicationForm != null && supplementaryCourse.ApplicationForm.Id > 0)
                {
                    entity.Application_From_Id = supplementaryCourse.ApplicationForm.Id;
                }

                if (supplementaryCourse.Option != null && supplementaryCourse.Option.Id > 0)
                {
                    entity.Department_Option_Id = supplementaryCourse.Option.Id;
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

        private APPLICANT_SUPPLEMENTARY_COURSE GetEntityBy(Person person)
        {
            try
            {
                Expression<Func<APPLICANT_SUPPLEMENTARY_COURSE, bool>> selector = s => s.Person_Id == person.Id;
                APPLICANT_SUPPLEMENTARY_COURSE entity = GetEntityBy(selector);

                return entity;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}