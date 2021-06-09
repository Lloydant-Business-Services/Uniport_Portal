﻿using Abundance_Nk.Model.Entity;
using Abundance_Nk.Model.Model;
using Abundance_Nk.Model.Translator;

namespace Abundance_Nk.Business
{
    public class ModeOfEntryLogic : BusinessBaseLogic<ModeOfEntry, MODE_OF_ENTRY>
    {
        public ModeOfEntryLogic()
        {
            translator = new ModeOfEntryTranslator();
        }
    }
}