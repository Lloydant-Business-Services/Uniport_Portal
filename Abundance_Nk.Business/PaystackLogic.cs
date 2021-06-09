using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Abundance_Nk.Model.Entity;
using Abundance_Nk.Model.Model;
using Abundance_Nk.Model.Translator;
using Newtonsoft.Json;
using RestSharp;
namespace Abundance_Nk.Business
{
	public class PaystackLogic:BusinessBaseLogic<Paystack,PAYMENT_PAYSTACK>
	{
		private RestClient client;
		protected RestRequest request;
		public static string RestUrl = "https://api.paystack.co/";
	   // public static string RestUrl = "https://requestb.in";
		static string ApiEndPoint = "";
		public PaystackLogic()
		{
			translator = new PaystackTranslator();
			client = new RestClient(RestUrl);
		}

		public PaystackRepsonse MakePayment(Payment payment, string Bearer, string SubAccount, decimal amount, string department, string level, string matricNo, decimal transaction_charge = 2180)
		{
			PaystackRepsonse paystackRepsonse = null;
			try
			{
				
				long milliseconds = DateTime.Now.Ticks;
				string testid =  milliseconds.ToString();
				ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
				ApiEndPoint = "/transaction/initialize";
			   // ApiEndPoint = "/uf06ccuf";
				request = new RestRequest(ApiEndPoint,Method.POST);
				request.AddHeader("accept", "application/json");
				request.AddHeader("Authorization", "Bearer " + Bearer);
				request.AddParameter("reference", payment.InvoiceNumber);
				//request.AddParameter("reference", DateTime.Now.Ticks + "will");
				request.AddParameter("transaction_charge", transaction_charge * 100);
				var person = request.JsonSerializer.Serialize(payment);
				request.AddParameter("amount", amount * 100);

				List<CustomeField> myCustomfields = new List<CustomeField>();

				CustomeField nameCustomeField = new CustomeField();
				nameCustomeField.display_name = "Name";
				nameCustomeField.variable_name = "Name";
				nameCustomeField.value = payment.Person.Name;
				myCustomfields.Add(nameCustomeField);

				CustomeField PhoneCustomeField = new CustomeField();
				PhoneCustomeField.display_name = "Phone Number";
				PhoneCustomeField.variable_name = "phone_number";
				PhoneCustomeField.value = payment.Person.MobilePhone;
				myCustomfields.Add(PhoneCustomeField);

				CustomeField PaymentCustomeField = new CustomeField();
				PaymentCustomeField.display_name = "Fee Type";
				PaymentCustomeField.variable_name = "fee_type";
				PaymentCustomeField.value = payment.FeeType.Name;
				myCustomfields.Add(PaymentCustomeField);

				CustomeField SessionCustomeField = new CustomeField();
				SessionCustomeField.display_name = "Academic Session";
				SessionCustomeField.variable_name = "academic_session";
				SessionCustomeField.value = payment.Session.Name;
				myCustomfields.Add(SessionCustomeField);

				CustomeField DepartmentCustomeField = new CustomeField();
				DepartmentCustomeField.display_name = "Department";
				DepartmentCustomeField.variable_name = "department";
				DepartmentCustomeField.value = department;
				myCustomfields.Add(DepartmentCustomeField);

				CustomeField LevelCustomeField = new CustomeField();
				LevelCustomeField.display_name = "Academic Level";
				LevelCustomeField.variable_name = "academic_level";
				LevelCustomeField.value = level;
				myCustomfields.Add(LevelCustomeField);

				CustomeField MatricNumberCustomeField = new CustomeField();
				MatricNumberCustomeField.display_name = "Matric Number";
				MatricNumberCustomeField.variable_name = "matric_number";
				MatricNumberCustomeField.value = matricNo;
				myCustomfields.Add(MatricNumberCustomeField);

				var javaScriptSerializer = new System.Web.Script.Serialization.JavaScriptSerializer();
				Dictionary<string, List<CustomeField>> metadata = new Dictionary<string, List<CustomeField>>();

				metadata.Add("custom_fields", myCustomfields);
				var serializedMetadata = javaScriptSerializer.Serialize(metadata);

				request.AddParameter("metadata", serializedMetadata);

				if (!String.IsNullOrEmpty(payment.Person.Email))
				{
				   request.AddParameter("email", payment.Person.Email);
				}
				else
				{
					request.AddParameter("email",  "support@abiastateuniversity.edu.ng");
				}
				if (!String.IsNullOrEmpty(SubAccount))
				{
					request.AddParameter("subaccount", SubAccount);
				}

				var serializedRequest = JsonConvert.SerializeObject(request);

				var result = client.Execute(request);
				if (result.StatusCode == System.Net.HttpStatusCode.OK)
				{
					paystackRepsonse = JsonConvert.DeserializeObject<PaystackRepsonse>(result.Content);
				}
				return paystackRepsonse;
			}
			catch (Exception ex)
			{  
				throw ex;
			}
		}

