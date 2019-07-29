using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Manager.SharedLib.Extensions
{
    public class SortingElement
    {
        public int id { get; set; }
        public int SortOrder { get; set; }
        public int ParentId { get; set; }

        public List<SortingElement> children { get; set; }
    }
}
