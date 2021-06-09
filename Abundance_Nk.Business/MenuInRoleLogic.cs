using System;
using System.Linq;
using System.Linq.Expressions;
using Abundance_Nk.Model.Entity;
using Abundance_Nk.Model.Model;
using Abundance_Nk.Model.Translator;

namespace Abundance_Nk.Business
{
    public class MenuInRoleLogic : BusinessBaseLogic<MenuInRole, MENU_IN_ROLE>
    {
        public MenuInRoleLogic()
        {
            translator = new MenuInRoleTranslator();
        }
         public bool Modify(MenuInRole menuInRole)
        {
            try
            {
                Expression<Func<MENU_IN_ROLE, bool>> selector = a => a.Menu_In_Role_Id == menuInRole.Id;
                MENU_IN_ROLE entity = GetEntityBy(selector);
                if (entity != null && entity.Menu_Id > 0)
                {
                    entity.Activated = menuInRole.Activated;
                    if (menuInRole.Menu != null)
                    {
                        entity.Menu_Id = menuInRole.Menu.Id;
                    }
                    if (menuInRole.Role != null)
                    {
                        entity.Role_Id = menuInRole.Role.Id;
                    }

                    int modifiedRecordCount = Save();

                    if (modifiedRecordCount > 0)
                    {
                        return true;
                    }
                }

                return false;
            }
            catch (Exception)
            {
                throw;
            }
        }
        
         public bool isMenuInRole(string action, string controller, Role role)
        {
            try
            {
                MenuInRole menuInRole = GetModelsBy(a => a.MENU.Controller ==controller && a.MENU.Action == action && a.Role_Id == role.Id ).FirstOrDefault();
                if (menuInRole != null)
                {
                    return true;
                }
            }
            catch (Exception)
            {
                
                throw;
            }
            return false;
        }
    
    }
}