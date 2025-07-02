using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DtDc_Billing.CustomModel
{
    public class RedirectException : Exception
    {
        public string ErrorMessage { get; }

        public RedirectException(string message)
        {
            ErrorMessage = message;
        }
    }

}