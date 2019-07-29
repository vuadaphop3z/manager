using System;
using System.Reflection;

using Autofac;
using Autofac.Integration.WebApi;

namespace Manager.WebApp.DependencyInjection
{
    public class WebApiModule : Autofac.Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            // Register API controllers using assembly scanning.            
            builder.RegisterApiControllers(Assembly.GetExecutingAssembly());
        }
    }
}