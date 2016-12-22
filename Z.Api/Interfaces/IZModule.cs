using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using Z.Api.Types;

namespace Z.Api.Interfaces
{
    public interface IZModule
    {
        void CollectSuggestions(string enteredText, string keywordAction, bool perfectMatchesOnly, ISuggestionCollector collector);    
        void ExecuteKeywordAction(string action, string expression, ExecuteOptions options);
        void ExecuteSuggestion(SuggestionInfo suggestion, ExecuteOptions options);

        string InternalName { get; }
        string DisplayName { get; }
        ImageSource Icon { get; }

        IEnumerable<KeywordInfo> GetKeywordActions();
    }
}
