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
        int ModuleCount { get; }
        List<SuggestionData> GetSuggestionsFor(string text, KeywordData keyword);
    }
}
