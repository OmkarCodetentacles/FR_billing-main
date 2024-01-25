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
using System.IO;
using System.Net.Mail;
using System.Net;

namespace DtDc_Billing.CustomModel
{
  
    public class SendModel
    {
        public string toEmail { get; set; }
        public string subject { get; set; }
        public dynamic body { get; set; }
        public string filepath { get; set;  }
      
    }
    
  public  class SendEmailModel
    {
        public async Task<string> MailSend(SendModel sendEmailModel)
        {
            if (sendEmailModel.toEmail != null)
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



            dynamic toRecipient = new ExpandoObject();
            toRecipient.email = sendModel.toEmail;
            toRecipient.name = "Sir/Madam";

            List<ExpandoObject> toList = new List<ExpandoObject> { toRecipient };

            dynamic emailData = new ExpandoObject();
            emailData.sender = new ExpandoObject();
            emailData.sender.name = "Fr-Billing";
            emailData.sender.email = "frbillingsoftware@gmail.com";
            emailData.to = toList;

            emailData.subject = sendModel.subject;
            emailData.htmlContent = sendModel.body;


            // Attachments
            if (!string.IsNullOrEmpty(sendModel.filepath))
            {
                try
                {
                    byte[] fileBytes = File.ReadAllBytes(sendModel.filepath);


                    string base64File = Convert.ToBase64String(fileBytes);

                    dynamic attachment = new ExpandoObject();

                    attachment.name = Path.GetFileName(sendModel.filepath); // Replace with the actual file name
                    attachment.content = base64File;

                    List<ExpandoObject> attachments = new List<ExpandoObject> { attachment };
                    emailData.attachments = attachments;
                }
                catch (Exception ex){ 
                    return ex.Message;
                }

            }

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



        public  async Task<string> SendEmailWithAttachment(SendModel sendEmailModel)
        {
            if (sendEmailModel.toEmail != null)
            {
                var mailMessage = await SendEmailWithAttachmentMethod(sendEmailModel);
                return mailMessage;
            }
            return null;
        }
        public async Task<string> SendEmailWithAttachmentMethod(SendModel sendModel)
            {
                // Replace these values with your actual email and SMTP server details
                string senderEmail = "prajaktacodetentacles@gmail.com";
                string senderPassword = "Prajakta@123";
                string recipientEmail = sendModel.toEmail;
                string subject = sendModel.subject;
                string body = sendModel.body;

                // Replace with the actual file path of the attachment
                string attachmentPath = sendModel.filepath;

                try
                {
                    // Create the email message
                    MailMessage mail = new MailMessage(senderEmail, recipientEmail, subject, body);

                    // Attach the file
                    Attachment attachment = new Attachment(attachmentPath);
                    mail.Attachments.Add(attachment);

                    // Set up SMTP client
                    SmtpClient smtp = new SmtpClient("smtp.gmail.com")
                    {
                        Port = 587,
                        Credentials = new NetworkCredential(senderEmail, senderPassword),
                        EnableSsl = true,
                    };

                    // Send the email
                    await smtp.SendMailAsync(mail);

                    return "Email sent successfully.";
                }
                catch (Exception ex)
                {
                    return "Error sending email: " + ex.Message;
                }
            }




        



    }
}