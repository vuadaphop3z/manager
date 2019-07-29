using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MsSql.AspNet.Identity.Entities
{
    public class IdentityCategory : IdentityCommonCategory
    {
        public string UrlFriendly { get; set; }
        public List<IdentityCategoryLang> LangList { get; set; }

        public IdentityCategory()
        {
            LangList = new List<IdentityCategoryLang>();
        }
    }

    public class IdentityCategoryLang
    {
        public int Id { get; set; }
        public string LangCode { get; set; }
        public string Name { get; set; }
        public string UrlFriendly { get; set; }
        public int CategoryId { get; set; }
    }
}
