using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Z.Api.Interfaces;
using Z.Models;

namespace Z.BusinessLogic.Services.Interfaces
{
    public interface IModuleService
    {
        IZModule GetModule(int index);
        IZModule GetModule(string internalName);
        List<SuggestionData> GetSuggestionsFor(string text, KeywordData keyword, bool perfectMatchesOnly = false);
        void AddModule(IZModule helpModule);
        void NotifyClosing();

        int ModuleCount { get; }
        event EventHandler ModulesChanged;
    }
}
