using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Ninject;
using Ninject.Web.Common;
using Ninject.Web.Common.WebHost;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Optimization;
using System.Web.Routing;
using System.Web.Security;
using System.Web.SessionState;

namespace WebServiceApplication
{
    //public class Global : HttpApplication
    //{
    //    void Application_Start(object sender, EventArgs e)
    //    {
    //        // Code that runs on application startup
    //        RouteConfig.RegisterRoutes(RouteTable.Routes);
    //        BundleConfig.RegisterBundles(BundleTable.Bundles);
    //    }
    //}

    public class Global : NinjectHttpApplication
    {
        protected override IKernel CreateKernel()
        {
            IKernel kernel = new StandardKernel();

            RegisterConfiguration(kernel);
            RegisterCache(kernel);
            //RegisterLogging(kernel);

            return kernel;
        }

        void RegisterConfiguration(IKernel kernel)
        {
            // Register IConfigurationRoot and build the configuration.
            kernel.Bind<IConfigurationRoot>().ToMethod(context => {
                return new ConfigurationBuilder()
                    // using the .config extension so IIS doesn't try to serve the file as static content.
                    .AddJsonFile("appsettings.json.config", optional: true)
                    .Build();
            });

            // Register Options. This allows us to use the IOptions binding feature
            // and is required by the distributed caching implementations.
            kernel.Bind(typeof(IOptions<>)).To(typeof(OptionsManager<>)).InSingletonScope();
            kernel.Bind(typeof(IOptionsSnapshot<>)).To(typeof(OptionsManager<>)).InRequestScope();
            kernel.Bind(typeof(IOptionsMonitor<>)).To(typeof(OptionsMonitor<>)).InSingletonScope();
            kernel.Bind(typeof(IOptionsFactory<>)).To(typeof(OptionsFactory<>));
            kernel.Bind(typeof(IOptionsMonitorCache<>)).To(typeof(OptionsCache<>)).InSingletonScope();
        }

        void RegisterCache(IKernel kernel)
        {
            // For demo purposes, we'll just us the memory cache.
            // You could also use the Redis or SQL Server implementations just as easily.
            kernel.Bind<IDistributedCache>().To<MemoryDistributedCache>().InSingletonScope();
        }

        //void RegisterLogging(IKernel kernel)
        //{
        //    // Register and add providers to the factory
        //    kernel.Bind<ILoggerFactory>().ToMethod(context => {
        //        return new LoggerFactory().AddDebug(minLevel: LogLevel.Debug);
        //    }).InSingletonScope();

        //    // Logger<T> takes ILoggerFactory into it's constructor
        //    kernel.Bind(typeof(ILogger<>)).To(typeof(Logger<>)).InSingletonScope();
        //}

    }
}