using Autofac;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Z.BusinessLogic.ViewModels;
using Z.BusinessLogic.Services;
using Z.BusinessLogic.ViewModels.Main;
using Z.BusinessLogic.ViewModels.ProCalc;
using Z.BusinessLogic.ViewModels.Application;
using Z.BusinessLogic.ViewModels.Configuration;
using Z.BusinessLogic.Services.GlobalHotkey;
using Z.BusinessLogic.Services.Keyword;
using Z.BusinessLogic.Services.Config;
using Z.BusinessLogic.Services.Paths;
using Z.BusinessLogic.Services.EventBus;
using Z.BusinessLogic.Services.Module;

namespace Z.BusinessLogic.Dependencies
{
    public static class Configuration
    {
        public static void Configure(ContainerBuilder container)
        {
            container.RegisterType<GlobalHotkeyService>().As<IGlobalHotkeyService>().SingleInstance();            
            container.RegisterType<KeywordService>().As<IKeywordService>().SingleInstance();
            container.RegisterType<ConfigurationService>().As<IConfigurationService>().SingleInstance();
            container.RegisterType<PathService>().As<IPathService>().SingleInstance();
            container.RegisterType<EventBus>().As<IEventBus>().SingleInstance();
            container.RegisterType<ModuleService>().As<IModuleService>().SingleInstance();

            container.RegisterType<MainViewModel>().As<MainViewModel>().SingleInstance();
            container.RegisterType<ProCalcViewModel>().As<ProCalcViewModel>().SingleInstance();
            container.RegisterType<AppViewModel>().As<AppViewModel>().SingleInstance();
            
            container.RegisterType<ConfigurationViewModel>();
            container.RegisterType<ViewModels.Configuration.General.GeneralViewModel>();
            container.RegisterType<ViewModels.Configuration.Behavior.BehaviorViewModel>();
            container.RegisterType<ViewModels.Configuration.Keywords.KeywordsViewModel>();
            container.RegisterType<ViewModels.Configuration.Modules.ModulesViewModel>();
            container.RegisterType<ViewModels.Configuration.Launcher.LauncherViewModel>();
        }
    }
}
