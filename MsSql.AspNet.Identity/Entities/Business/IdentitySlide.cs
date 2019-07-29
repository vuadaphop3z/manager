using System;
using System.Collections.Generic;

namespace MsSql.AspNet.Identity.Entities
{
    public class IdentitySlide : IdentityCommon
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Code { get; set; }

        public string SlideType { get; set; }

        public string CssClass { get; set; }

        public int DelayTime { get; set; }

        public string Configs { get; set; }

        public int Status { get; set; }

        public List<IdentitySlideItem> ListItems { get; set; }

        public IdentitySlide()
        {
            ListItems = new List<IdentitySlideItem>();
        }
    }

    public class IdentitySlideItem
    {
        public int Id { get; set; }

        public int SlideId { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public string ImageUrl { get; set; }

        public string Link { get; set; }

        public int LinkAction { get; set; }

        public int SortOrder { get; set; }

        public DateTime? CreatedDate { get; set; }

        public int Status { get; set; }

    }
}
