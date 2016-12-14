﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Z.Api.Types;

namespace Z.Api.Interfaces
{
    public interface IZModule
    {
        void ExecuteKeywordAction(string action, string expression);
        void CollectSuggestions(string enteredText, string keywordAction, ISuggestionCollector collector);    
        string InternalName { get; }
        string DisplayName { get; }
        void ExecuteSuggestion(SuggestionInfo suggestion);

        IEnumerable<KeywordInfo> GetKeywordActions();
    }
}