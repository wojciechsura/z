using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Z.Api;
using Z.Api.Interfaces;
using Z.Api.Types;
using Z.BusinessLogic.Events;
using Z.BusinessLogic.Services.Interfaces;
using Z.Models;

namespace Z.BusinessLogic.Services
{
    class KeywordService : IKeywordService, IEventListener<ConfigurationChangedEvent>, IEventListener<ModulesChangedEvent>
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
        private readonly IEventBus eventBus;
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

        private void HandleConfigurationChanged()
        {
            ReloadKeywords();
        }

        private void HandleModulesChanged()
        {
            ReloadKeywords();
        }

        private void ReloadKeywords()
        {
            keywords = CollectOriginalKeywords();
            ApplyKeywordOverrides(keywords);
        }

        // IEventListener implementation --------------------------------------

        void IEventListener<ConfigurationChangedEvent>.Receive(ConfigurationChangedEvent @event)
        {
            HandleConfigurationChanged();
        }

        void IEventListener<ModulesChangedEvent>.Receive(ModulesChangedEvent @event)
        {
            HandleModulesChanged();
        }

        // Public methods -----------------------------------------------------

        public KeywordService(IModuleService moduleService, IConfigurationService configurationService, IEventBus eventBus)
        {
            this.moduleService = moduleService;
            this.configurationService = configurationService;
            this.eventBus = eventBus;

            eventBus.Register((IEventListener<ConfigurationChangedEvent>)this);

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
