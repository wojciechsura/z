using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Z.Api.Types;

namespace Z.Api.Interfaces
{
    public interface IZModule
    {
        void CollectSuggestions(string enteredText, string keywordAction, bool perfectMatchesOnly, ISuggestionCollector collector);    
        string InternalName { get; }
        string DisplayName { get; }
        void ExecuteKeywordAction(string action, string expression, ExecuteOptions options);
        void ExecuteSuggestion(SuggestionInfo suggestion, ExecuteOptions options);

        IEnumerable<KeywordInfo> GetKeywordActions();
    }
}
