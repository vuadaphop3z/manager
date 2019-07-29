
using Autofac;

namespace Manager.WebApp
{
    /// <summary>
    /// Instance of the Global Container... yes, a God class. It's the best way to do it, though.
    /// </summary>
    public static class GlobalContainer
    {
        public static IContainer IocContainer { get; set; }
    }
}
