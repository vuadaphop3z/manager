using MsSql.AspNet.Identity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Manager.WebApp.Models
{
    public class ManageProjectModel : CommonPagingModel
    {
        public static string DATETIME_FORMAT = "MM/dd/yyyy h:mm tt";
        public List<IdentityProject> SearchResults { get; set; }
        public string Code { get; set; }
        public int ProjectCategoryId { get; set; }
        public int CompanyId { get; set; }
        public string Name { get; set; }
        public string ShortDescription { get; set; }
        public string Detail { get; set; }
        public DateTime? BeginDate { get; set; }
        public DateTime? FinishDate { get; set; }
        public int CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        //public int LastUpdatedBy { get; set; }
        //public DateTime? LastUpdated { get; set; }
        public int? Status { get; set; }

        public List<IdentityCompany> Companys { get; set; }
        public List<IdentityProjectCategory> ProjectCategorys { get; set; }
        public ManageProjectModel()
        {
            Companys = new List<IdentityCompany>();
            ProjectCategorys = new List<IdentityProjectCategory>();
        }


    }

    public class ProjectCommonInfoModel 
    {
        public string Code { get; set; }
        public int ProjectCategoryId { get; set; }
        public int CompanyId { get; set; }
        public string Name { get; set; }
        public string ShortDescription { get; set; }
        public string Detail { get; set; }
        //public string BeginDate { get; set; }
        //public string FinishDate { get; set; } 
       
       
        public DateTime? BeginDate { get; set; }
        public DateTime? FinishDate { get; set; }
        public int CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        //public int LastUpdatedBy { get; set; }
        //public DateTime? LastUpdated { get; set; }
        public int Status { get; set; }

        public List<IdentityProjectCategory> ProjectCategorys { get; set; }
        public List<IdentityCompany> Companys { get; set; }

        public ProjectCommonInfoModel()
        {
            ProjectCategorys = new List<IdentityProjectCategory>();

            Companys = new List<IdentityCompany>();
        }

    }

    public class ProjectCreateModel : ProjectCommonInfoModel
    {

    }

    public class ProjectEditModel : ProjectCommonInfoModel
    {
        public int Id { get; set; }
    }
}