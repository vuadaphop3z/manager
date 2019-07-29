using System;
using System.Collections.Generic;
using Microsoft.AspNet.Identity;
using MsSql.AspNet.Identity.Entities;

namespace MsSql.AspNet.Identity
{
    public class IdentityAccessRoles 
    {
        public virtual string RoleId { get; set; }
        public virtual string AccessId { get; set; }
        public virtual int OperationId { get; set; }

        public virtual string RoleName { get; set; }
        public virtual string AccessName { get; set; }
        public virtual string AccessDescription { get; set; }

        public List<IdentityOperation> OperationsList { get; set; }
    }

    public class IdentityAccess
    {        
        public virtual string Id { get; set; }
        public virtual string AccessName { get; set; }
        public virtual bool Active { get; set; }
        public virtual string Description { get; set; }

        public List<IdentityAccessRoles> RolesList { get; set; }

        public List<IdentityOperation> OperationsList { get; set; }
    }

    public class IdentityAccessList
    {
        public List<IdentityAccess> AccessList { get; set; }
    }

    public class IdentityPermission
    {
        public string Action { get; set; }
        public string Controller { get; set; }
    }

    //public class IdentityOperation
    //{
    //    //public virtual int Id { get; set; }
    //    //public virtual string OperationName { get; set; }
    //    //public virtual bool Enabled { get; set; }
    //    //public virtual string AccessId { get; set; }
    //    //public virtual string ActionName { get; set; }

    //    ////Extends
    //    //public virtual string AccessName { get; set; }
    //}
}
