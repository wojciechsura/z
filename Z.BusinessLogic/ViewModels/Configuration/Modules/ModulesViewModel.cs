﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Z.Api;
using Z.Api.Interfaces;
using Z.BusinessLogic.Services.Module;
using Z.BusinessLogic.ViewModels.Configuration.Base;
using Z.BusinessLogic.ViewModels.Configuration.Modules;
using Z.Resources;

namespace Z.BusinessLogic.ViewModels.Configuration.Modules
{
    public class ModulesViewModel : BaseConfigurationViewModel, INotifyPropertyChanged, IDisposable
    {
        public class ConfigProviderInfo
        {
            public ConfigProviderInfo(IZModule module, IConfigurationProvider configProvider)
            {
                Module = module;
                ConfigProvider = configProvider;
            }

            public IZModule Module { get; private set; }
            public IConfigurationProvider ConfigProvider { get; private set; }
        }

        private readonly List<ConfigProviderInfo> configProviders;
        private readonly IModuleService moduleService;

        private readonly List<ModuleConfigViewModel> modules;

        public override void Save()
        {
            foreach (var provider in configProviders)
            {
                provider.ConfigProvider.Save();
            }
        }

        public override IEnumerable<string> Validate()
        {
            return configProviders.SelectMany(
                cp => cp.ConfigProvider.Validate() ?? new string[] { }
                    .Select(m => string.Format(Strings.Z_Message_ModuleReportsError, cp.Module.DisplayName, m)));
        }

        public void Dispose()
        {
            foreach (var providerInfo in configProviders)
            {
                providerInfo.ConfigProvider.Dispose();
            }
        }

        public ModulesViewModel(IModuleService moduleService)
        {
            this.moduleService = moduleService;

            configProviders = new List<ConfigProviderInfo>();

            for (int i = 0; i < moduleService.ModuleCount; i++)
            {
                IZModule module = moduleService.GetModule(i);
                IZConfigurable configurableModule = module as IZConfigurable;
                if (configurableModule != null)
                {
                    var provider = configurableModule.GetConfigurationProvider();
                    if (provider != null)
                        configProviders.Add(new ConfigProviderInfo(moduleService.GetModule(i), provider));
                }
            }

            modules = configProviders
                .Select(cp => new ModuleConfigViewModel(cp.Module.DisplayName, cp.Module.Icon, cp.ConfigProvider))
                .ToList();
        }

        public override string DisplayName
        {
            get
            {
                return Strings.Z_ConfigurationPage_Modules;
            }
        }

        public IEnumerable<ModuleConfigViewModel> Modules => modules;
    }
}