		public PaystackRepsonse VerifyPayment(Payment payment, string Bearer)
		{
			PaystackRepsonse paystackRepsonse = null;
			try
			{
				ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
				ApiEndPoint = "/transaction/verify/"+payment.InvoiceNumber;
				request = new RestRequest(ApiEndPoint,Method.GET);
				request.AddHeader("accept", "application/json");
				request.AddHeader("Authorization", "Bearer " + Bearer);
				var result = client.Execute(request);
				if (result.StatusCode == System.Net.HttpStatusCode.OK)
				{
					paystackRepsonse = JsonConvert.DeserializeObject<PaystackRepsonse>(result.Content);
					Update(paystackRepsonse);
					payment = UpdateScratchCardPin(payment);
				}
				return paystackRepsonse;
			}
			catch (Exception ex)
			{  
				throw ex;
			}
		}

		private static Payment UpdateScratchCardPin(Payment payment)
		{
			//Check if payment is scratch card
			PaymentLogic paymentLogic = new PaymentLogic();
			payment = paymentLogic.GetBy(payment.InvoiceNumber);
			if (payment.Id > 0 && payment.FeeType.Id == 7)
			{
				ScratchCard card = new ScratchCard();
				ScratchCardLogic cardLogic = new ScratchCardLogic();
				var cardExists = cardLogic.GetBy(payment.InvoiceNumber);
				if (cardExists == null)
				{
					card.Batch = new ScratchCardBatch() { Id = 1 };
					card.FirstUsedDate = DateTime.Now;
					card.person = payment.Person;
					card.Pin = payment.InvoiceNumber;
					card.SerialNumber = payment.SerialNumber.ToString();
					card.UsageCount = 0;
					cardLogic.Create(card);
				}
				

			}
			else if(payment.Id > 0 && payment.FeeType.Id == 8)
			{
				ScratchCard card = new ScratchCard();
				ScratchCardLogic cardLogic = new ScratchCardLogic();
				var cardExists = cardLogic.GetBy(payment.InvoiceNumber);
				if (cardExists == null)
				{
					card.Batch = new ScratchCardBatch() { Id = 2 };
					card.FirstUsedDate = DateTime.Now;
					card.person = payment.Person;
					card.Pin = payment.InvoiceNumber;
					card.SerialNumber = payment.SerialNumber.ToString();
					card.UsageCount = 0;
					cardLogic.Create(card);
				}

			}
			return payment;
		}

		public Paystack GetBy(Payment payment)
		{

			try
			{
				return GetModelBy(a => a.Payment_Id == payment.Id);
			}
			catch (Exception ex)
			{
				throw ex;
			}
		}
   
	   public Paystack GetBy(string reference)
		{

			try
			{
				return GetModelBy(a => a.PAYMENT.Invoice_Number == reference);
			}
			catch (Exception ex)
			{
				throw ex;
			}
		}

	   public bool Update(PaystackRepsonse PaystackRepsonse)
		{
			try
			{
				Expression<Func<PAYMENT_PAYSTACK, bool>> selector = p => p.PAYMENT.Invoice_Number == PaystackRepsonse.data.reference;
				PAYMENT_PAYSTACK _paystackEntity = GetEntityBy(selector);
				if (_paystackEntity != null)
				{
					_paystackEntity.amount = PaystackRepsonse.data.amount;
					_paystackEntity.bank = PaystackRepsonse.data.authorization.bank;
					_paystackEntity.brand = PaystackRepsonse.data.authorization.brand;
					_paystackEntity.card_type = PaystackRepsonse.data.authorization.card_type;
					_paystackEntity.channel = PaystackRepsonse.data.channel;
					_paystackEntity.country_code = PaystackRepsonse.data.authorization.country_code;
					_paystackEntity.currency = PaystackRepsonse.data.currency;
					_paystackEntity.domain = PaystackRepsonse.data.domain;
					_paystackEntity.exp_month = PaystackRepsonse.data.authorization.exp_month;
					_paystackEntity.exp_year = PaystackRepsonse.data.authorization.exp_year;
					_paystackEntity.fees = PaystackRepsonse.data.fees.ToString();
					_paystackEntity.gateway_response = PaystackRepsonse.data.gateway_response;
					_paystackEntity.ip_address = PaystackRepsonse.data.ip_address;
					_paystackEntity.last4 = PaystackRepsonse.data.authorization.last4;
					_paystackEntity.message = PaystackRepsonse.message;
					_paystackEntity.reference = PaystackRepsonse.data.reference;
					_paystackEntity.reusable = PaystackRepsonse.data.authorization.reusable;
					_paystackEntity.signature = PaystackRepsonse.data.authorization.signature;
					_paystackEntity.status = PaystackRepsonse.data.status;
					_paystackEntity.transaction_date = PaystackRepsonse.data.transaction_date;
					int modifiedRecordCount = Save();
					if (modifiedRecordCount <= 0)
					{
						return false;
					}
					return true;
				}
				return false;
			}
			catch (Exception ex)
			{
				throw ex;
			}
		}

