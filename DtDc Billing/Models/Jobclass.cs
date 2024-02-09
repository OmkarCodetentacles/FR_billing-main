using DtDc_Billing.Entity_FR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DtDc_Billing.Models
{
    public class Jobclass
    {
        private db_a92afa_frbillingEntities db = new db_a92afa_frbillingEntities();


        public void ExpiredStationary()
        {


            var stationary = db.Stationaries.ToList().Where(m => DateTime.Now.Date >= m.Expiry_Date.Value.AddDays(60) && m.Status == 0).ToList();


            List<string> Mystring = new List<string>();


            foreach (var i in stationary)
            {
                char stch = i.startno[0];
                char Endch = i.endno[0];

                long startConsignment = Convert.ToInt64(i.startno.Substring(1));
                long EndConsignment = Convert.ToInt64(i.endno.Substring(1));



                for (long b = startConsignment; b <= EndConsignment; b++)
                {

                    ExpiredStationary ex = new ExpiredStationary();

                    ex.Consignment_no = stch + b.ToString();

                    ex.Expiry_Date = i.Expiry_Date.Value.AddDays(90);



                    if (DateTime.Now > i.Expiry_Date.Value.AddDays(90))
                    {
                        ex.Expiry_Exceded = "Expired";
                    }

                    db.ExpiredStationaries.Add(ex);
                    db.SaveChanges();



                }




            }








        }



        public void deletefromExpiry(string Con_no)
        {

            ExpiredStationary expiredStationary = db.ExpiredStationaries.Where(m => m.Consignment_no == Con_no).FirstOrDefault();

            if (expiredStationary != null)
            {
                db.ExpiredStationaries.Remove(expiredStationary);
                db.SaveChanges();
            }
        }


    }
}