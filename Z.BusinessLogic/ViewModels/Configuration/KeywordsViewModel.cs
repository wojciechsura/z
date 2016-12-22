using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Z.BusinessLogic.Services.Interfaces;
using Z.Api.Interfaces;
using Z.Api.Types;
using Z.BusinessLogic.ViewModels.Configuration.Keywords;
using Z.Models.Configuration;

namespace Z.BusinessLogic.ViewModels.Configuration
{
    public class KeywordsViewModel : BaseConfigurationViewModel
    {
        public const string PAGE_DISPLAY_NAME = "Keywords";

        private readonly IConfigurationService configurationService;
        private readonly IKeywordService keywordService;
        private readonly IModuleService moduleService;

        private readonly List<KeywordOverrideViewModel> keywordOverrides;

        public KeywordsViewModel(IConfigurationService configurationService, IKeywordService keywordService, IModuleService moduleService)
        {
            this.configurationService = configurationService;
            this.keywordService = keywordService;
            this.moduleService = moduleService;

            keywordOverrides = new List<KeywordOverrideViewModel>();

            for (int i = 0; i < moduleService.ModuleCount; i++)
            {
                var module = moduleService.GetModule(i);
                var keywords = module.GetKeywordActions();
                if (keywords != null)
                    foreach (var keywordInfo in keywords)
                    {
                        var @override = configurationService.Configuration.Keywords.KeywordOverrides
                            .FirstOrDefault(o => o.ModuleName == module.Name && o.ActionName == keywordInfo.Name);

                        keywordOverrides.Add(new KeywordOverrideViewModel(keywordInfo.DefaultKeyword,
                            @override != null ? @override.Keyword : keywordInfo.DefaultKeyword,
                            module.Name,
                            keywordInfo.Name,
                            module.DisplayName,
                            keywordInfo.DisplayName,
                            module.Icon));
                    }
            }
        }

        public override void Save()
        {
            configurationService.Configuration.Keywords.KeywordOverrides.Clear();

            foreach (var @override in keywordOverrides)
            {
                if (!@override.Override) continue;

                var keywordOverride = new KeywordOverride
                {
                    ActionName = @override.ActionName,
                    ModuleName = @override.ModuleName,
                    Keyword = @override.Keyword
                };
                configurationService.Configuration.Keywords.KeywordOverrides.Add(keywordOverride);
            }
        }

        public IEnumerable<KeywordOverrideViewModel> Keywords => keywordOverrides;

        public override string DisplayName => PAGE_DISPLAY_NAME;
    }
}
