using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Z.Api.Interfaces;
using Z.Api.Types;

namespace PgsModule
{
    public class Module : IZModule
    {
        // Private constants --------------------------------------------------

        private const string MODULE_NAME = "Pgs";

        private const string PGS_ACTION = "PgsShortcut";
        private const string PGS_KEYWORD = "pgs";
        private const string PGS_KEYWORD_DISPLAY = "PGS";

        // Private types ------------------------------------------------------

        private class OperationInfo
        {
            public OperationInfo(string word, string command, string description)
            {
                this.Word = word;
                this.Command = command;
                this.Description = description;
            }

            public string Word { get; private set; }
            public string Command { get; private set; }
            public string Description { get; private set; }
        }

        // Private fields -----------------------------------------------------

        private readonly IReadOnlyCollection<OperationInfo> operations = new List<OperationInfo>
        {
            new OperationInfo("my", "http://my.pgs-soft.com", "My PGS"),
            new OperationInfo("confluence", "http://confluence.pgs-soft.com", "Confluence"),
            new OperationInfo("jira", "http://jira.pgs-soft.com", "Jira"),
            new OperationInfo("food", "http://food.pgs-soft.com", "Food")
        };

        // Public methods -----------------------------------------------------

        public void CollectSuggestions(string enteredText, string keywordAction, ISuggestionCollector collector)
        {
            operations
                .Where(op => op.Word.ToUpper().StartsWith(enteredText.ToUpper()))
                .Select(op => new Suggestion(op.Description, op))
                .ToList()
                .ForEach(s => collector.AddSuggestion(s));
        }

        public void ExecuteKeywordAction(string action, string expression)
        {
            OperationInfo info = operations.FirstOrDefault(op => op.Word.ToUpper().StartsWith(expression.ToUpper()));

            if (info != null)
                Process.Start(info.Command);
        }

        public IEnumerable<KeywordInfo> GetKeywordActions()
        {
            return new List<KeywordInfo>
            {
                new KeywordInfo(PGS_KEYWORD, PGS_ACTION, PGS_KEYWORD_DISPLAY)
            };
        }

        // Public properties --------------------------------------------------

        public string InternalName
        {
            get
            {
                return MODULE_NAME;
            }
        }
    }
}
