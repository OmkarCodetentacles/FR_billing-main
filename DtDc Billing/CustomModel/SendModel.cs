using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;

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
        readonly HttpClient client = new HttpClient();
        public async Task Main(SendModel sendModel)
        {
            // Set your API key here
            string apiKey = "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";

            // JSON data for the email

            string jsonData = $@"
        {{
            ""sender"": {{  
                ""name"": ""Fr-Billing"",
                ""email"": ""frbillingsoftware@gmail.com""
            }},
            ""to"": [{{
                ""email"": ""{sendModel.toEmail}"",
                ""name"": ""Sir/Madam""
            }}],
            ""subject"": ""{sendModel.subject}"",
            ""htmlContent"": ""{sendModel.body}""
        }}";


            try
            {
                // Set up the request
                HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, "https://api.brevo.com/v3/smtp/email");
                request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                request.Headers.Add("api-key", apiKey);
                request.Content = new StringContent(jsonData, Encoding.UTF8, "application/json");

                // Send the request and get the response
                HttpResponseMessage response = await client.SendAsync(request);
                response.EnsureSuccessStatusCode();
                string responseBody = await response.Content.ReadAsStringAsync();

                Console.WriteLine(responseBody);
            }
            catch (HttpRequestException e)
            {
                Console.WriteLine("\nException Caught!");
                Console.WriteLine("Message :{0} ", e.Message);
            }
        }
      
    }
}