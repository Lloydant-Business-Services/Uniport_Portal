using Abundance_Nk.Model.Entity;
using Abundance_Nk.Model.Model;
using Abundance_Nk.Model.Translator;

namespace Abundance_Nk.Business
{
    public class BankBranchLogic : BusinessBaseLogic<BankBranch, BANK_BRANCH>
    {
        public BankBranchLogic()
        {
            translator = new BankBranchTranslator();
        }
    }
}