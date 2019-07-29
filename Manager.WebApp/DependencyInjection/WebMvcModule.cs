using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using Autofac;
using Autofac.Integration.Mvc;
using StackExchange.Redis.Extensions.Core;
using Manager.SharedLib.Caching;
using Manager.SharedLib.Caching.Providers;
using StackExchange.Redis.Extensions.Newtonsoft;

namespace Manager.WebApp.DependencyInjection
{
    public class WebMvcModule : Autofac.Module
    {
        protected override void Load(Autofac.ContainerBuilder builder)
        {
            // Register MVC controllers using assembly scanning.
            builder.RegisterControllers(System.Reflection.Assembly.GetExecutingAssembly());

            // Dependency injection module that registers abstractions for common web application properties. 
            // Ref: http://autofac.org/apidoc/html/DA6737B.htm
            builder.RegisterModule(new AutofacWebTypesModule());
            builder.RegisterFilterProvider();

            //Injecting the Redis client service
            builder.RegisterType<NewtonsoftSerializer>().As<ISerializer>().SingleInstance();
            builder.RegisterType<StackExchangeRedisCacheClient>().As<ICacheClient>().SingleInstance();

            builder.Register(c => CacheProviderManager.Default).As<ICacheProvider>().SingleInstance();
        }
    }
}