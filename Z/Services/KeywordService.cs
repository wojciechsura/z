using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Z.Common.Interfaces;
using Z.Services.Interfaces;

namespace Z.Services
{
    class KeywordService : IKeywordService
    {
        // Private types ------------------------------------------------------

        private class KeywordInfo
        {
            public string DefaultKeyword { get; set; }
            public string Keyword { get; set; }
            public string DisplayName { get; set; }
            public string ActionName { get; set; }
            public IZModule Module { get; set; }
        }
        
        // Private fields -----------------------------------------------------

        private readonly IModuleService moduleService;
        private List<KeywordInfo> keywords;

        // Private methods ----------------------------------------------------

        private List<KeywordInfo> CollectOriginalKeywords()
        {
            List<KeywordInfo> result = new List<KeywordInfo>();

            for (int i = 0; i < moduleService.ModuleCount; i++)
            {
                var module = moduleService.GetModule(i);
                var keywordActions = module.GetKeywordActions();

                foreach (var action in keywordActions)
                {
                    KeywordInfo info = new KeywordInfo
                    {
                        ActionName = action.ActionName,
                        DefaultKeyword = action.DefaultKeyword,
                        DisplayName = action.DisplayName,
                        Keyword = action.DefaultKeyword,
                        Module = module
                    };

                    result.Add(info);
                }
            }

            return result;
        }

        private void ApplyKeywordOverrides(List<KeywordInfo> keywords)
        {
            // TODO
        }

        private void ReloadKeywords()
        {
            keywords = CollectOriginalKeywords();
            ApplyKeywordOverrides(keywords);
        }

        // Public methods -----------------------------------------------------

        public KeywordService(IModuleService moduleService)
        {
            this.moduleService = moduleService;

            ReloadKeywords();
        }
    }
}
