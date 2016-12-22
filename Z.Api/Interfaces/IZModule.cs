using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Xml;
using Z.Api.Types;

namespace Z.Api.Interfaces
{
    public interface IZModule
    {
        void CollectSuggestions(string enteredText, string action, bool perfectMatchesOnly, ISuggestionCollector collector);    
        void ExecuteKeywordAction(string action, string expression, ExecuteOptions options);
        void ExecuteSuggestion(SuggestionInfo suggestion, ExecuteOptions options);
        IEnumerable<KeywordInfo> GetKeywordActions();
        void Initialize(IModuleContext context);
        void Deinitialize();

        string Name { get; }
        string DisplayName { get; }
        ImageSource Icon { get; }
    }
}
