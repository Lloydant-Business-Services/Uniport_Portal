using Abundance_Nk.Model.Entity;
using Abundance_Nk.Model.Model;
using Abundance_Nk.Model.Translator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Abundance_Nk.Business
{

    public class ApplicantRefereeResponseLogic : BusinessBaseLogic<ApplicantRefereeResponse, APPLICANT_REFEREE_RESPONSE>
    {
        public ApplicantRefereeResponseLogic()
        {
            translator = new ApplicantRefereeResponseTranslator();
        }

    }
}
