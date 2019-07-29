using MsSql.AspNet.Identity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Manager.WebApp.Models
{
    public class ManageCompanyModel : CommonPagingModel
    {

        public static string DATETIME_FORMAT = "MM/dd/yyyy h:mm tt";
        public List<IdentityCompany> SearchResults { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public Boolean? IsEPE { get; set; }
        public int? Status { get; set; }
        public int CountryId { get; set; }
        public List<IdentityCountry> Countrys { get; set; }
        public int ProvinceId { get; set; }
        public List<IdentityProvince> Provinces { get; set; }
        public int DistrictId { get; set; }
        public List<IdentityDistrict> Districts { get; set; }

        public ManageCompanyModel()
        {
            Countrys = new List<IdentityCountry>();

            Provinces = new List<IdentityProvince>();

            Districts = new List<IdentityDistrict>();
        }

    }

    public class CompanyCommonInfoModel
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public Boolean IsEPE { get; set; }
        public int Status { get; set; }
        public int CountryId { get; set; }
        public List<IdentityCountry> Countrys { get; set; }
        public int ProvinceId { get; set; }
        public List<IdentityProvince> Provinces { get; set; }
        public int DistrictId { get; set; }
        public List<IdentityDistrict> Districts { get; set; }

        public CompanyCommonInfoModel()
        {
            Countrys = new List<IdentityCountry>();

            Provinces = new List<IdentityProvince>();

            Districts = new List<IdentityDistrict>();
        }

    }

    public class CompanyCreateModel : CompanyCommonInfoModel
    {
      

    }

    public class CompanyEditModel : CompanyCommonInfoModel
    {
        public int Id { get; set; }

    }


}