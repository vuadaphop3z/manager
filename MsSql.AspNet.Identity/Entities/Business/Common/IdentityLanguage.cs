using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MsSql.AspNet.Identity.Entities
{
    public class IdentityLanguage : CommonIdentity
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string LangCode { get; set; }
        public int Status { get; set; }
    }
}
