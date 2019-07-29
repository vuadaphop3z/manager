using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using Owin;
using Microsoft.Owin;

using Autofac;

namespace Manager.WebApp.DependencyInjection
{
    public class OwinModule: Autofac.Module
    {
        protected override void Load(ContainerBuilder builder)
        {           
            builder.Register(ctx => HttpContext.Current.GetOwinContext()).As<IOwinContext>();
        }
    }
}