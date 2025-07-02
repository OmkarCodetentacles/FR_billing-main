using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DtDc_Billing.CustomModel
{
    public class receiptdetailsModel
    {
        public long Receipt_Id { get; set; }
        public string Consignment_No { get; set; }
        public string Destination { get; set; }
        public string sender_phone { get; set; }
        public string Sender_Email { get; set; }
        public string Sender { get; set; }
        public string SenderCompany { get; set; }
        public string SenderAddress { get; set; }
        public string SenderCity { get; set; }
        public string SenderState { get; set; }
        public string SenderPincode { get; set; }
        public string Reciepents_phone { get; set; }
        public string Reciepents_Email { get; set; }
        public string Reciepents { get; set; }
        public string ReciepentCompany { get; set; }
        public string ReciepentsAddress { get; set; }
        public string ReciepentsCity { get; set; }
        public string ReciepentsState { get; set; }
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
        public float Charges_Amount { get; set; }
        public Nullable<float> Charges_Service { get; set; }
        public Nullable<float> Risk_Surcharge { get; set; }
        public Nullable<float> Service_Tax { get; set; }
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
        public Nullable<float> Discount { get; set; }
        public string Pf_Code { get; set; }
        public Nullable<long> User_Id { get; set; }
        public Nullable<System.DateTime> Datetime_Cons { get; set; }
        public Nullable<float> Paid_Amount { get; set; }

        public string CreateDateString { get; set; }
    }
}