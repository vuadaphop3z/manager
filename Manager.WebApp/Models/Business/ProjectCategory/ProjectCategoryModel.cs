using MsSql.AspNet.Identity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Manager.WebApp.Models
{
    public class ManageProjectCategoryModel : CommonPagingModel
    {
        public static string DATETIME_FORMAT = "MM/dd/yyyy h:mm tt";
        public List<IdentityProjectCategory> SearchResults { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public DateTime CreatedDate { get; set; }
        public int? Status { get; set; }

    }

    public class ProjectCategoryInfoModel
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public DateTime CreatedDate { get; set; }
        public int Status { get; set; }
    }

    public class ProjectCategoryCreateModel : ProjectCategoryInfoModel
    {

    }
    public class ProjectCategoryEditMode : ProjectCategoryInfoModel
    {
        public int Id { get; set; }
    }
}