using Microsoft.Practices.Unity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Z.BusinessLogic.ViewModels;
using Z.BusinessLogic.Services;
using Z.BusinessLogic.Services.Interfaces;
using Z.BusinessLogic.ViewModels.Main;
using Z.BusinessLogic.ViewModels.ProCalc;
using Z.BusinessLogic.ViewModels.Application;
using Z.BusinessLogic.ViewModels.Configuration;

namespace Z.BusinessLogic.Dependencies
{
    public static class Configuration
    {
        public static void Configure(IUnityContainer container)
        {
            container.RegisterType<IGlobalHotkeyService, GlobalHotkeyService>(new ContainerControlledLifetimeManager());            
            container.RegisterType<IKeywordService, KeywordService>(new ContainerControlledLifetimeManager());
            container.RegisterType<IConfigurationService, ConfigurationService>(new ContainerControlledLifetimeManager());
            container.RegisterType<IPathService, PathService>(new ContainerControlledLifetimeManager());
            container.RegisterType<IEventBus, EventBus>(new ContainerControlledLifetimeManager());

            container.RegisterType<MainViewModel>(new ContainerControlledLifetimeManager());
            container.RegisterType<ProCalcViewModel>(new ContainerControlledLifetimeManager());
            container.RegisterType<AppViewModel>(new ContainerControlledLifetimeManager());
            container.RegisterType<ConfigurationViewModel>();
            container.RegisterType<ViewModels.Configuration.General.GeneralViewModel>();
            container.RegisterType<ViewModels.Configuration.Behavior.BehaviorViewModel>();
            container.RegisterType<ViewModels.Configuration.Keywords.KeywordsViewModel>();
            container.RegisterType<ViewModels.Configuration.Modules.ModulesViewModel>();
        }

        public static void LateConfigure(IUnityContainer container)
        {
            container.RegisterInstance<IModuleService>(container.Resolve<ModuleService>());
        }
    }
}
