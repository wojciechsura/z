using Microsoft.Practices.Unity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Z.BusinessLogic.Interfaces;
using Z.BusinessLogic.Interfaces.ViewModels;
using Z.BusinessLogic.Services;
using Z.BusinessLogic.Services.Interfaces;

namespace Z.BusinessLogic.Dependencies
{
    public static class Configuration
    {
        public static void Configure(IUnityContainer container)
        {
            container.RegisterType<IGlobalHotkeyService, GlobalHotkeyService>(new ContainerControlledLifetimeManager());
            container.RegisterType<IModuleService, ModuleService>(new ContainerControlledLifetimeManager());
            container.RegisterType<IKeywordService, KeywordService>(new ContainerControlledLifetimeManager());
            container.RegisterType<IConfigurationService, ConfigurationService>(new ContainerControlledLifetimeManager());

            container.RegisterType<MainLogic>(new ContainerControlledLifetimeManager());
            container.RegisterInstance<IMainWindowViewModel>(container.Resolve<MainLogic>().MainWindowViewModel);
            container.RegisterInstance<IListWindowViewModel>(container.Resolve<MainLogic>().ListWindowViewModel);

            container.RegisterType<ConfigurationLogic>(new ContainerControlledLifetimeManager());
            container.RegisterType<IConfigurationWindowViewModel>(new InjectionFactory(c => c.Resolve<ConfigurationLogic>().ConfigurationWindowViewModel));
        }
    }
}
