using System.Web;
using System.Web.Mvc;

namespace Manager.WebApp
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            //filters.Add(new HandleErrorAttribute());
            filters.Add(new LogExceptionFilter());
            //filters.Add(new AccessRoleChecker());
        }
    }
}
