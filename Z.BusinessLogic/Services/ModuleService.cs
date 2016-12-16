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
    class ModuleService : IModuleService
    {
        private class SuggestionCollector : ISuggestionCollector, IDisposable
        {
            private readonly List<SuggestionData> suggestions;
            private readonly IZModule currentModule;
            private bool disposed = false;

            public SuggestionCollector(List<SuggestionData> suggestions, IZModule currentModule)
            {
                this.suggestions = suggestions;
                this.currentModule = currentModule;
            }

            public void AddSuggestion(SuggestionInfo suggestion)
            {
                if (disposed)
                    throw new InvalidOperationException("Do not store this class - use only when passed through a parameter!");
                if (suggestion == null)
                    throw new ArgumentNullException(nameof(suggestion));

                suggestions.Add(new SuggestionData(suggestion, currentModule));
            }

            public void Dispose()
            {
                this.disposed = true;
            }
        }

        private List<IZModule> modules;

        private void InitDefaultModules()
        {
            modules.Add(new WebSearchModule.Module());
            modules.Add(new PgsModule.Module());
            modules.Add(new Filesystem.Module());
            modules.Add(new ControlPanelModule.Module());
            modules.Add(new StartMenuModule.Module());
            modules.Add(new ProCalcModule.Module());
        }

        public ModuleService()
        {
            modules = new List<IZModule>();
            InitDefaultModules();
        }

        public IZModule GetModule(int index)
        {
            return modules[index];
        }

        public IZModule GetModule(string internalName)
        {
            return modules
                .SingleOrDefault(m => m.InternalName == internalName);
        }

        public List<SuggestionData> GetSuggestionsFor(string text, KeywordData keyword)
        {
            var suggestions = new List<SuggestionData>();

            if (keyword != null)
            {
                using (var collector = new SuggestionCollector(suggestions, keyword.Module))
                {
                    keyword.Module.CollectSuggestions(text, keyword.ActionName, collector);
                }
            }
            else
            {
                for (int i = 0; i < modules.Count; i++)
                {
                    IZModule module = modules[i];

                    using (var collector = new SuggestionCollector(suggestions, module))
                    {
                        module.CollectSuggestions(text, null, collector);
                    }
                }
            }

            return suggestions;
        }

        public int ModuleCount
        {
            get
            {
                return modules.Count;
            }
        }
    }
}
