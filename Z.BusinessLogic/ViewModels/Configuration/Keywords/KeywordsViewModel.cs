using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Z.Api.Interfaces;
using Z.Api.Types;
using Z.BusinessLogic.ViewModels.Configuration.Keywords;
using Z.BusinessLogic.Models.Configuration;
using Z.Api;
using Z.BusinessLogic.ViewModels.Configuration.Base;
using Z.BusinessLogic.Services.Config;
using Z.BusinessLogic.Services.Keyword;
using Z.BusinessLogic.Services.Module;
using Z.Resources;

namespace Z.BusinessLogic.ViewModels.Configuration.Keywords
{
    public class KeywordsViewModel : BaseConfigurationViewModel
    {
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

        public override IEnumerable<string> Validate()
        {
            var duplicateOverrides = keywordOverrides
                .Select(k => k.Keyword.ToUpper())
                .GroupBy(k => k)
                .Where(g => g.Count() > 1);

            if (duplicateOverrides.Any())
                yield return $"The following keyword{(duplicateOverrides.Count() > 1 ? "s are" : " is")} duplicated: {String.Join(", ", duplicateOverrides.Select(d => d.Key))}";
        }

        public IEnumerable<KeywordOverrideViewModel> Keywords => keywordOverrides;

        public override string DisplayName => Strings.Z_ConfigurationPage_Keywords;
    }
}
