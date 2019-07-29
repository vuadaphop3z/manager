using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MsSql.AspNet.Identity.Entities
{
    public class IdentityOperation
    {
        public int Id { get; set; }
        public string OperationName { get; set; }
        public bool Enabled { get; set; }
        public string AccessId { get; set; }
        public string ActionName { get; set; }

        public string AccessName { get; set; }
        public string ControllerName { get; set; }
    }
}
