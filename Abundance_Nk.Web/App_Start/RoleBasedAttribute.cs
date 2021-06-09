using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http.Controllers;
using System.Web.Mvc;
using System.Web.Routing;
using Abundance_Nk.Model.Model;
using Abundance_Nk.Business;

namespace Abundance_Nk.Web
{
    public class RoleBasedAttribute: AuthorizeAttribute
    {
         public override void OnAuthorization(AuthorizationContext filterContext)
            {      
                /*Create permission string based on the requested controller 
                  name and action name in the format 'controllername-action'*/
                string requiredPermission = String.Format("{0}-{1}",filterContext.ActionDescriptor.ControllerDescriptor.ControllerName,filterContext.ActionDescriptor.ActionName);

                MenuInRoleLogic menuInRoleLogic = new MenuInRoleLogic();
                UserLogic userLogic = new UserLogic();

                /*Create an instance of our custom user authorization object passing requesting 
                  user's 'Windows Username' into constructor*/
                User requestingUser = userLogic.GetBy( filterContext.RequestContext.HttpContext.User.Identity.Name);
                
                
                //Check if the requesting user has the permission to run the controller's action
                if (requestingUser != null &&  requestingUser.Role.Id > 0)
                {
                    if (!menuInRoleLogic.isMenuInRole(filterContext.ActionDescriptor.ActionName,filterContext.ActionDescriptor.ControllerDescriptor.ControllerName,requestingUser.Role))
                    {
                         /*User doesn't have the required permission and is not a SysAdmin, return our 
                      custom '401 Unauthorized' access error. Since we are setting 
                      filterContext.Result to contain an ActionResult page, the controller's 
                      action will not be run.

                      The custom '401 Unauthorized' access error will be returned to the 
                      browser in response to the initial request.*/
                    //filterContext.Result = new RedirectToRouteResult(
                    //                               new RouteValueDictionary { 
                    //                                    { "action", "Login" }, 
                    //                                    { "controller", "Account" },
                    //                                    {"Area","Security"}});
                    }
                   
                }
                /*If the user has the permission to run the controller's action, then 
                  filterContext.Result will be uninitialized and executing the controller's 
                  action is dependent on whether filterContext.Result is uninitialized.*/
            }
    
    }
}