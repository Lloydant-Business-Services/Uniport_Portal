using Abundance_Nk.Model.Model;
using Abundance_Nk.Web.Models;
using System.Net;
using System.Web.Mvc;

namespace Abundance_Nk.Web.Controllers
{
    //[SetPermissions()]
    public class BaseController :Controller
    {
        protected void SetMessage(string message,Message.Category messageType)
        {
            var msg = new Message(message,(int)messageType);
            TempData["Message"] = msg;
        }
        protected static bool CheckForInternetConnection()
        {
            try
            {
                using (var client = new WebClient())
                using (client.OpenRead("http://google.com/generate_204"))
                    return true;
            }
            catch
            {
                return false;
            }
        }
    }
    
}