using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Z.Api;
using Z.Api.Interfaces;
using Z.Models;

namespace Z.BusinessLogic.Services.Module
{
    public interface IModuleService : IEnumerable<IZModule>
    {
        IZModule GetModule(int index);
        IZModule GetModule(string internalName);
        List<SuggestionData> GetSuggestionsFor(string text, KeywordData keyword, bool perfectMatchesOnly = false);
        void AddModule(IZModule helpModule);

        int ModuleCount { get; }
    }
}
