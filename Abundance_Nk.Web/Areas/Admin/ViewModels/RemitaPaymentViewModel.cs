﻿using Abundance_Nk.Model.Model;
using System.Collections.Generic;

namespace Abundance_Nk.Web.Areas.Admin.ViewModels
{
    public class RemitaPaymentViewModel
    {
        public RemitaPaymentViewModel()
        {
            remitaPaymentList = new List<RemitaPayment>();
            remitaList = new List<Remita>();
            remitaResponseList = new List<RemitaResponse>();
        }

        public RemitaPayment remitaPayment { get; set; }
        public Remita remita { get; set; }
        public RemitaResponse remitaResponse { get; set; }
        public List<RemitaPayment> remitaPaymentList { get; set; }
        public List<Remita> remitaList { get; set; }
        public List<RemitaResponse> remitaResponseList { get; set; }
        public string order_Id { get; set; }
        public string url { get; set; }
        public string message { get; set; }
        public string rrr { get; set; }
        public string statuscode { get; set; }
    }
}