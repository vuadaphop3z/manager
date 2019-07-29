using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MsSql.AspNet.Identity.Entities
{
    public class IdentityPlace : IdentityCommon
    {
        public long Id { get; set; }

        public string GName { get; set; }

        public string GFullName { get; set; }

        public string GType { get; set; }

        public string GPlaceId { get; set; }

        public string GId { get; set; }

        public string GLat { get; set; }

        public string GLong { get; set; }

        public string GUrl { get; set; }

        public string Icon { get; set; }

        public string RawData { get; set; }

        public int Status { get; set; }

        //Extends
        public int SubPlacesCount { get; set; }

        public int PostCount { get; set; }

        public int PostCountProvince { get; set; }

        public string Province { get; set; }

        public string District { get; set; }

        public int ProvinceId { get; set; }

        public int DistrictId { get; set; }

        public bool IsProvince { get; set; }

        public string Cover { get; set; }
    }

    public class IdentityPlaceTypeGroup : IdentityCommon
    {
        public int Id { get; set; }
        public string GroupCode { get; set; }
        public string GroupName { get; set; }
        public bool FilterOnMap { get; set; }
        public string Icon { get; set; }
        public int SortOrder { get; set; }
        public int Status { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string CreatedBy { get; set; }

        public string CultureName { get; set; }
        public int PostCount { get; set; }

        public List<IdentityPlaceTypeGroupLang> LangList { get; set; }

        public IdentityPlaceTypeGroup()
        {
            LangList = new List<IdentityPlaceTypeGroupLang>();
        }
    }

    public class IdentityPlaceTypeGroupLang
    {
        public int Id { get; set; }
        public string LangCode { get; set; }
        public string GroupName { get; set; }
        public int GroupId { get; set; }
    }

    public class IdentityPlaceType : IdentityCommon
    {
        public int Id { get; set; }
        public string GCode { get; set; }
        public string Name { get; set; }
        public string Note { get; set; }
        public string GroupCode { get; set; }
        public int Status { get; set; }
        public DateTime? CreatedDate { get; set; }
    }
}
