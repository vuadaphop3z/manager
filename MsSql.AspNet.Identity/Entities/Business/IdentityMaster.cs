using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MsSql.AspNet.Identity.Entities
{
    public class IdentityMaster : CommonIdentity
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? LastUpdated { get; set; }
        public string LastUpdatedBy { get; set; }        
        public int Status { get; set; }
    }


    public class IdentityReviewType : IdentityMaster
    {
        //Extends information here
    }

    public class IdentityTravellerType : IdentityMaster
    {
        //Extends information here
    }

    public class IdentityPolicy : IdentityMaster
    {
        //Extends information here
    }

    public class IdentityUnit : IdentityMaster
    {
        //Extends information here
    }

    public class IdentityPropertyCategory : IdentityMaster
    {
        //Extends information here
        public List<IdentityProperty> Properties { get; set; }
        public IdentityPropertyCategory()
        {
            Properties = new List<IdentityProperty>();
        }
    }

    public class IdentityGroupProperty : IdentityMaster
    {
        //Extends information here
        public string Icon { get; set; }

        public string Description { get; set; }

        public List<IdentityGroupPropertyLang> LangList { get; set; }

        public IdentityGroupProperty()
        {
            LangList = new List<IdentityGroupPropertyLang>();
        }
    }

    public class IdentityGroupPropertyLang
    {
        public int Id { get; set; }
        public string LangCode { get; set; }
        public string GroupName { get; set; }
        public int GroupId { get; set; }
        public string Description { get; set; }
    }

    public class IdentityProperty : IdentityMaster
    {
        public string Icon { get; set; }

        public List<IdentityPropertyLang> LangList { get; set; }

        public int PropertyCategoryId { get; set; }

        public IdentityProperty()
        {
            LangList = new List<IdentityPropertyLang>();
        }
    }

    public class IdentityPropertyLang : IdentityMaster
    {
        //Extends information here
        public int PropertyId { get; set; }
        public string LangCode { get; set; }
    }
    
    public class IdentityPriceType : IdentityMaster
    {
        //Extends information here
        public string Icon { get; set; }
        public string Description { get; set; }
        public List<IdentityPriceTypeLang> LangList { get; set; }

        public IdentityPriceType()
        {
            LangList = new List<IdentityPriceTypeLang>();
        }
    }

    public class IdentityPriceTypeLang
    {
        public int Id { get; set; }
        public string LangCode { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int PriceTypeId { get; set; }
    }

    public class IdentityReviewCategory : IdentityMaster
    {
        //Extends information here
        public string Icon { get; set; }
    }

    public class IdentityCurrency : IdentityMaster
    {
        //Extends information here
        public string Icon { get; set; }
    }

    public class IdentityCredit : IdentityMaster
    {
        //Extends information here
        public string Icon { get; set; }
    }
    public class IdentityPayment : IdentityMaster
    {
        //Extends information here
        public string Icon { get; set; }
    }

    public class IdentityDevice : IdentityMaster
    {
        //Extends information here
    }

    public class IdentityHTDefaultSetting : IdentityMaster
    {
        //Extends information here
        public int EnumValue { get; set; }
        public int MaxLength { get; set; }
        public int StartPosition { get; set; }
        public int NumberOfCharacters { get; set; }
    }
}