	   public Payment ValidatePayment(string reference)
		{
			try
			{
				var details = GetModelBy(  a =>a.PAYMENT.Invoice_Number == reference);
                if (details != null && details.status != null && details.status.Contains("success") && (details.gateway_response.Contains("Approved") || details.gateway_response.Contains("Transaction Successful") || details.gateway_response.Contains("Successful") || details.gateway_response.Contains("Payment successful") || details.gateway_response.Contains("success")) && details.domain == "live")
				{
					details.Payment.Amount = details.amount.ToString();
					return details.Payment;
				}
				//return details.Payment;
				return null;

			   
			}
			catch (Exception ex)
			{
				throw ex;
			}
		}

	   public bool ValidateAmountPaid(string reference, decimal AmountToBePaid)
		{
			try
			{
			   var payment = GetModelBy(
						a =>
							a.PAYMENT.Invoice_Number == reference && a.status.Contains("success")  && a.amount >= AmountToBePaid);
				if (payment != null && payment.Payment.Id > 0)
				{
					return true;
				}
				return false;
			}
			catch (Exception ex)
			{
				throw ex;
			}  
		}

		public List<PaymentPaystackView> GetPaymentBy(string dateFrom, string dateTo)
		{
			try
			{
				DateTime processedDateFrom = new DateTime();
				DateTime processedDateTo = new DateTime();

				processedDateFrom = ConvertToDate(dateFrom);
				processedDateTo = ConvertToDate(dateTo);
				TimeSpan ts = new TimeSpan(00, 00, 0);
				processedDateFrom = processedDateFrom.Date + ts;
				ts = new TimeSpan(23, 59, 0);
				processedDateTo = processedDateTo.Date + ts;

				List<PaymentPaystackView> payments =
					(from sr in
						repository.GetBy<VW_PAYMENT_PAYSTACK>(
							p => (p.transaction_date >= processedDateFrom && p.transaction_date <= processedDateTo))
					 select new PaymentPaystackView
					 {
						 InvoiceNumber = sr.Invoice_Number,
						 PaymentModeId = sr.Payment_Mode_Id,
						 FeeTypeId = sr.Fee_Type_Id,
						 SessionId = sr.Session_Id,
						 PersonId = sr.Person_Id,
						 FeeTypeName = sr.Fee_Type_Name,
						 TransactionAmount = sr.Amount,
						 FullName = sr.Expr1,
						 SessionName = sr.Session_Name,
						 MatricNumber = sr.Matric_NO,
						 LevelName = sr.Level_Name,
						 DepartmentName = sr.Department_Name,
						 FacultyName = sr.Faculty_Name,
						 ProgrammeName = sr.Programme_Name,
						 TransactionDate = sr.transaction_date,
                         PaymentMode = sr.Payment_Mode_Name
					 }).ToList();
				return payments;
			}




			catch (Exception)
			{

				throw;
			}
		}
		private DateTime ConvertToDate(string date)
		{
			DateTime newDate = new DateTime();
			try
			{
				//newDate = DateTime.Parse(date);
				string[] dateSplit = date.Split('-');
				newDate = new DateTime(Convert.ToInt32(dateSplit[0]), Convert.ToInt32(dateSplit[1]), Convert.ToInt32(dateSplit[2]));
			}
			catch (Exception)
			{
				throw;
			}

			return newDate;
		}
        public bool HasStudentPaidSugFeeForSession(long StudentId, Session session, FeeType feeType)
        {
            try
            {
                Expression<Func<PAYMENT_PAYSTACK, bool>> selector;

                   selector = p => p.PAYMENT.Person_Id == StudentId && p.PAYMENT.Fee_Type_Id == feeType.Id && p.PAYMENT.Payment_Mode_Id == 1 && p.PAYMENT.Session_Id== session.Id;
                
                Paystack payment = GetModelBy(selector);
                if (payment != null && payment.status!=null &&  payment.status.Contains("success") && payment.domain!=null && payment.domain.Contains("live") )
                {
                    return true;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return false;
        }


    }
}
