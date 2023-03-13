using DtDc_Billing.Entity_FR;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace DtDc_Billing.Models
{
    public class CompanyModel
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public CompanyModel()
        {
            this.Nondoxes = new HashSet<Nondox>();
            this.Nondoxes1 = new HashSet<Nondox>();
            this.Priorities = new HashSet<Priority>();
            this.Ratems = new HashSet<Ratem>();
            this.Ratems1 = new HashSet<Ratem>();
            this.dtdcPlus = new HashSet<dtdcPlu>();
            this.Dtdc_Ptp = new HashSet<Dtdc_Ptp>();
            this.express_cargo = new HashSet<express_cargo>();
            this.Transactions = new HashSet<Transaction>();
        }

        [Required(ErrorMessage = "Please Enter Company Id")]
        public string Company_Id { get; set; }

        public int c_id { get; set; }

        [Required(ErrorMessage = "Please Enter Phone No")]
        [DataType(DataType.PhoneNumber)]
        [RegularExpression(@"^\(?([0-9]{3})\)?[-. ]?([0-9]{3})[-. ]?([0-9]{4})$", ErrorMessage = "Not a valid phone number")]
        public Nullable<long> Phone { get; set; }

        [Required(ErrorMessage = "Please Enter Email Id")]
        [MaxLength(50)]
        [RegularExpression(@"[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,4}", ErrorMessage = "Please enter correct email")]
        public string Email { get; set; }

        public Nullable<double> Insurance { get; set; }
        public Nullable<double> Minimum_Risk_Charge { get; set; }
        public string Other_Details { get; set; }
        public Nullable<double> Fuel_Sur_Charge { get; set; }
        public Nullable<double> Topay_Charge { get; set; }
        public Nullable<double> Cod_Charge { get; set; }
        public Nullable<double> Gec_Fuel_Sur_Charge { get; set; }
        public string Pf_code { get; set; }

        [Required(ErrorMessage = "Please Enter Company Address")]
        public string Company_Address { get; set; }

        [Required(ErrorMessage = "Please Enter Company Name")]
        public string Company_Name { get; set; }

        public Nullable<System.DateTime> Datetime_Comp { get; set; }

        [Required(ErrorMessage = "Please Enter Gst No")]
        public string Gst_No { get; set; }

        [Required(ErrorMessage = "Please Enter Pan No")]
        public string Pan_No { get; set; }

        public Nullable<double> Royalty_Charges { get; set; }
        public Nullable<double> D_Docket { get; set; }
        public Nullable<double> P_Docket { get; set; }
        public Nullable<double> E_Docket { get; set; }
        public Nullable<double> V_Docket { get; set; }
        public Nullable<double> I_Docket { get; set; }
        public Nullable<double> N_Docket { get; set; }

        //[Required(ErrorMessage = "Please Enter Password")]
        //public string Password { get; set; }

        //[Required(ErrorMessage = "Please Enter Username")]
        //public string Username { get; set; }

        public Nullable<double> G_Docket { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Nondox> Nondoxes { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Nondox> Nondoxes1 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Priority> Priorities { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Ratem> Ratems { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Ratem> Ratems1 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<dtdcPlu> dtdcPlus { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Dtdc_Ptp> Dtdc_Ptp { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<express_cargo> express_cargo { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Transaction> Transactions { get; set; }
        public virtual Franchisee Franchisee { get; set; }
    }
}