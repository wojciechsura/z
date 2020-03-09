using Microsoft.Practices.Unity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Z.BusinessLogic.Services.Application;
using Z.BusinessLogic.Services.AppWindows;
using Z.Services;

namespace Z.Dependencies
{
    public static class Configuration
    {
        public static void Configure(App app)
        {
            Container.Instance.RegisterInstance<IApplicationController>(app);
            Container.Instance.RegisterType<IAppWindowService, AppWindowService>(new ContainerControlledLifetimeManager());

            BusinessLogic.Dependencies.Configuration.Configure(Container.Instance);
        }

        public static void Dispose()
        {
            Container.Dispose();
        }

        internal static void LateConfigure()
        {
            BusinessLogic.Dependencies.Configuration.LateConfigure(Container.Instance);
        }
    }
}
