using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


using Autofac;
using Manager.WebApp.Services;

namespace Manager.WebApp.DependencyInjection
{
    public class ServiceModule: Autofac.Module
    {
        protected override void Load(Autofac.ContainerBuilder builder)
        {
            const string ServiceBaseSuffix = "Service";
            Type serviceBaseType = typeof(ServiceBase);
            
            List<Type> serviceBaseTypes = serviceBaseType.Assembly.GetTypes().Where(type => type != serviceBaseType && serviceBaseType.IsAssignableFrom(type)).ToList();

            serviceBaseTypes.ForEach(serviceType =>
            {
                Type serviceInterface = serviceType.GetInterfaces().Where(x => x.Name.EndsWith(ServiceBaseSuffix, StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault();

                builder.RegisterType(serviceType).As(serviceInterface).InstancePerLifetimeScope();
            });
        }
    }
}