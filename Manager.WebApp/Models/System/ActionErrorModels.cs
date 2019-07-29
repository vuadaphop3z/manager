using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Manager.WebApp.Models
{
    public class ActionError
    {
        public string Reason { get; set; }
        public string Details { get; set; }
    }

    public class ActionStatus
    {
        public int StatusCode { get; set; }
        public string Message { get; set; }
        public string Details { get; set; }
        public List<ActionError> Errors { get; set; }
        public dynamic Data { get; set; }
    }
}