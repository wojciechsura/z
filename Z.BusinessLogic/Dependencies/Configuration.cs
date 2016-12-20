using Microsoft.Practices.Unity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Z.BusinessLogic.ViewModels;
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

            container.RegisterType<MainViewModel>(new ContainerControlledLifetimeManager());            
            container.RegisterType<ConfigurationViewModel>();
            container.RegisterType<ViewModels.Configuration.GeneralViewModel>();
            container.RegisterType<ViewModels.Configuration.BehaviorViewModel>();
            container.RegisterType<ViewModels.Configuration.KeywordsViewModel>();
        }
    }
}
