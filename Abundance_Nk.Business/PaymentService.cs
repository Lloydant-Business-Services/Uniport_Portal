using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Script.Serialization;

namespace Abundance_Nk.Business
{
    public class PaymentService
    {
        private string schoolCode = "28443389";
        private string apiKey = "CHCLk9InPJsbmS2cLhShV07guAWLDcBiTQ9QZWdMNog=";
        //public string basePaymentURL = "https://abiairs.gov.ng/aics-r-ws/rest/AbiaSchools/studentPayment";
        //public string baseRegistrationURL = "https://abiairs.gov.ng/aics-r-ws/rest/AbiaSchools/studentRegistration";
        public string basePaymentURL = "http://staging.abiairs.gov.ng/aics-r-ws/rest/AbiaSchools/studentPayment";
        public string baseRegistrationURL = "http://staging.abiairs.gov.ng/aics-r-ws/rest/AbiaSchools/studentRegistration";
      
        Response abiapayResponse;
       
 	   public Response PostJsonDataToUrl(string baseAddress,Request request)
       {
              abiapayResponse = new Response();
              try
              {

                  List<Request> toJson = new List<Request>();
                  toJson.Add(request);
                  string json = "";
                  string jsondata = "";
                  json = new JavaScriptSerializer().Serialize(toJson);
                  string toHash = json + apiKey; //hash: a SHA-256 hash of request JSON string + the school’s Api-key
                  string hash = HashPaymentDetailToSHA256(toHash);
                  if (request != null)
                  {
                       using (var webRequest = new WebClient())
                      {
                          webRequest.Headers[HttpRequestHeader.Accept] = "application/json";
                          webRequest.Headers[HttpRequestHeader.ContentType] = "application/json";
                          webRequest.Headers["schoolCode"]= schoolCode;
                          webRequest.Headers["hash"]= hash;
                          jsondata = webRequest.UploadString(baseAddress, "POST", json);
                      
                      }
                      jsondata = jsondata.Replace("jsonp(", "");
                      jsondata = jsondata.Replace(")", "");
                     
                      abiapayResponse = new JavaScriptSerializer().Deserialize<Response>(jsondata);

                  }
              }
              catch (Exception ex)
              {
                  abiapayResponse.message = ex.Message;
                  throw ex;
              }
              return abiapayResponse;
          }
       public RegistrationResponse PostJsonDataToUrl2(string baseAddress,StudentRegistrationRequest studentRegistrationRequest)
          {
             RegistrationResponse abiapayResponse = new RegistrationResponse();
              try
              {

                  List<StudentRegistrationRequest> toJson = new List<StudentRegistrationRequest>();
                  toJson.Add(studentRegistrationRequest);
                  string json = "";
                  string jsondata = "";
                  json = new JavaScriptSerializer().Serialize(toJson);
                  string toHash = json + apiKey; //hash: a SHA-256 hash of request JSON string + the school’s Api-key
                  string hash = HashPaymentDetailToSHA256(toHash);
                  if (studentRegistrationRequest != null)
                  {
                       using (var webRequest = new WebClient())
                      {
                          webRequest.Headers[HttpRequestHeader.Accept] = "application/json";
                          webRequest.Headers[HttpRequestHeader.ContentType] = "application/json";
                          webRequest.Headers["schoolCode"]= schoolCode;
                          webRequest.Headers["hash"]= hash;
                          jsondata = webRequest.UploadString(baseAddress, "POST", json);
                      
                      }
                     
                      abiapayResponse = new JavaScriptSerializer().Deserialize<RegistrationResponse>(jsondata);

                  }
              }
              catch (Exception ex)
              {
                  
                  throw ex;
              }
              return abiapayResponse;
          }
       private string HashPaymentDetailToSHA256(string hash_string)
        {
            System.Security.Cryptography.SHA256Managed sha256 = new System.Security.Cryptography.SHA256Managed();
            Byte[] EncryptedSHA256 = sha256.ComputeHash(System.Text.Encoding.UTF8.GetBytes(hash_string));
            sha256.Clear();
            string hashed = BitConverter.ToString(EncryptedSHA256).Replace("-", "").ToLower();
            return hashed;

        }
   
   }
    public class Request
    {
         public long totalAmountInKobo { get; set; }
         public string payerFirstName { get; set; }
         public string payerSurname { get; set; }
         public long studentId { get; set; }
         public string dateOfPayment { get; set; }
         public string trackingId { get; set; }
         public List<paymentLineItems> paymentLineItems { get; set; }

    }
    public class paymentLineItems
    {
         public string itemCode { get; set; }
         public string description { get; set; }
         public long amountInKobo { get; set; }
         public string datePaid { get; set; }
    }
    public class Response
    {
         public string status { get; set; }
         public string message { get; set; }
         public string trackingId { get; set; }
         public long totalAmountInKobo { get; set; }
    }
    public class RegistrationResponse
    {
       public List<Response> registrationResponses { get; set; }
    }
    public class StudentRegistrationRequest
    {
         //matric Number
         public long studentId { get; set; }
         public string address { get; set; }
         public string addressTown { get; set; }
         public string stateCode { get; set; }
         public string dateOfBirth { get; set; }
         public string trackingId { get; set; }
         public string yearOfRegistration { get; set; }
         public string studentCurrentClass { get; set; }
         public string firstName { get; set; }
         public string surname { get; set; }
         public string middleName { get; set; }
         public string gender { get; set; }

    }

}