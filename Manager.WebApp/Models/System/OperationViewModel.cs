using MsSql.AspNet.Identity;
using MsSql.AspNet.Identity.Entities;
using System.Collections.Generic;

namespace Manager.WebApp.Models
{
    public class OperationViewModel
    {
        public OperationViewModel()
        {

        }

        public int OperationId { get; set; }

        public string OperationName { get; set; }      

        public string AccessId { get; set; }

        public string ActionName { get; set; }

        public List<IdentityAccess> AllAccess { get; set; }

        public List<IdentityOperation> AllOperations { get; set; }
    }    
}