using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Z.Api;
using Z.Api.Interfaces;
using Z.Api.Types;
using Z.Api.Utils;

namespace PgsModule
{
    public class Module : IZModule
    {
        // Private constants --------------------------------------------------

        private const string MODULE_NAME = "Pgs";
        private const string MODULE_DISPLAY_NAME = "PGS";

        private const string PGS_ACTION = "PgsShortcut";
        private const string PGS_KEYWORD = "pgs";
        private const string PGS_KEYWORD_DISPLAY = "PGS";
        private const string PGS_KEYWORD_COMMENT = "Shortcuts to internal PGS Software services";

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

        private readonly List<OperationInfo> operations = new List<OperationInfo>
        {
            new OperationInfo("my", "http://my.pgs-soft.com", "My PGS"),
            new OperationInfo("confluence", "http://confluence.pgs-soft.com", "Confluence"),
            new OperationInfo("jira", "http://jira.pgs-soft.com", "Jira"),
            new OperationInfo("food", "http://food.pgs-soft.com", "Food"),
            new OperationInfo("locator", "http://locator.pgs-soft.com", "Locator"),
            new OperationInfo("faq", "https://confluence.pgs-soft.com/display/PGS/FAQ+2.0", "FAQ"),
            new OperationInfo("mail", "https://owa.pgs-soft.com", "Outlook Web Access"),
            new OperationInfo("owa", "https://owa.pgs-soft.com", "Outlook Web Access"),
            new OperationInfo("wake", "https://wakeonlan.pgs-soft.com", "Wake on LAN"),
            new OperationInfo("structure", "https://my.pgs-soft.com/Person/OrganizationChart", "Struktura organizacyjna"),
            new OperationInfo("crucible", "https://crucible.pgs-soft.com", "Crucible"),
            new OperationInfo("bitbucket", "https://bitbucket.pgs-soft.com", "Bitbucket"),
            new OperationInfo("feedme", "https://feedme.pgs-soft.com", "Feedme - Rzeszów"),
            new OperationInfo("findme", "https://findme.pgs-soft.com", "Find me - Rzeszów"),
            new OperationInfo("entertime", "https://my.pgs-soft.com/karta-pracy-logowanie-czasu", "Wprowadzanie godzin"),
            new OperationInfo("logtime", "https://my.pgs-soft.com/karta-pracy-logowanie-czasu", "Logowanie zadań"),
            new OperationInfo("holidaylist", "https://my.pgs-soft.com/moje-wnioski", "Lista wniosków urlopowych"),
            new OperationInfo("holiday", "https://my.pgs-soft.com/urlopy-dodaj-wniosek", "Nowy wniosek urlopowy"),
            new OperationInfo("tasks", "https://my.pgs-soft.com/karta-pracy-pokaz-moje-zalogowane-czasy", "Zalogowane zadania"),
            new OperationInfo("absencecalendar", "https://my.pgs-soft.com/Report/Report/AbsenceCalendar", "Kalendarz nieobecności"),
            new OperationInfo("support", "https://support.pgs-soft.com", "Support")
        };

        private BitmapImage icon;

        // Public methods -----------------------------------------------------

        public Module()
        {
            operations.Sort((x, y) => x.Word.CompareTo(y.Word));

            icon = new BitmapImage(new Uri("pack://application:,,,/PgsModule;component/Resources/pgs.png"));
        }

        public void CollectSuggestions(string enteredText, string action, bool perfectMatchesOnly, ISuggestionCollector collector)
        {
            Func<OperationInfo, bool> filter;
            if (perfectMatchesOnly)
                filter = op => op.Word.ToUpper() == enteredText.ToUpper();
            else
                filter = op => (op.Word.ToUpper().Contains(enteredText.ToUpper()) || op.Description.ToUpper().Contains(enteredText.ToUpper()));

            operations
                .Where(filter)
                .Select(op => new SuggestionInfo(op.Word, op.Word, op.Description, icon, SuggestionUtils.EvalMatch(enteredText, op.Word, op.Description)))
                .ToList()
                .ForEach(s => collector.AddSuggestion(s));
        }

        public void ExecuteKeywordAction(string action, string expression, ExecuteOptions options)
        {
            OperationInfo info = operations.FirstOrDefault(op => op.Word.ToUpper() == expression.ToUpper());

            if (info != null)
                Process.Start(info.Command);
        }

        public void ExecuteSuggestion(SuggestionInfo suggestion, ExecuteOptions options)
        {
            OperationInfo info = operations.FirstOrDefault(op => op.Word.ToUpper() == suggestion.Text.ToUpper());

            if (info != null)
                Process.Start(info.Command);
        }

        public IEnumerable<KeywordInfo> GetKeywordActions()
        {
            return new List<KeywordInfo>
            {
                new KeywordInfo(PGS_KEYWORD, PGS_ACTION, PGS_KEYWORD_DISPLAY, PGS_KEYWORD_COMMENT)
            };
        }

        // Public properties --------------------------------------------------

        public string Name
        {
            get
            {
                return MODULE_NAME;
            }
        }

        public string DisplayName
        {
            get
            {
                return MODULE_DISPLAY_NAME;
            }
        }

        public ImageSource Icon => icon;
    }
}
