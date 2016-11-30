using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Z.Common.Interfaces;
using Z.Common.Types;
using Z.Models;
using Z.Services.Interfaces;

namespace Z.Services
{
    class KeywordService : IKeywordService
    {
        // Private types ------------------------------------------------------

        private class KeywordActionData
        {
            public KeywordActionInfo Info { get; set; }
            public string Keyword { get; set; }
            public IZModule Module { get; set; }
        }
        
        // Private fields -----------------------------------------------------

        private readonly IModuleService moduleService;
        private List<KeywordActionData> keywords;

        // Private methods ----------------------------------------------------

        private List<KeywordActionData> CollectOriginalKeywords()
        {
            List<KeywordActionData> result = new List<KeywordActionData>();

            for (int i = 0; i < moduleService.ModuleCount; i++)
            {
                var module = moduleService.GetModule(i);
                var keywordActions = module.GetKeywordActions();

                foreach (var action in keywordActions)
                {
                    KeywordActionData info = new KeywordActionData
                    {
                        Info = action,
                        Keyword = action.DefaultKeyword,
                        Module = module
                    };

                    result.Add(info);
                }
            }

            return result;
        }

        private void ApplyKeywordOverrides(List<KeywordActionData> keywords)
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

        public KeywordAction GetKeywordAction(string keyword)
        {
            return keywords.Where(k => k.Keyword.ToLower() == keyword.ToLower())
                .Select(k => new KeywordAction(k.Keyword, k.Info.ActionName, k.Info.DisplayName, k.Module))
                .FirstOrDefault();
        }
    }
}
