using Microsoft.SqlServer.Management.Smo;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Text;

namespace DtDc_Billing.Models
{
    public class sendEmail
    {

        public class mail
            {
            public string franchiseName { get;set; }

            public DateTime After1Year { get;set; }

            public string emailId { get;set; }
            }
        public static void send(mail mailDetails)
        {
            //Fetching Email Body Text from EmailTemplate File.  
            string FilePath = System.Web.HttpContext.Current.Server.MapPath("~/images/SignUp.html");// "D:\\MBK\\SendEmailByEmailTemplate\\EmailTemplates\\SignUp.html";

            //string FilePath = "http://codetentacles-005-site1.htempurl.com/images/SignUp.html";// "D:\\MBK\\SendEmailByEmailTemplate\\EmailTemplates\\SignUp.html";
            StreamReader str = new StreamReader(FilePath);
            string MailText = str.ReadToEnd();
            str.Close();

            //Repalce [newusername] = signup user name   
            MailText = MailText.Replace("[newusername]", mailDetails.franchiseName);
            MailText = MailText.Replace("[RenewalDate]", mailDetails.After1Year.ToString("MM/dd/yyyy"));


            string subject = "Welcome To FrBilling Subscription";

            //Base class for sending email  
            MailMessage _mailmsg = new MailMessage();

            //Make TRUE because our body text is html  
            _mailmsg.IsBodyHtml = true;

            //Set From Email ID  
            _mailmsg.From = new MailAddress("frbillingsoftware@gmail.com");

            //Set To Email ID  
            _mailmsg.To.Add(mailDetails.emailId);

            //Set Subject  
            _mailmsg.Subject = subject;

            //Set Body Text of Email   
            _mailmsg.Body = MailText;


            //Now set your SMTP   
            SmtpClient _smtp = new SmtpClient();

            //Set HOST server SMTP detail  
            _smtp.Host = "smtp.gmail.com";

            //Set PORT number of SMTP  
            _smtp.Port = 587;

            //Set SSL --> True / False  
            _smtp.EnableSsl = true;
            _smtp.UseDefaultCredentials = false;
            //Set Sender UserEmailID, Password  
            NetworkCredential _network = new NetworkCredential("frbillingsoftware@gmail.com", "rqaynjbevkygswkx");
            _smtp.Credentials = _network;

            //Send Method will send your MailMessage create above.  
            _smtp.Send(_mailmsg);
        }
    }
}