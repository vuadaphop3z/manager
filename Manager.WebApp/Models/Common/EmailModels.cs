using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Manager.WebApp.Models
{
    public class EmailSenderModel
    {
        public string Email { get; set; }
        public string Body { get; set; }
    }

    public class EmailBookingConfirmModel
    {
        public string Receiver { get; set; }        
    }
}