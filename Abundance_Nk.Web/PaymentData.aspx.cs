using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Abundance_Nk.Business;
using Abundance_Nk.Model.Model;
using Microsoft.Ajax.Utilities;
using Newtonsoft.Json;

namespace Abundance_Nk.Web
{
    public partial class PaymentData :System.Web.UI.Page
    {
        public string statuscode;
        protected void Page_Load(object sender,EventArgs e)
        {
            //HttpContext.Current.Request.UrlReferrer.OriginalString;
            if (Request.QueryString["RECEIPT_NO"] != null)
            {
                 var d = HttpContext.Current.Request.UrlReferrer;
           
                PaymentEtranzactLogic paymentEtranzactLogic = new PaymentEtranzactLogic();
                PaymentEtranzact paymentEtranzact = new PaymentEtranzact();
                paymentEtranzact.ReceiptNo = Request.QueryString["RECEIPT_NO"];
                paymentEtranzact.PaymentCode =  Request.QueryString["PAYMENT_CODE"];
                paymentEtranzact.ConfirmationNo =  Request.QueryString["PAYMENT_CODE"];
                paymentEtranzact.MerchantCode =  Request.QueryString["MERCHANT_CODE"];
                paymentEtranzact.TransactionAmount = Convert.ToDecimal(Request.QueryString["TRANS_AMOUNT"]) ;
                paymentEtranzact.TransactionDescription =  Request.QueryString["TRANS_DESCR"];
                paymentEtranzact.BankCode =  Request.QueryString["BANK_CODE"];
                paymentEtranzact.BranchCode =  Request.QueryString["BRANCH_CODE"];
                paymentEtranzact.CustomerName =  Request.QueryString["CUSTOMER_NAME"];
                paymentEtranzact.CustomerAddress =  Request.QueryString["CUSTOMER_ADDRESS"];
                paymentEtranzact.CustomerID =  Request.QueryString["CUSTOMER_ID"];
                paymentEtranzact.TransactionDate = Convert.ToDateTime(Request.QueryString["TRANS_DATE"]);
                paymentEtranzact.Used = false;
                paymentEtranzact.UsedBy = 0;

                string jison = "";
                statuscode = "Transaction Status = true";
               
                //jison = JsonConvert.SerializeObject(statuscode);
                //HttpContext.Current.Response.ContentType = "text/json";
                //HttpContext.Current.Response.Write(jison);

                //Payment payment = new Payment();
                //PaymentLogic paymentLogic = new PaymentLogic();
                //payment = paymentLogic.GetBy(paymentEtranzact.CustomerID);
                //if (payment != null && payment.Id > 0)
                //{
                //    PaymentTerminalLogic paymentTerminalLogic = new PaymentTerminalLogic();
                //    PaymentTerminal paymentTerminal = paymentTerminalLogic.GetBy(payment);
                //    if (paymentTerminal != null)
                //    {
                //       paymentEtranzact.Terminal = paymentTerminal;
                //    }
                //    else
                //    {
                //         paymentEtranzact.Terminal = new PaymentTerminal(){Id = 1, FeeType = new FeeType(){Id = 1}};
                //    }
                //    var paymentEtranzactType = new PaymentEtranzactType();
                //    var paymentEtranzactTypeLogic = new PaymentEtranzactTypeLogic();
                //    paymentEtranzactType = paymentEtranzactTypeLogic.GetModelsBy(p => p.Fee_Type_Id == paymentEtranzact.Terminal.FeeType.Id).LastOrDefault();
                //    paymentEtranzact.EtranzactType = paymentEtranzactType;
                //    paymentEtranzactLogic.Create(paymentEtranzact);
                //    string json = "";
                //    statuscode = "Transaction Status = true";
                //    json = JsonConvert.SerializeObject(statuscode);
                //    HttpContext.Current.Response.ContentType = "text/json";
                //    HttpContext.Current.Response.Write(json);
                //}

            }
            
        }
    }
}