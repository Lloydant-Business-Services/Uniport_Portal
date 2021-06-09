﻿using Abundance_Nk.Model.Entity;
using Abundance_Nk.Model.Model;
using Abundance_Nk.Model.Translator;

namespace Abundance_Nk.Business
{
    public class AdmissionListBatchLogic : BusinessBaseLogic<AdmissionListBatch, ADMISSION_LIST_BATCH>
    {
        public AdmissionListBatchLogic()
        {
            translator = new AdmissionListBatchTranslator();
        }
    }
}