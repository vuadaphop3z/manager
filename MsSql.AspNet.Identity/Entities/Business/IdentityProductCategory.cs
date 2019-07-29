using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MsSql.AspNet.Identity.Entities
{
    public class IdentityProductCategory : IdentityMaster
    {
        ////Extends information here
        //public string Icon { get; set; }
        //public int ParentId { get; set; }
        //public string UrlFriendly { get; set; }

        //public List<IdentityProductCategoryLang> LangList { get; set; }
        //public List<IdentityProductCategory> Childrens { get; set; }

        //public IdentityProductCategory()
        //{
        //    Childrens = new List<IdentityProductCategory>();
        //    LangList = new List<IdentityProductCategoryLang>();
        //}

        ////Extends
        //public string ParentName { get; set; }

        public List<IdentityProperty> Properties { get; set; }
        public IdentityProductCategory()
        {
            Properties = new List<IdentityProperty>();
        }
    }


    public class IdentityProductCategoryLang
    {
        public int Id { get; set; }
        public string LangCode { get; set; }
        public string Name { get; set; }
        public string UrlFriendly { get; set; }
        public int ProductCategoryId { get; set; }
        public string Description { get; set; }
    }
}
