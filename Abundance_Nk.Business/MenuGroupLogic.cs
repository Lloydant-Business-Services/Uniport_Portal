using Abundance_Nk.Model.Entity;
using Abundance_Nk.Model.Model;
using Abundance_Nk.Model.Translator;

namespace Abundance_Nk.Business
{
    public class MenuGroupLogic : BusinessBaseLogic<MenuGroup, MENU_GROUP>
    {
        public MenuGroupLogic()
        {
            translator = new MenuGroupTranslator();
        }
    }
}