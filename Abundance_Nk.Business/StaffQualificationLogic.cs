using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Abundance_Nk.Model.Entity;
using Abundance_Nk.Model.Model;
using Abundance_Nk.Model.Translator;

namespace Abundance_Nk.Business
{
    public class StaffQualificationLogic : BusinessBaseLogic<StaffQualification, STAFF_QUALIFICATION>
    {
        public StaffQualificationLogic()
        {
            translator = new StaffQualificationTranslator();
        }

        public bool Modify(StaffQualification model)
        {
            try
            {
                STAFF_QUALIFICATION entity = GetEntityBy(s => s.Staff_Id == model.Id);
                if (entity != null)
                {
                    entity.Institution_Attended = model.InstitutionAttended;
                    
                    if (model.EducationalQualification != null && model.EducationalQualification.Id > 0)
                    {
                        entity.Educational_Qualification_Id = model.EducationalQualification.Id;
                    }

                    if (model.StaffResultGrade != null && model.StaffResultGrade.Id > 0)
                    {
                        entity.Staff_Result_Grade_Id = model.StaffResultGrade.Id;
                    }

                    entity.FromDate = model.FromDate;
                    entity.ToDate = model.ToDate;
                    entity.Certificate_Number = model.CertificateNumber;

                    int modified = Save();
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
