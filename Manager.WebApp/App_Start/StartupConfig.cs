using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using System.Web.Http;
using System.Net.Http.Formatting;

using System.Reflection;

using Microsoft.Owin;
using Owin;

using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security.Cookies;

using Microsoft.Owin.FileSystems;

using Autofac;
using Autofac.Integration.Owin;
using Autofac.Integration.WebApi;
using Autofac.Integration.Mvc;

using Newtonsoft.Json.Serialization;

using Manager.SharedLib.Logging;
using Manager.WebApp.Models;

using Manager.SharedLib.Caching;
using Manager.Business;

namespace Manager.WebApp
{
    public partial class Startup
    {
        private readonly ILog logger = LogProvider.For<Startup>();
        //private IContainer _container;

        public void ConfigureForOwinStartup(IAppBuilder app)
        {
            //Apply log4net configuration
            ConfigureLog4net();

            //try
            //{
            //    CacheProviderManager.Initialize();
            //}
            //catch (Exception ex)
            //{

            //    logger.Error("Cache initializing error: " + ex.InnerException.ToString());
            //}

            logger.Info("======================================================");
            logger.Info("Manager.WebApp is starting up");
            logger.Info("======================================================");


            //Register WebApi
            // The “HttpConfiguration” object is used to configure API routes, 
            // so we’ll pass this object to method “Register” in “WebApiConfig” class.
            var config = new HttpConfiguration();
            //ConfigureWebApi(config);


            //Configure Autofac for WebApi + WebMvc + Owin
            ConfigureAutofac(app, config);


            //Apply authentication configuration
            ConfigureAuth(app);

            logger.Info("======================================================");
            logger.Info("Manager.WebApp is successfully configured");
            logger.Info("======================================================");
        }

        #region Log4net

        private void ConfigureLog4net()
        {
            //log4net.Config.XmlConfigurator.Configure();
            //. = root, Web = your physical directory that contains all other static content, see prev step
            var physicalFileSystem = new PhysicalFileSystem(@".\App_Data\Log4net");
            IFileInfo log4netConfigFileInfo = null;
            physicalFileSystem.TryGetFileInfo("log4net.config", out log4netConfigFileInfo);
            if (log4netConfigFileInfo != null)
            {
                log4net.Config.XmlConfigurator.Configure(new System.IO.FileInfo(log4netConfigFileInfo.PhysicalPath));
            }
        }

        #endregion


        #region WebAPI


        private void ConfigureWebApi(HttpConfiguration config)
        {

            // Web API configuration and services

            // Web API routes
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );

            /*
            // Change JSON Provider to Newtonsoft.Json
            var jsonFormatter = config.Formatters.OfType<JsonMediaTypeFormatter>().First();
            jsonFormatter.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
             */
        }

        #endregion


        #region Autofac IOC

        private void ConfigureAutofac(IAppBuilder app, HttpConfiguration webApiConfig)
        {
            /*
            if (_container == null)
            {
                _container = InitializeAutofacContainer();              
            }
            */

            if (GlobalContainer.IocContainer == null)
            {
                GlobalContainer.IocContainer = InitializeAutofacContainer();
            }

            //Middlewares: Enabling DI for OWIN middleware: 
            //should be the first middleware added to IAppBuilder
            //app.UseAutofacMiddleware(GlobalContainer.IocContainer);

            //Extending the lifetime scope to MVC
            // Autofac MVC Integration -- http://alexmg.com/owin-support-for-the-web-api-2-and-mvc-5-integrations-in-autofac/
            app.UseAutofacMvc();

            //MVC: Set MVC DI resolver to use our Autofac container
            var dependencyResolver = new AutofacDependencyResolver(GlobalContainer.IocContainer);            
            System.Web.Mvc.DependencyResolver.SetResolver(dependencyResolver); //TODO: Still needed with OWIN?

            //WebAPI: Set the WebApi dependency resolver.
            //webApiConfig.DependencyResolver = new AutofacWebApiDependencyResolver(GlobalContainer.IocContainer);

            //Web.API enable lifetime scope created during the OWIN request to extend into WebAPI.
            //app.UseAutofacWebApi(webApiConfig); 
        }


        public static IContainer InitializeAutofacContainer()
        {
            ContainerBuilder builder = new ContainerBuilder();

            // Register all Autofact Modules (OWin, WebApi, WebMvc, ....)
            builder.RegisterAssemblyModules(Assembly.GetExecutingAssembly());

            return builder.Build();
        }

        #endregion



        #region Authentication

        // For more information on configuring authentication, please visit http://go.microsoft.com/fwlink/?LinkId=301864
        private void ConfigureAuth(IAppBuilder app)
        {
            // Configure the db context, user manager and role manager to use a single instance per request
            //app.CreatePerOwinContext(ApplicationDbContext.Create);
            app.CreatePerOwinContext<ApplicationUserManager>(ApplicationUserManager.Create);
            app.CreatePerOwinContext<ApplicationRoleManager>(ApplicationRoleManager.Create);


            // Enable the application to use a cookie to store information for the signed in user
            // and to use a cookie to temporarily store information about a user logging in with a third party login provider
            // Configure the sign in cookie
            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationType = DefaultAuthenticationTypes.ApplicationCookie,
                LoginPath = new PathString("/Account/Login"),
                Provider = new CookieAuthenticationProvider
                {
                    // Enables the application to validate the security stamp when the user logs in.
                    // This is a security feature which is used when you change a password or add an external login to your account.  
                    OnValidateIdentity = SecurityStampValidator.OnValidateIdentity<ApplicationUserManager, ApplicationUser>(
                        validateInterval: TimeSpan.FromMinutes(30),
                        regenerateIdentity: (manager, user) => user.GenerateUserIdentityAsync(manager))
                }
            });

            
            app.UseExternalSignInCookie(DefaultAuthenticationTypes.ExternalCookie);


            // Enables the application to temporarily store user information when they are verifying the second factor in the two-factor authentication process.
            app.UseTwoFactorSignInCookie(DefaultAuthenticationTypes.TwoFactorCookie, TimeSpan.FromMinutes(5));

            // Enables the application to remember the second login verification factor such as phone or email.
            // Once you check this option, your second step of verification during the login process will be remembered on the device where you logged in from.
            // This is similar to the RememberMe option when you log in.
            app.UseTwoFactorRememberBrowserCookie(DefaultAuthenticationTypes.TwoFactorRememberBrowserCookie);

            // Uncomment the following lines to enable logging in with third party login providers
            //app.UseMicrosoftAccountAuthentication(
            //    clientId: "",
            //    clientSecret: "");

            //app.UseTwitterAuthentication(
            //   consumerKey: "",
            //   consumerSecret: "");

            //app.UseFacebookAuthentication(
            //   appId: "",
            //   appSecret: "");

            //app.UseGoogleAuthentication();
        }

        #endregion

    }
}