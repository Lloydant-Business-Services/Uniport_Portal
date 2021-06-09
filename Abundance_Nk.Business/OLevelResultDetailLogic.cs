using System;
using System.Linq.Expressions;
using Abundance_Nk.Model.Entity;
using Abundance_Nk.Model.Model;
using Abundance_Nk.Model.Translator;

namespace Abundance_Nk.Business
{
    public class OLevelResultDetailLogic : BusinessBaseLogic<OLevelResultDetail, APPLICANT_O_LEVEL_RESULT_DETAIL>
    {
        public OLevelResultDetailLogic()
        {
            translator = new OLevelResultDetailTranslator();
        }

        public bool Modify(OLevelResultDetail oLevelResultDetail)
        {
            try
            {
                Expression<Func<APPLICANT_O_LEVEL_RESULT_DETAIL, bool>> selector =
                    o => o.Applicant_O_Level_Result_Detail_Id == oLevelResultDetail.Id;
                APPLICANT_O_LEVEL_RESULT_DETAIL entity = GetEntityBy(selector);

                if (entity == null)
                {
                    throw new Exception(NoItemFound);
                }

                //entity.Applicant_O_Level_Result_Detail_Id = oLevelResultDetail.Header.Id;
                entity.O_Level_Subject_Id = oLevelResultDetail.Subject.Id;
                entity.O_Level_Grade_Id = oLevelResultDetail.Grade.Id;

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

        public bool DeleteBy(OLevelResult oLevelResult)
        {
            try
            {
                Expression<Func<APPLICANT_O_LEVEL_RESULT_DETAIL, bool>> selector =
                    o => o.Applicant_O_Level_Result_Id == oLevelResult.Id;
                return Delete(selector);
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}