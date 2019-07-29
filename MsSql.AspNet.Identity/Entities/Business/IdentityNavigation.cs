using System;

namespace MsSql.AspNet.Identity.Entities
{
	public class IdentityNavigation : CommonIdentity
    {
	 	 public int Id { get; set; }
	 	 public int ParentId { get; set; }
	 	 public string Area { get; set; }
	 	 public string Name { get; set; }
	 	 public string Title { get; set; }
	 	 public string Desc { get; set; }
	 	 public string Action { get; set; }
	 	 public string Controller { get; set; }
	 	 public int Visible { get; set; }
	 	 public int Authenticate { get; set; }
	 	 public string CssClass { get; set; }
	 	 public int SortOrder { get; set; }
	 	 public string AbsoluteUri { get; set; }
	 	 public int Active { get; set; }
	 	 public string IconCss { get; set; }
	 }
}
