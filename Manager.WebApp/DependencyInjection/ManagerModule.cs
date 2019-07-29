using Autofac;
using MsSql.AspNet.Identity;
using MsSql.AspNet.Identity.MsSqlStores;
using MsSql.AspNet.Identity.MsSqlStores.Business;
using MsSql.AspNet.Identity.Stores;

namespace Manager.WebApp.DependencyInjection
{
    public class ManagerModule : Autofac.Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            //For system
            builder.RegisterType<IdentityStore>().As<IIdentityStore>();

            //For business
            builder.RegisterType<StoreProvider>().As<IStoreProvider>();
            builder.RegisterType<StoreArea>().As<IStoreArea>();

            builder.RegisterType<StoreCategory>().As<IStoreCategory>();            
            //builder.RegisterType<StoreLanguage>().As<IStoreLanguage>();
			builder.RegisterType<StoreProductCategory>().As<IStoreProductCategory>();
			builder.RegisterType<StoreProduct>().As<IStoreProduct>();
			builder.RegisterType<StoreWarehouse>().As<IStoreWarehouse>();

            //builder.RegisterType<StoreMember>().As<IStoreMember>();
            builder.RegisterType<StoreProperty>().As<IStoreProperty>();
            builder.RegisterType<StoreUnit>().As<IStoreUnit>();
            builder.RegisterType<StoreHTDefaultSetting>().As<IStoreHTDefaultSetting>();
            builder.RegisterType<StoreDevice>().As<IStoreDevice>();
            builder.RegisterType<StoreCurrency>().As<IStoreCurrency>();
            builder.RegisterType<StorePropertyCategory>().As<IStorePropertyCategory>();


            builder.RegisterType<StoreCompany>().As<IStoreCompany>();
            builder.RegisterType<StoreProjectCategory>().As<IStoreProjectCategory>();
            builder.RegisterType<StoreProject>().As<IStoreProject>();
            //builder.RegisterType<StoreArea>().As<IStoreArea>();


        }
    }
}