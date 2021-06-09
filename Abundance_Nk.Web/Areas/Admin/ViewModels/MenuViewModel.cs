using Abundance_Nk.Model.Model;
using Abundance_Nk.Web.Models;
using System.Collections.Generic;
using System.Web.Mvc;
using Menu = Abundance_Nk.Model.Model.Menu;

namespace Abundance_Nk.Web.Areas.Admin.ViewModels
{
    public class MenuViewModel
    {
        public MenuViewModel()
        {
            RoleSelectList = Utility.PopulateRoleSelectListItem();
            MenuGroupSelectList = Utility.PopulateMenuGroupSelectListItem();
            MenuSelectList = Utility.PopulateMenuSelectListItem();
        }

        public Role Role { get; set; }
        public MenuGroup MenuGroup { get; set; }
        public Menu Menu { get; set; }
        public MenuInRole MenuInRole { get; set; }
        public List<Menu> MenuList { get; set; }
        public List<MenuInRole> MenuInRoleList { get; set; }
        public List<SelectListItem> RoleSelectList { get; set; }
        public List<SelectListItem> MenuGroupSelectList { get; set; }
        public List<SelectListItem> MenuSelectList { get; set; }
    }
}