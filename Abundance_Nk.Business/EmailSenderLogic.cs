using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using Abundance_Nk.Model.Model;
using RazorEngine;
using RazorEngine.Templating;
using RestSharp;
using RestSharp.Authenticators;

namespace Abundance_Nk.Business
{
    public class EmailSenderLogic<T>:IEmailSender 
    {
        private string _templateFilePath;
        private T _model;
        public EmailSenderLogic(string templateFilePath,T model)
        {
            _templateFilePath = templateFilePath;
            _model = model;
        }
        public void Send(Model.Model.EmailMessage message)
        {
            try
            {

                string template = File.ReadAllText(_templateFilePath);
                if (!string.IsNullOrEmpty(template))
                {
                    var key = "key_" + DateTime.Now.Ticks;

                    string result = Engine.Razor.RunCompile(template, key, null, _model);
                    if (!string.IsNullOrEmpty(result))
                    {
                        message.Body = result;
                        useMailgun(message);
                    }
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public void Send(List<Model.Model.EmailMessage> messages)
        {
            foreach (EmailMessage message in messages)
            {
                Send(message);
            }
        }

        public bool SendPasswordReset(Model.Model.EmailMessage message)
        {
            try
            {

                string template = File.ReadAllText(_templateFilePath);
                if (!string.IsNullOrEmpty(template))
                {
                    var key = "key_" + DateTime.Now.Ticks;

                    string result = Engine.Razor.RunCompile(template, key, null, _model);
                    if (!string.IsNullOrEmpty(result))
                    {
                        message.Body = result;
                        useMailgun(message);
                        return true;
                    }

                }
                return false;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        private IRestResponse useMailgun(EmailMessage message)
        {
            RestClient client = new RestClient();
            client.BaseUrl = new Uri("https://api.mailgun.net/v3");
            client.Authenticator =
                new HttpBasicAuthenticator("api",
                    "key-8540f3ef6a66cdaf8d9121f11c99aa6b");
            RestRequest request = new RestRequest();
            request.AddParameter("domain", "nrf.lloydant.com", ParameterType.UrlSegment);
            request.Resource = "{domain}/messages";
            //if (message.From != null)
            //{
            //    request.AddParameter("from", message.From);
            //}
            //else
            //{
            request.AddParameter("from", "Portal Admin <portaladmin@uniport.edu.ng>");
            //}

            request.AddParameter("to", message.Email);
            request.AddParameter("subject", message.Subject);
            request.AddParameter("html", message.Body);
            request.Method = Method.POST;
            return client.Execute(request);
        }


    }
}
