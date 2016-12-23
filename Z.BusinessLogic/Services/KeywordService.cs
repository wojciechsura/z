﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Z.Api;
using Z.Api.Interfaces;
using Z.Api.Types;
using Z.BusinessLogic.Services.Interfaces;
using Z.Models;

namespace Z.BusinessLogic.Services
{
    class KeywordService : IKeywordService
    {
        private class InternalKeywordData
        {
            public KeywordInfo Info { get; internal set; }
            public IZModule Module { get; set; }
            public string Keyword { get; set; }
        }

        // Private fields -----------------------------------------------------

        private readonly IModuleService moduleService;
        private readonly IConfigurationService configurationService;
        private List<InternalKeywordData> keywords;

        // Private methods ----------------------------------------------------

        private List<InternalKeywordData> CollectOriginalKeywords()
        {
            List<InternalKeywordData> result = new List<InternalKeywordData>();

            for (int i = 0; i < moduleService.ModuleCount; i++)
            {
                var module = moduleService.GetModule(i);
                var keywordActions = module.GetKeywordActions();

                if (keywordActions == null) continue;

                result.AddRange(keywordActions
                    .Select(action => new InternalKeywordData
                        {
                            Info = action,
                            Keyword = action.DefaultKeyword,
                            Module = module
                        }));
            }

            return result;
        }

        private void ApplyKeywordOverrides(List<InternalKeywordData> keywords)
        {
            foreach (var keywordOverride in configurationService.Configuration.Keywords.KeywordOverrides)
            {
                var data = keywords
                    .FirstOrDefault(k => k.Module.Name == keywordOverride.ModuleName && k.Info.Name == keywordOverride.ActionName);

                if (data != null)
                    data.Keyword = keywordOverride.Keyword;
            }
        }

        private void HandleConfigurationChanged(object sender, EventArgs e)
        {
            ReloadKeywords();
        }

        private void HandleModulesChanged(object sender, EventArgs e)
        {
            ReloadKeywords();
        }

        private void ReloadKeywords()
        {
            keywords = CollectOriginalKeywords();
            ApplyKeywordOverrides(keywords);
        }

        // Public methods -----------------------------------------------------

        public KeywordService(IModuleService moduleService, IConfigurationService configurationService)
        {
            this.moduleService = moduleService;
            this.configurationService = configurationService;

            moduleService.ModulesChanged += HandleModulesChanged;
            configurationService.ConfigurationChanged += HandleConfigurationChanged;

            ReloadKeywords();
        }

        public KeywordData GetKeywordAction(string keyword)
        {
            return keywords.Where(k => k.Keyword.ToLower() == keyword.ToLower())
                .Select(k => new KeywordData(k.Keyword, k.Info.Name, k.Info.DisplayName, k.Info.Comment, k.Module))
                .FirstOrDefault();
        }

        public IEnumerable<KeywordData> GetKeywords()
        {
            return keywords
                .Select(k => new KeywordData(k.Keyword, k.Info.Name, k.Info.DisplayName, k.Info.Comment, k.Module))
                .ToList();
        }
    }
}
