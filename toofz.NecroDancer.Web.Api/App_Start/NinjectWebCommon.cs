using System;
using System.Linq;
using System.Web.Http.ExceptionHandling;
using Microsoft.ApplicationInsights;
using Microsoft.Web.Infrastructure.DynamicModuleHelper;
using Ninject;
using Ninject.Activation;
using Ninject.Web.Common;
using Ninject.Web.Common.WebHost;
using toofz.NecroDancer.Leaderboards;
using toofz.NecroDancer.Web.Api;
using toofz.NecroDancer.Web.Api.Infrastructure;
using WebActivatorEx;

[assembly: PreApplicationStartMethod(typeof(NinjectWebCommon), nameof(NinjectWebCommon.Start))]
[assembly: ApplicationShutdownMethod(typeof(NinjectWebCommon), nameof(NinjectWebCommon.Stop))]

namespace toofz.NecroDancer.Web.Api
{
    internal static class NinjectWebCommon
    {
        private static readonly Bootstrapper bootstrapper = new Bootstrapper();

        /// <summary>
        /// Starts the application
        /// </summary>
        public static void Start()
        {
            DynamicModuleUtility.RegisterModule(typeof(OnePerRequestHttpModule));
            DynamicModuleUtility.RegisterModule(typeof(NinjectHttpModule));
            bootstrapper.Initialize(CreateKernel);
        }

        /// <summary>
        /// Stops the application.
        /// </summary>
        public static void Stop()
        {
            bootstrapper.ShutDown();
        }

        /// <summary>
        /// Creates the kernel that will manage your application.
        /// </summary>
        /// <returns>The created kernel.</returns>
        private static IKernel CreateKernel()
        {
            var kernel = new StandardKernel();
            try
            {
                kernel.Bind<Func<IKernel>>().ToMethod(ctx => () => new Bootstrapper().Kernel);
                kernel.Bind<System.Web.IHttpModule>().To<HttpApplicationInitializationHttpModule>();

                RegisterServices(kernel);
                return kernel;
            }
            catch
            {
                kernel.Dispose();
                throw;
            }
        }

        /// <summary>
        /// Load your modules or register your services here!
        /// </summary>
        /// <param name="kernel">The kernel.</param>
        internal static void RegisterServices(IKernel kernel)
        {
            kernel.Bind<TelemetryClient>()
                  .ToConstant(WebApiApplication.TelemetryClient);

            kernel.Bind<IExceptionLogger>()
                  .To<AiExceptionLogger>();

            kernel.Bind<string>()
                  .ToMethod(GetNecroDancerContextConnectionString)
                  .WhenInjectedInto<NecroDancerContext>();
            kernel.Bind<INecroDancerContext>()
                  .To<NecroDancerContext>();

            kernel.Bind<string>()
                  .ToMethod(GetLeaderboardsContextConnectionString)
                  .WhenInjectedInto<LeaderboardsContext>();
            kernel.Bind<ILeaderboardsContext>()
                  .To<LeaderboardsContext>();

            kernel.Bind<ProductsBinder>()
                  .ToMethod(GetProductsBinder);
            kernel.Bind<ModesBinder>()
                  .ToMethod(GetModesBinder);
            kernel.Bind<RunsBinder>()
                  .ToMethod(GetRunsBinder);
            kernel.Bind<CharactersBinder>()
                  .ToMethod(GetCharactersBinder);
        }

        private static string GetNecroDancerContextConnectionString(IContext c)
        {
            return Helper.GetDatabaseConnectionString(nameof(NecroDancerContext));
        }

        private static string GetLeaderboardsContextConnectionString(IContext c)
        {
            return Helper.GetDatabaseConnectionString(nameof(LeaderboardsContext));
        }

        private static ProductsBinder GetProductsBinder(IContext c)
        {
            using (var db = c.Kernel.Get<ILeaderboardsContext>())
            {
                var products = db.Products.Select(p => p.Name).ToList();

                return new ProductsBinder(products);
            }
        }

        private static ModesBinder GetModesBinder(IContext c)
        {
            using (var db = c.Kernel.Get<ILeaderboardsContext>())
            {
                var modes = db.Modes.Select(p => p.Name).ToList();

                return new ModesBinder(modes);
            }
        }

        private static RunsBinder GetRunsBinder(IContext c)
        {
            using (var db = c.Kernel.Get<ILeaderboardsContext>())
            {
                var runs = db.Runs.Select(p => p.Name).ToList();

                return new RunsBinder(runs);
            }
        }

        private static CharactersBinder GetCharactersBinder(IContext c)
        {
            using (var db = c.Kernel.Get<ILeaderboardsContext>())
            {
                var characters = db.Characters.Select(p => p.Name).ToList();

                return new CharactersBinder(characters);
            }
        }
    }
}
