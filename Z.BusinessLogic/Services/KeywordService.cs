using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Z.Api.Interfaces;
using Z.Api.Types;
using Z.BusinessLogic.Services.Interfaces;
using Z.Models;

namespace Z.BusinessLogic.Services
{
    class KeywordService : IKeywordService
    {
        // Private types ------------------------------------------------------

        private class InternalKeywordData
        {
            public KeywordInfo Info { get; set; }
            public string Keyword { get; set; }
            public IZModule Module { get; set; }
        }
        
        // Private fields -----------------------------------------------------

        private readonly IModuleService moduleService;
        private List<InternalKeywordData> keywords;

        // Private methods ----------------------------------------------------

        private List<InternalKeywordData> CollectOriginalKeywords()
        {
            List<InternalKeywordData> result = new List<InternalKeywordData>();

            for (int i = 0; i < moduleService.ModuleCount; i++)
            {
                var module = moduleService.GetModule(i);
                var keywordActions = module.GetKeywordActions();

                foreach (var action in keywordActions)
                {
                    InternalKeywordData info = new InternalKeywordData
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

        private void ApplyKeywordOverrides(List<InternalKeywordData> keywords)
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

        public KeywordData GetKeywordAction(string keyword)
        {
            return keywords.Where(k => k.Keyword.ToLower() == keyword.ToLower())
                .Select(k => new KeywordData(k.Keyword, k.Info.ActionName, k.Info.DisplayName, k.Module))
                .FirstOrDefault();
        }
    }
}
