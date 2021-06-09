using Abundance_Nk.Business;
using Abundance_Nk.Model.Model;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Abundance_Nk.Web.Models
{
    public static class Menu
    {
        public static string GetUserRole(string userName)
        {
            string roleName = "";
            try
            {
                var userLogic = new UserLogic();
                User user = userLogic.GetModelsBy(u => u.User_Name == userName).FirstOrDefault();
                roleName = user.Role.Name;
            }
            catch(Exception)
            {
                throw;
            }

            return roleName;
        }

        public static List<Model.Model.Menu> GetMenuList(string role)
        {
            var menuList = new List<Model.Model.Menu>();
            try
            {
                var menuLogic = new MenuLogic();
                var menuInRoleLogic = new MenuInRoleLogic();

                List<MenuInRole> menuInRoleList =
                    menuInRoleLogic.GetModelsBy(m => m.ROLE.Role_Name == role && m.MENU.Activated);
                for(int i = 0;i < menuInRoleList.Count;i++)
                {
                    MenuInRole thisMenuInRole = menuInRoleList[i];
                    menuList.Add(thisMenuInRole.Menu);
                }
            }
            catch(Exception)
            {
                throw;
            }

            return menuList;
        }
    }
}