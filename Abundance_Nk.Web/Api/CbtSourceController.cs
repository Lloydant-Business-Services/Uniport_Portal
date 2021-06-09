using Abundance_Nk.Business;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Abundance_Nk.Web.Api
{
    public class CbtSourceController : ApiController
    {
        private readonly ApplicationFormLogic applicationFormLogic;
        public CbtSourceController()
        {
            applicationFormLogic = new ApplicationFormLogic();
        }
        //public HttpResponseMessage GetApplicationBy(int SessionId)
        //{
        //    //var allAcceptanceReportCount = paymentLogic.GetAcceptanceReportsCount();

        //    return Request.CreateResponse(allAcceptanceReportCount);

        //}
    }
}
