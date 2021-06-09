using System;
using System.Linq.Expressions;
using Abundance_Nk.Model.Entity;
using Abundance_Nk.Model.Model;
using Abundance_Nk.Model.Translator;

namespace Abundance_Nk.Business
{
    public class RemitaSettingsLogic : BusinessBaseLogic<RemitaSettings, REMITA_PAYMENT_SETTINGS>
    {
        public RemitaSettingsLogic()
        {
            translator = new RemitaSettingsTranslator();
        }


        public RemitaSettings GetBy(long settingsId)
        {
            try
            {
                Expression<Func<REMITA_PAYMENT_SETTINGS, bool>> selector = a => a.Payment_SettingId == settingsId;
                RemitaSettings remitaSettings = GetModelBy(selector);
                return remitaSettings;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}