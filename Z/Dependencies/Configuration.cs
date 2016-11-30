using Microsoft.Practices.Unity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Z.Services;
using Z.Services.Interfaces;

namespace Z.Dependencies
{
    public static class Configuration
    {
        private static UnityContainer container;

        public static void Configure()
        {
            container = new UnityContainer();

            container.RegisterType<IHotkeyService, HotkeyService>(new ContainerControlledLifetimeManager());
            container.RegisterType<IModuleService, ModuleService>(new ContainerControlledLifetimeManager());
            container.RegisterType<IKeywordService, KeywordService>(new ContainerControlledLifetimeManager());
        }

        public static void Dispose()
        {
            container.Dispose();
        }

        public static IUnityContainer Container
        {
            get
            {
                return container;
            }
        }
    }
}
