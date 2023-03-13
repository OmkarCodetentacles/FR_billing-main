using DtDc_Billing.Entity_FR;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace DtDc_Billing.Models
{
    public class Receipt_DetailsModel
    {
        public long Receipt_Id { get; set; }
        [Display(Name = "Consignment No")]
        [Required]
        public string Consignment_No { get; set; }
        [Display(Name = "Destination")]
        [Required]
        public string Destination { get; set; }
        [Display(Name = "Sender Phone")]
        [Required]
        [MaxLength(10)]
        [RegularExpression("^[0-9]*$", ErrorMessage = "Phone no must be numeric")]
        public string sender_phone { get; set; }
        public string Sender_Email { get; set; }
        [Display(Name = "Sender Name")]
        public string Sender { get; set; }
        public string SenderCompany { get; set; }
        [Required]
        public string SenderAddress { get; set; }
        [Display(Name = "Sender City")]
        [Required]
        public string SenderCity { get; set; }

        public string SenderState { get; set; }
        [Display(Name = "Sender Pincode")]
        [Required]
        [MaxLength(12)]
        [MinLength(1)]
        [RegularExpression("^[0-9]*$", ErrorMessage = "Pincode must be numeric")]
        public string SenderPincode { get; set; }
        [Display(Name = "Receipents Phone")]
        [Required]
        [MaxLength(10)]
        [RegularExpression("^[0-9]*$", ErrorMessage = "Phone no must be numeric")]
        public string Reciepents_phone { get; set; }
        public string Reciepents_Email { get; set; }
        [Display(Name = "Receipents Name")]
        public string Reciepents { get; set; }
        public string ReciepentCompany { get; set; }
        [Required]
        public string ReciepentsAddress { get; set; }
        [Required]
        public string ReciepentsCity { get; set; }

        public string ReciepentsState { get; set; }
        [Display(Name = "Receipents  Pincode")]
        [Required]
        [MaxLength(12)]
        [MinLength(1)]
        [RegularExpression("^[0-9]*$", ErrorMessage = "Pincode must be numeric")]

        public string ReciepentsPincode { get; set; }
        public string Shipmenttype { get; set; }
        public Nullable<float> Shipment_Length { get; set; }

        public Nullable<int> Shipment_Quantity { get; set; }
        public Nullable<float> Shipment_Breadth { get; set; }
        public Nullable<float> Shipment_Heigth { get; set; }
        public Nullable<float> DivideBy { get; set; }
        public Nullable<int> TotalNo { get; set; }

        public Nullable<float> Actual_Weight { get; set; }
        public Nullable<float> volumetric_Weight { get; set; }
        public string DescriptionContent1 { get; set; }
        public string DescriptionContent2 { get; set; }
        public string DescriptionContent3 { get; set; }
        public Nullable<float> Amount1 { get; set; }
        public Nullable<float> Amount2 { get; set; }
        public Nullable<float> Amount3 { get; set; }

        public Nullable<float> Total_Amount { get; set; }
        public string Insurance { get; set; }
        public Nullable<float> Insuance_Percentage { get; set; }
        public Nullable<float> Insuance_Amount { get; set; }
        [Required]
        public float Charges_Amount { get; set; }
        public Nullable<float> Charges_Service { get; set; }
        public Nullable<float> Risk_Surcharge { get; set; }
        public Nullable<float> Service_Tax { get; set; }
        [Required]
        public Nullable<float> Charges_Total { get; set; }
        public string Cash { get; set; }
        public string Credit { get; set; }
        public Nullable<int> Credit_Amount { get; set; }
        public Nullable<bool> secure_Pack { get; set; }
        public Nullable<bool> Passport { get; set; }
        public Nullable<bool> OfficeSunday { get; set; }
        public string Shipment_Mode { get; set; }
        public Nullable<float> Addition_charge { get; set; }
        public string Addition_Lable { get; set; }
        public string Status { get; set; }
        public string Pf_Code { get; set; }
        public Nullable<long> User_Id { get; set; }
        public string CreateDateString { get; set; }
        public Nullable<float> Discount { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd MM yyyy}")]
        public Nullable<System.DateTime> Datetime_Cons { get; set; }
        public Nullable<float> Paid_Amount { get; set; }
        public string Token { get; set; }
        public string change_amt_reason { get; set; }
        public virtual User User { get; set; }
        public virtual Franchisee Franchisee { get; set; }

    }
}