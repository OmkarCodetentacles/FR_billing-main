using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;

namespace DtDc_Billing.Models
{
    public class SendWhatsappMessage
    {
        private readonly HttpClient _httpClient=new HttpClient();
        public async Task<string> sendWhatsappMessage(string mobileno, string message)
        {
            try
            {
                string apiUrl = "https://thetexton.in/api/send";

                // Create a dictionary to hold your query parameters
                var parameters = new Dictionary<string, string>
        {
            { "number",91+mobileno },
            { "type", "text" },
            { "message", message },
            { "instance_id", "654A0CFB15D45" },
            { "access_token", "64b920aa7b961" }
        };

                // Use System.Web.HttpUtility to encode the parameters
                var queryString = string.Join("&", parameters.Select(kvp => $"{kvp.Key}={System.Web.HttpUtility.UrlEncode(kvp.Value)}"));

                // Construct the full URL
                string fullUrl = $"{apiUrl}?{queryString}";

                HttpResponseMessage response = await _httpClient.GetAsync(fullUrl);

                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadAsStringAsync();
                }
                else
                {
                    // Handle API errors or return null/empty string.
                    return string.Empty;
                }
                //string apiurl = "https://thetexton.in/api/send?number=91&type=text&message=Pratiksha&instance_id=64E7665101E8F&access_token=64b920aa7b961\r\n\r\n";

                //HttpResponseMessage response = await _httpClient.GetAsync(apiurl);
                //if (response.IsSuccessStatusCode)
                //{
                //  return await response.Content.ReadAsStringAsync();  
                //}
                //else
                //{
                //  //Handle Error return empty string
                //  return string.Empty;
                //}
            }
            catch (Exception ex)
            {
                return ex.Message;
            }

        }
    }
}