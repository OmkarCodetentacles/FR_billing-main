using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Newtonsoft.Json;
using System.Dynamic;

namespace DtDc_Billing.CustomModel
{
  
    public class SendModel
    {
        public string toEmail { get; set; }
        public string subject { get; set; }
        public dynamic body { get; set; }

      
    }
    
    class SendEmailModel
    {
        public async Task<string> MailSend(SendModel sendEmailModel)
        {
            if (sendEmailModel != null)
            {
                var mailMessage = await Main(sendEmailModel);
                return mailMessage;
            }
            return null;
        }


        //readonly HttpClient client = new HttpClient();
        public async Task<string> Main(SendModel sendModel)
        {
            // Set your API key here
            string apiKey = "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";

            // JSON data for the email

            //    string jsonData = $@"
            //{{
            //    ""sender"": {{  
            //        ""name"": ""Fr-Billing"",
            //        ""email"": ""frbillingsoftware@gmail.com""
            //    }},
            //    ""to"": [{{
            //        ""email"": ""{sendModel.toEmail}"",
            //        ""name"": ""Sir/Madam""
            //    }}],
            //    ""subject"": ""{sendModel.subject}"",
            //    ""htmlContent"": ""{sendModel.body}""
            //}}";

            //    using (HttpClient client = new HttpClient())
            //    {
            //        try
            //        {
            //            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            //            client.DefaultRequestHeaders.Add("api-key", apiKey);
            //            var br = JsonConvert.SerializeObject(jsonData, Formatting.Indented);

            //            var content = new StringContent(br, Encoding.UTF8, "application/json");

            //         //   var content = new StringContent(jsonData, Encoding.UTF8, "application/json");
            //            HttpResponseMessage response = await client.PostAsync("https://api.brevo.com/v3/smtp/email", content);

            //            response.EnsureSuccessStatusCode();
            //            string responseBody = await response.Content.ReadAsStringAsync();
            //            return responseBody;
            //        }
            //        catch (HttpRequestException e)
            //        {
            //            return e.Message;
            //        }
            //    }
            //try
            //{
            //    // Set up the request
            //    HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, "https://api.brevo.com/v3/smtp/email");
            //    request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            //    request.Headers.Add("api-key", apiKey);
            //    request.Content = new StringContent(jsonData, Encoding.UTF8, "application/json");

            //    // Send the request and get the response
            //    HttpResponseMessage response = await client.SendAsync(request);
            //    response.EnsureSuccessStatusCode();
            //    string responseBody = await response.Content.ReadAsStringAsync();
            //    return responseBody;
            //    Console.WriteLine(responseBody);

            //}
            //catch (HttpRequestException e)
            //{
            //    return e.Message;
            //    Console.WriteLine("\nException Caught!");
            //    Console.WriteLine("Message :{0} ", e.Message);
            //}

            dynamic toRecipient = new ExpandoObject();
            toRecipient.email =sendModel.toEmail;
            toRecipient.name = "Sir/Madam";

            List<ExpandoObject> toList = new List<ExpandoObject> { toRecipient };

            dynamic emailData = new ExpandoObject();
            emailData.sender = new ExpandoObject();
            emailData.sender.name = "Fr-Billing";
            emailData.sender.email = "frbillingsoftware@gmail.com";
            emailData.to = toList;
            emailData.subject =sendModel.subject;
            emailData.htmlContent =sendModel.body;

            // Convert dynamic object to JSON string
            var jsonBody = JsonConvert.SerializeObject(emailData, Formatting.Indented);

            using (HttpClient client = new HttpClient())
            {
                try
                {
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    client.DefaultRequestHeaders.Add("api-key", apiKey);

                    var content = new StringContent(jsonBody, Encoding.UTF8, "application/json");

                    HttpResponseMessage response = await client.PostAsync("https://api.brevo.com/v3/smtp/email", content);

                    response.EnsureSuccessStatusCode();
                    string responseBody = await response.Content.ReadAsStringAsync();
                    return responseBody;
                }
                catch (HttpRequestException e)
                {
                    return e.Message;
                }
            }
        }
      
    }
}