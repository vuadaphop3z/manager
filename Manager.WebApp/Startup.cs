using System;

using Owin;
using Microsoft.Owin;
using Autofac;
using System.Reflection;
using Microsoft.AspNet.SignalR.Client;
using Manager.WebApp.Settings;

[assembly: OwinStartup(typeof(Manager.WebApp.Startup))]
namespace Manager.WebApp
{
    public partial class Startup
    {
        private static IContainer _container;
        public static IContainer AutofacContainer { get { return _container; } }

        public void Configuration(IAppBuilder app)
        {
            //Initialize autofac container
            InitAutofacContainer(app);

            ConfigureForOwinStartup(app);
        }

        private void InitAutofacContainer(IAppBuilder app)
        {
            ContainerBuilder builder = new ContainerBuilder();

            // Register all Autofact Modules (OWin, WebApi, WebMvc, ....)
            builder.RegisterAssemblyModules(Assembly.GetExecutingAssembly());

            // You can  register all dependencies here, normally we define in modules

            //then
            _container = builder.Build();


            //Middlewares: Enabling DI for OWIN middleware: 
            //should be the first middleware added to IAppBuilder

            // Register the Autofac middleware FIRST. This also adds
            // Autofac-injected middleware registered with the container.
            //app.UseAutofacMiddleware(_container);

            // ...then register your other middleware not registered
            // with Autofac.
        }

        public sealed class SignalrConnection
        {
            // Because Singleton's constructor is private, we must explicitly
            // give the Lazy<Singleton> a delegate for creating the Singleton.
            private static readonly Lazy<SignalrConnection> instanceHolder =
                new Lazy<SignalrConnection>(() => new SignalrConnection());

            HubConnection myCon = null;
            IHubProxy myHub = null;
            bool connected = false;
            private SignalrConnection(string hubName = "ManagerHub")
            {
                InitSignalrConnection(hubName);

                myCon.Closed += () =>
                {
                    connected = false;
                    while (!connected)
                    {
                        System.Threading.Thread.Sleep(1000);

                        InitSignalrConnection(hubName);

                        myCon.Start().Wait();
                        connected = true;
                    }
                };

                myCon.Start().Wait();
            }

            private void InitSignalrConnection(string hubName = "ManagerHub")
            {
                var signalRServerUrl = SystemSettings.MyCloudServer;
                myCon = new HubConnection(signalRServerUrl);
                myHub = myCon.CreateHubProxy(hubName);
            }


            public HubConnection SinalrMyConnection { get { return myCon; } }
            public IHubProxy SignalrMyHub { get { return myHub; } }

            public static SignalrConnection Instance
            {
                get { return instanceHolder.Value; }
            }
        }

    }
}