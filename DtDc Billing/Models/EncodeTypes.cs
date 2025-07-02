using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace DtDc_Billing.Models
{
    public enum BarcodeType
    {
        Code128,
        Code11,
        Code32,
        QR,
        Datamatrix,
        EAN13,
        EAN8,
        ITF14,
        PDF417
    }
   
}