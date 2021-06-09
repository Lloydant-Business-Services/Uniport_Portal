using Abundance_Nk.Model.Entity;
using Abundance_Nk.Model.Model;
using Abundance_Nk.Model.Translator;

namespace Abundance_Nk.Business
{
    public class ModeOfFinanceLogic : BusinessBaseLogic<ModeOfFinance, MODE_OF_FINANCE>
    {
        public ModeOfFinanceLogic()
        {
            translator = new ModeOfFinanceTranslator();
        }
    }
}