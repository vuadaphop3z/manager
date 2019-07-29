using System;

using Autofac;
using MsSql.AspNet.Identity;

namespace Manager.WebApp.DependencyInjection
{
    public class AuthStoreModule : Autofac.Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            //builder.RegisterType<AuthNotificationStore>().As<IAuthNotificationStore>();
            //builder.RegisterType<AuthFingerprintStore>().As <IAuthFingerprintStore>();
            //builder.RegisterType<AuthMaxmindStore>().As<IAuthMaxmindStore>();
            //builder.RegisterType<AuthBigEventStore>().As<IAuthBigEventStore>();
        }
    }
}